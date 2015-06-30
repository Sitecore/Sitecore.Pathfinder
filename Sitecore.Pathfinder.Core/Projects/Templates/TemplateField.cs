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
        }

        [NotNull]
        public Attribute<string> FieldName { get; } = new Attribute<string>("Name", string.Empty);

        [NotNull]
        public Attribute<string> LongHelp { get; } = new Attribute<string>("LongHelp", string.Empty);

        // todo: make shared and unversioned into attributes
        public bool Shared { get; set; }

        [NotNull]
        public Attribute<string> ShortHelp { get; } = new Attribute<string>("ShortHelp", string.Empty);

        [NotNull]
        public Attribute<int> SortOrder { get; } = new Attribute<int>("SortOrder", 0);

        [NotNull]
        public Attribute<string> Source { get; } = new Attribute<string>("Source", string.Empty);

        [NotNull]
        public Template Template { get; }

        [NotNull]
        public Attribute<string> Type { get; } = new Attribute<string>("Type", string.Empty);

        public bool Unversioned { get; set; }

        public void Merge([NotNull] TemplateField newField, bool overwrite)
        {
            // todo: consider making a strict and loose mode
            if (!string.IsNullOrEmpty(newField.Type.Value))
            {
                Type.Merge(newField.Type);
            }

            if (!string.IsNullOrEmpty(newField.Source.Value))
            {
                Source.Merge(newField.Source);
            }

            if (newField.Shared)
            {
                Shared = true;
            }

            if (newField.Unversioned)
            {
                Unversioned = true;
            }

            if (!string.IsNullOrEmpty(newField.ShortHelp.Value))
            {
                ShortHelp.Merge(newField.ShortHelp);
            }

            if (!string.IsNullOrEmpty(newField.LongHelp.Value))
            {
                LongHelp.Merge(newField.LongHelp);
            }

            if (newField.SortOrder.Value != 0)
            {
                SortOrder.Merge(newField.SortOrder);
            }
        }
    }
}
