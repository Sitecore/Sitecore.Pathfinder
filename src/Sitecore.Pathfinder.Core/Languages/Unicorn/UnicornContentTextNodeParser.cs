// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class UnicornContentTextNodeParser : TextNodeParserBase
    {
        [ImportingConstructor]
        public UnicornContentTextNodeParser([NotNull] IFactory factory, [NotNull] ITraceService trace) : base(Constants.TextNodeParsers.Items)
        {
            Factory = factory;
            Trace = trace;
        }

        public override ISchemaService SchemaService { get; } = null;

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is UnicornTextSnapshot && textNode.Snapshot.SourceFile.AbsoluteFileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var guid = GetGuid(textNode, "ID");
            if (guid == Guid.Empty)
            {
                Trace.TraceError(Msg.P1035, "'ID' attribute is missing or invalid", textNode);
                return;
            }

            var templateGuid = GetGuid(textNode, "Template");
            if (templateGuid == Guid.Empty)
            {
                Trace.TraceError(Msg.P1036, "'Template' attribute is missing or invalid", textNode);
                return;
            }

            var databaseName = textNode.GetAttributeValue("DB", context.Database.DatabaseName);
            var database = context.ParseContext.Project.GetDatabase(databaseName);
            var itemIdOrPath = textNode.GetAttributeValue("Path");
            var itemName = Path.GetFileName(itemIdOrPath);
            var templateIdOrPath = templateGuid.Format();

            var item = Factory.Item(database, guid, itemName, itemIdOrPath, templateIdOrPath).With(textNode);

            var sharedFieldContext = new LanguageVersionContext();
            ParseFields(item, textNode, sharedFieldContext);

            foreach (var languageNode in textNode.ChildNodes.Where(n => n.Key == "Language"))
            {
                var unversionedFieldContext = new LanguageVersionContext();
                unversionedFieldContext.LanguageProperty.SetValue(languageNode);

                ParseFields(item, languageNode, unversionedFieldContext);

                foreach (var versionNode in languageNode.ChildNodes.Where(n => n.Key == "Version"))
                {
                    if (!int.TryParse(versionNode.Value, out var _))
                    {
                        Trace.TraceError(Msg.P1037, "Version number must be an integer", versionNode);
                        continue;
                    }

                    var versionedFieldContext = new LanguageVersionContext();
                    versionedFieldContext.LanguageProperty.SetValue(languageNode);
                    versionedFieldContext.VersionProperty.SetValue(versionNode);

                    ParseFields(item, versionNode, versionedFieldContext);
                }
            }

            context.ParseContext.Project.AddOrMerge(item);
        }

        private Guid GetGuid([NotNull] ITextNode textNode, [NotNull] string attributeName)
        {
            var value = textNode.GetAttributeValue(attributeName);
            return string.IsNullOrEmpty(value) ? Guid.Empty : GetGuid(value);
        }

        private static Guid GetGuid([NotNull] string value)
        {
            value = value.TrimStart('"').TrimEnd('"');
            return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
        }

        private void ParseFields([NotNull] Item item, [NotNull] ITextNode textNode, [NotNull] LanguageVersionContext fieldContext)
        {
            foreach (var fieldNode in textNode.ChildNodes.Where(n => n.Key == "ID"))
            {
                var fieldName = fieldNode.GetAttribute("Hint");
                if (fieldName == null)
                {
                    Trace.TraceError(Msg.P1038, "'Hint' attribute expected", fieldNode);
                    continue;
                }

                var fieldValue = fieldNode.GetAttribute("Value");
                if (fieldValue == null)
                {
                    Trace.TraceError(Msg.P1039, "'Value' attribute expected", fieldNode);
                    continue;
                }

                var field = Factory.Field(item).With(fieldNode);
                field.FieldNameProperty.SetValue(fieldName);
                field.ValueProperty.SetValue(fieldValue);
                field.LanguageProperty.SetValue(fieldContext.LanguageProperty);
                field.VersionProperty.SetValue(fieldContext.VersionProperty);

                item.Fields.Add(field);
            }
        }
    }
}
