// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    [DebuggerDisplay("{GetType().Name,nq}: {FieldName}")]
    public class TemplateField : TextNodeSourcePropertyBag
    {
        [NotNull]
        public static readonly TemplateField Empty = new TemplateField(Template.Empty, new Guid("{D269BE69-A982-4415-ABC6-A870F286435A}"));

        [CanBeNull]
        private ID _id;

        public TemplateField([NotNull] Template template, Guid guid)
        {
            Template = template;

            FieldNameProperty = NewSourceProperty("Name", string.Empty);
            LongHelpProperty = NewSourceProperty("LongHelp", string.Empty);
            ShortHelpProperty = NewSourceProperty("ShortHelp", string.Empty);
            SortorderProperty = NewSourceProperty("SortOrder", 0);
            SourceProperty = NewSourceProperty("Source", string.Empty);
            TypeProperty = NewSourceProperty("Type", string.Empty);
            IconProperty = NewSourceProperty("Icon", string.Empty);

            Uri = new ProjectItemUri(template.DatabaseName, guid);
        }

        [NotNull]
        public Database Database => Template.Database;

        [NotNull]
        public string DatabaseName => Template.DatabaseName;

        [NotNull]
        public string FieldName
        {
            get { return FieldNameProperty.GetValue(); }
            set { FieldNameProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> FieldNameProperty { get; }

        [NotNull]
        public string Icon
        {
            get { return IconProperty.GetValue(); }
            set { IconProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> IconProperty { get; }

        [NotNull, Obsolete("Use Uri.Guid instead", false)]
        public ID ID => _id ?? (_id = new ID(Uri.Guid));

        public override Locking Locking => Template.Locking;

        [NotNull]
        public string LongHelp
        {
            get { return LongHelpProperty.GetValue(); }
            set { LongHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; }

        [NotNull, Obsolete("Use FieldName instead", false)]
        public string Name => FieldName;

        // todo: make shared and unversioned into attributes
        public bool Shared { get; set; }

        [NotNull]
        public string ShortHelp
        {
            get { return ShortHelpProperty.GetValue(); }
            set { ShortHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ShortHelpProperty { get; }

        public int Sortorder
        {
            get { return SortorderProperty.GetValue(); }
            set { SortorderProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<int> SortorderProperty { get; }

        [NotNull]
        public string Source
        {
            get { return SourceProperty.GetValue(); }
            set { SourceProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> SourceProperty { get; }

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public string Type
        {
            get { return TypeProperty.GetValue(); }
            set { TypeProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> TypeProperty { get; }

        public bool Unversioned { get; set; }

        [NotNull]
        public IProjectItemUri Uri { get; }

        public void Merge([NotNull] TemplateField newField, bool overwrite)
        {
            base.Merge(newField, overwrite);

            // todo: consider making a strict and loose mode
            if (!string.IsNullOrEmpty(newField.Type))
            {
                TypeProperty.SetValue(newField.TypeProperty);
            }

            if (!string.IsNullOrEmpty(newField.Source))
            {
                SourceProperty.SetValue(newField.SourceProperty);
            }

            if (newField.Shared)
            {
                Shared = true;
            }

            if (newField.Unversioned)
            {
                Unversioned = true;
            }

            if (!string.IsNullOrEmpty(newField.ShortHelp))
            {
                ShortHelpProperty.SetValue(newField.ShortHelpProperty);
            }

            if (!string.IsNullOrEmpty(newField.LongHelp))
            {
                LongHelpProperty.SetValue(newField.LongHelpProperty);
            }

            if (newField.Sortorder != 0)
            {
                SortorderProperty.SetValue(newField.SortorderProperty);
            }

            if (!string.IsNullOrEmpty(newField.Icon))
            {
                IconProperty.SetValue(newField.IconProperty);
            }
        }

        [NotNull]
        public TemplateField With([NotNull] ITextNode sourceTextNode)
        {
            WithSourceTextNode(sourceTextNode);
            return this;
        }
    }
}
