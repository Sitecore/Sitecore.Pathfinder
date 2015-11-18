// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeField
    {
        [CanBeNull]
        private CodeTemplateField _templateField;

        public CodeField([NotNull] CodeProject project, [NotNull] CodeItem item, [NotNull] Field innerField)
        {
            Project = project;
            Item = item;
            InnerField = innerField;
        }

        [NotNull]
        public CodeDatabase Database => Item.Database;

        [NotNull]
        public Field InnerField { get; }

        [NotNull]
        public CodeItem Item { get; }

        [NotNull]
        public string Name => InnerField.FieldName;

        [NotNull]
        public CodeProject Project { get; }

        [CanBeNull]
        public CodeTemplateField TemplateField => _templateField ?? (_templateField = Item.Template.Fields.FirstOrDefault(f => string.Equals(f.Name, Name, StringComparison.OrdinalIgnoreCase)));

        [NotNull]
        public string Type => TemplateField?.Type ?? string.Empty;

        [NotNull]
        public string Value => InnerField.Value;
    }
}
