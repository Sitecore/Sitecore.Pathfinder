// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateSection : SourcePropertyBag, IHasSourceTextNodes
    {
        public TemplateSection([NotNull] Template template, Guid guid, [NotNull] ITextNode templateSectionTextNode)
        {
            Template = template;

            IconProperty = NewSourceProperty("Icon", string.Empty);
            SectionNameProperty = NewSourceProperty("Name", string.Empty);
            Fields = new LockableList<TemplateField>(this);

            SourceTextNodes = new LockableList<ITextNode>(this)
            {
                templateSectionTextNode
            };

            Uri = new ProjectItemUri(template.DatabaseName, guid);
        }

        [NotNull, ItemNotNull]
        public ICollection<TemplateField> Fields { get; }

        [NotNull]
        public string Icon
        {
            get { return IconProperty.GetValue(); }
            set { IconProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> IconProperty { get; }

        public override Locking Locking => Template.Locking;

        [NotNull]
        public string SectionName
        {
            get { return SectionNameProperty.GetValue(); }
            set { SectionNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> SectionNameProperty { get; }

        public ICollection<ITextNode> SourceTextNodes { get; }

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public IProjectItemUri Uri { get; private set; }

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
    }
}
