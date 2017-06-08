// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ITextNodeParser)), Shared]
    public class YamlContentTextNodeParser : ContentTextNodeParserBase
    {
        [ImportingConstructor]
        public YamlContentTextNodeParser([NotNull] IFactory factory, [NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser, [NotNull] ISchemaService schemaService) : base(factory, trace, referenceParser, schemaService, Constants.TextNodeParsers.Items)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Snapshot is YamlTextSnapshot && textNode.Snapshot.SourceFile.AbsoluteFileName.EndsWith(".content.yaml", StringComparison.OrdinalIgnoreCase);
        }

        protected override ITextNode GetItemNameTextNode(IParseContext context, ITextNode textNode, string attributeName = "Name")
        {
            return !string.IsNullOrEmpty(textNode.Value) ? textNode : base.GetItemNameTextNode(context, textNode, attributeName);
        }

        protected override void ParseLayoutTextNode(ItemParseContext context, Item item, ITextNode layoutTextNode)
        {
            var parser = new YamlLayoutTextNodeParser(Factory, ReferenceParser);
            parser.Parse(context, layoutTextNode, item);
        }

        protected override void ParseFieldsTextNode(ItemParseContext context, Item item, ITextNode fieldsTextNode)
        {
            // parse shared fields
            var fieldContext = new LanguageVersionContext();
            ParseAttributes(context, item, fieldContext, fieldsTextNode);

            // parse unversioned and versioned fields
            foreach (var languageChildNode in fieldsTextNode.ChildNodes)
            {
                var languageVersionContext = new LanguageVersionContext();
                languageVersionContext.LanguageProperty.SetValue(new AttributeNameTextNode(languageChildNode));

                ParseAttributes(context, item, languageVersionContext, languageChildNode);

                foreach (var versionChildNode in languageChildNode.ChildNodes)
                {
                    var versionVersionContext = new LanguageVersionContext();
                    versionVersionContext.LanguageProperty.SetValue(languageVersionContext.LanguageProperty);
                    versionVersionContext.VersionProperty.SetValue(new AttributeNameTextNode(versionChildNode));

                    ParseAttributes(context, item, versionVersionContext, versionChildNode);
                }
            }
        }
    }
}
