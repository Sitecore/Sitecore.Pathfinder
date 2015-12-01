// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateSection : IHasSourceTextNodes, IUnloadable
    {
        public TemplateSection([NotNull] Template template, Guid guid, [NotNull] ITextNode templateSectionTextNode)
        {
            Template = template;
            SourceTextNodes.Add(templateSectionTextNode);

            Uri = new ProjectItemUri(template.DatabaseName, guid);
        }

        [NotNull]
        [ItemNotNull]
        public IList<TemplateField> Fields { get; } = new List<TemplateField>();

        [NotNull]
        public string Icon
        {
            get { return IconProperty.GetValue(); }
            set { IconProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> IconProperty { get; } = new SourceProperty<string>("Icon", string.Empty);

        [NotNull]
        public string SectionName
        {
            get { return SectionNameProperty.GetValue(); }
            set { SectionNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> SectionNameProperty { get; } = new SourceProperty<string>("Name", string.Empty);

        public ICollection<ITextNode> SourceTextNodes { get; } = new List<ITextNode>();

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public ProjectItemUri Uri { get; private set; }

        public void Merge([NotNull] TemplateSection newSection, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newSection.Icon))
            {
                IconProperty.SetValue(newSection.IconProperty, SetValueOptions.DisableUpdates);
            }

            foreach (var newField in newSection.Fields)
            {
                var field = Fields.FirstOrDefault(f => string.Equals(f.FieldName, newField.FieldName, StringComparison.OrdinalIgnoreCase));
                if (field == null)
                {
                    Fields.Add(newField);
                    continue;
                }

                field.Merge(newField, overwrite);
            }
        }

        void IUnloadable.Unload()
        {
            foreach (var unloadable in Fields.OfType<IUnloadable>())
            {
                unloadable.Unload();
            }
        }
    }
}
