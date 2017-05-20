// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class FieldBuilder
    {
        public FieldBuilder([NotNull] IFactoryService factory)
        {
            Factory = factory;
        }

        [NotNull]
        public string FieldId { get; set; } = string.Empty;

        [NotNull]
        public ITextNode FieldIdTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string FieldName { get; set; } = string.Empty;

        [NotNull]
        public ITextNode FieldNameTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public ITextNode FieldTextNode { get; private set; }

        [NotNull]
        public ItemBuilder ItemBuilder { get; private set; }

        [NotNull]
        public Language Language { get; set; } = Language.Undefined;

        [NotNull]
        public ITextNode LanguageTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateLongHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateLongHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateShortHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateShortHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string Value { get; set; } = string.Empty;

        [NotNull]
        public string ValueHint { get; set; } = string.Empty;

        [NotNull]
        public ITextNode ValueHintTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public ITextNode ValueTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public Projects.Items.Version Version { get; set; } = Projects.Items.Version.Undefined;

        [NotNull]
        public ITextNode VersionTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        public Field Build([NotNull] Item item)
        {
            var field = Factory.Field(item).With(FieldTextNode);

            Guid fieldIdGuid;
            if (Guid.TryParse(FieldId, out fieldIdGuid))
            {
                field.FieldId = fieldIdGuid;
            }

            if (FieldIdTextNode != TextNode.Empty)
            {
                field.FieldIdProperty.AddSourceTextNode(FieldIdTextNode);
            }

            field.FieldName = FieldName;
            if (FieldNameTextNode != TextNode.Empty)
            {
                field.FieldNameProperty.AddSourceTextNode(FieldNameTextNode);
            }

            field.Language = Language;
            if (LanguageTextNode != TextNode.Empty)
            {
                field.LanguageProperty.AddSourceTextNode(LanguageTextNode);
            }

            field.Version = Version;
            if (VersionTextNode != TextNode.Empty)
            {
                field.VersionProperty.AddSourceTextNode(VersionTextNode);
            }

            field.Value = Value;
            if (ValueTextNode != TextNode.Empty)
            {
                field.ValueProperty.AddSourceTextNode(ValueTextNode);
            }

            return field;
        }

        [NotNull]
        public FieldBuilder With([NotNull] ItemBuilder itemBuilder, [NotNull] ITextNode fieldTextNode)
        {
            ItemBuilder = itemBuilder;
            FieldTextNode = fieldTextNode;

            return this;
        }
    }
}
