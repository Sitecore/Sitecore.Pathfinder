// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class ContentTextNodeParserBase : TextNodeParserBase
    {
        protected ContentTextNodeParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = GetItemNameTextNode(context.ParseContext, textNode);
            var parentItemPath = textNode.GetAttributeValue("ParentItemPath", context.ParentItemPath);
            var itemIdOrPath = PathHelper.CombineItemPath(parentItemPath, itemNameTextNode.Value);
            var guid = StringHelper.GetGuid(context.ParseContext.Project, textNode.GetAttributeValue("Id", itemIdOrPath));
            var databaseName = textNode.GetAttributeValue("Database", context.DatabaseName);
            var templateIdOrPath = textNode.Key.UnescapeXmlElementName();

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, textNode, guid, databaseName, itemNameTextNode.Value, itemIdOrPath, templateIdOrPath);
            item.ItemNameProperty.AddSourceTextNode(itemNameTextNode);
            item.TemplateIdOrPathProperty.AddSourceTextNode(new AttributeNameTextNode(textNode));
            item.IsEmittable = !string.Equals(textNode.GetAttributeValue(Constants.Fields.IsEmittable), "False", StringComparison.OrdinalIgnoreCase);
            item.IsImport = string.Equals(textNode.GetAttributeValue(Constants.Fields.IsImport, context.IsImport.ToString()), "True", StringComparison.OrdinalIgnoreCase);

            if (!item.IsImport)
            {
                item.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));
            }

            var fieldContext = new LanguageVersionContext();
            ParseAttributes(context, item, fieldContext, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(item);
        }

        protected virtual void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.Attributes)
            {
                if (childNode.Key == "Language")
                {
                    continue;
                }

                if (childNode.Key == "Version")
                {
                    continue;
                }

                ParseFieldTextNode(context, item, languageVersionContext, childNode);
            }
        }

        protected virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.ChildNodes)
            {
                switch (childNode.Key)
                {
                    case "Fields.Unversioned":
                        ParseUnversionedTextNode(context, item, childNode);
                        break;

                    case "Fields.Versioned":
                        ParseVersionedTextNode(context, item, childNode);
                        break;

                    case "Fields.Layout":
                        ParseLayoutTextNode(context, item, childNode);
                        break;

                    default:
                        var newContext = context.ParseContext.Factory.ItemParseContext(context.ParseContext, context.Parser, item.DatabaseName, item.ItemIdOrPath, item.IsImport);
                        context.Parser.ParseTextNode(newContext, childNode);
                        break;
                }
            }
        }

        protected abstract void ParseLayoutTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseUnversionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode childNode)
        {
            var fieldContext = new LanguageVersionContext();
            fieldContext.LanguageProperty.Parse(childNode);

            ParseAttributes(context, item, fieldContext, childNode);
        }

        protected virtual void ParseVersionedTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode childNode)
        {
            var fieldContext = new LanguageVersionContext();
            fieldContext.LanguageProperty.Parse(childNode);
            fieldContext.VersionProperty.Parse("Version", childNode);

            ParseAttributes(context, item, fieldContext, childNode);
        }

        protected virtual void ParseFieldTextNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] LanguageVersionContext languageVersionContext, [NotNull] ITextNode textNode)
        {
            var fieldName = textNode.Key.UnescapeXmlElementName();
            if (fieldName == "Name" || fieldName == "Id" || fieldName == "ParentItemPath" || fieldName == Constants.Fields.IsEmittable || fieldName == Constants.Fields.IsImport || fieldName == "Database")
            {
                return;
            }

            var field = context.ParseContext.Factory.Field(item, textNode);
            field.FieldNameProperty.SetValue(new AttributeNameTextNode(textNode));
            field.LanguageProperty.SetValue(languageVersionContext.LanguageProperty, SetValueOptions.DisableUpdates);
            field.VersionProperty.SetValue(languageVersionContext.VersionProperty, SetValueOptions.DisableUpdates);
            field.ValueProperty.SetValue(textNode);

            // check if field is already defined
            var duplicate = item.Fields.FirstOrDefault(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, field.Language, StringComparison.OrdinalIgnoreCase) && f.Version == field.Version);
            if (duplicate == null)
            {
                item.Fields.Add(field);
            }
            else
            {
                context.ParseContext.Trace.TraceError(Msg.P1008, Texts.Field_is_already_defined, textNode, duplicate.FieldName);
            }

            if (!item.IsImport)
            {
                item.References.AddRange(context.ParseContext.ReferenceParser.ParseReferences(item, field.ValueProperty));
            }
        }
    }
}
