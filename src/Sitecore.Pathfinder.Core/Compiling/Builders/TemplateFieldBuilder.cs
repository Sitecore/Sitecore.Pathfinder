// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class TemplateFieldBuilder
    {
        public TemplateFieldBuilder([NotNull] IFactoryService factory)
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
        public string Source { get; set; } = string.Empty;

        [NotNull]
        public ITextNode SourceTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateFieldLongHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateFieldLongHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public string TemplateFieldShortHelp { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TemplateFieldShortHelpTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public TemplateSectionBuilder TemplateSectionBuilder { get; private set; }

        [NotNull]
        public string Type { get; set; } = string.Empty;

        [NotNull]
        public ITextNode TypeTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        public TemplateField Build([NotNull] Template template)
        {
            Guid fieldIdGuid;
            if (!Guid.TryParse(FieldId, out fieldIdGuid))
            {
                throw new InvalidOperationException("Template Field Guid is not valid");
            }

            var field = Factory.TemplateField(template, fieldIdGuid, FieldTextNode);

            field.FieldName = FieldName;
            if (FieldNameTextNode != TextNode.Empty)
            {
                field.FieldNameProperty.AddSourceTextNode(FieldNameTextNode);
            }

            field.Type = Type;
            if (TypeTextNode != TextNode.Empty)
            {
                field.TypeProperty.AddSourceTextNode(TypeTextNode);
            }

            field.Source = Source;
            if (SourceTextNode != TextNode.Empty)
            {
                field.SourceProperty.AddSourceTextNode(SourceTextNode);
            }

            return field;
        }

        [NotNull]
        public TemplateFieldBuilder With([NotNull] TemplateSectionBuilder templateBuilder, [NotNull] ITextNode fieldTextNode)
        {
            TemplateSectionBuilder = templateBuilder;
            FieldTextNode = fieldTextNode;

            return this;
        }
    }
}
