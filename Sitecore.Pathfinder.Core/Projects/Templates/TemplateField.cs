// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class TemplateField
    {
        public static readonly TemplateField Empty = new TemplateField(Template.Empty);

        public TemplateField([NotNull] Template template)
        {
            Template = template;
            FieldName = new Attribute<string>("Name", string.Empty);
        }

        [NotNull]
        public string LongHelp { get; set; } = string.Empty;

        [NotNull]
        public Attribute<string> FieldName { get; }

        public bool Shared { get; set; }

        [NotNull]
        public string ShortHelp { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        [NotNull]
        public string Source { get; set; } = string.Empty;

        [NotNull]
        public string StandardValue { get; set; } = string.Empty;

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public string Type { get; set; } = string.Empty;

        public bool Unversioned { get; set; }

        public void Merge([NotNull] TemplateField newField, bool overwrite)
        {
            if (!string.IsNullOrEmpty(newField.Type))
            {
                Type = newField.Type;
            }

            if (!string.IsNullOrEmpty(newField.Source))
            {
                Source = newField.Source;
            }

            if (newField.Shared)
            {
                Shared = true;
            }

            if (newField.Unversioned)
            {
                Unversioned = true;
            }

            if (!string.IsNullOrEmpty(newField.StandardValue))
            {
                StandardValue = newField.StandardValue;
            }

            if (!string.IsNullOrEmpty(newField.ShortHelp))
            {
                ShortHelp = newField.ShortHelp;
            }

            if (!string.IsNullOrEmpty(newField.LongHelp))
            {
                LongHelp = newField.LongHelp;
            }

            if (newField.SortOrder != 0)
            {
                SortOrder = newField.SortOrder;
            }
        }
    }
}
