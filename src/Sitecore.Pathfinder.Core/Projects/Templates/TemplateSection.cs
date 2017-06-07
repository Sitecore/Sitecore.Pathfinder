// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateSection : TextNodeSourcePropertyBag
    {
        [FactoryConstructor]
        public TemplateSection([NotNull] Template template, Guid guid)
        {
            Template = template;

            IconProperty = NewSourceProperty("Icon", string.Empty);
            SectionNameProperty = NewSourceProperty("Name", string.Empty);
            SortorderProperty = NewSourceProperty("Sortorder", 0);

            Fields = new LockableList<TemplateField>(this);
            Uri = new ProjectItemUri(template.DatabaseName, guid);
        }

        [NotNull, ItemNotNull]
        public ICollection<TemplateField> Fields { get; }

        [NotNull]
        public string Icon
        {
            get => IconProperty.GetValue();
            set => IconProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> IconProperty { get; }

        public bool IsSynthetic { get; set; }

        public override Locking Locking => Template.Locking;

        [NotNull]
        public string SectionName
        {
            get => SectionNameProperty.GetValue();
            set => SectionNameProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<string> SectionNameProperty { get; }

        public int Sortorder
        {
            get => SortorderProperty.GetValue();
            set => SortorderProperty.SetValue(value);
        }

        [NotNull]
        public SourceProperty<int> SortorderProperty { get; }

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public IProjectItemUri Uri { get; }

        public void Merge([NotNull] TemplateSection newSection, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newSection.Icon))
            {
                IconProperty.SetValue(newSection.IconProperty);
            }

            if (newSection.Sortorder != 0)
            {
                SortorderProperty.SetValue(newSection.SortorderProperty);
            }

            if (IsSynthetic && !newSection.IsSynthetic)
            {
                IsSynthetic = false;
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

        [NotNull]
        public TemplateSection With([NotNull] ITextNode sourceTextNode)
        {
            WithSourceTextNode(sourceTextNode);
            return this;
        }
    }
}
