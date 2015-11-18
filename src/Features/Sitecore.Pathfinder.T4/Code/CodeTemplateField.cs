// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeTemplateField
    {
        public CodeTemplateField([NotNull] CodeProject project, [NotNull] CodeTemplate template, [NotNull] TemplateField innerTemplateField)
        {
            Project = project;
            Template = template;
            InnerTemplateField = innerTemplateField;
        }

        [NotNull]
        public CodeDatabase Database => Template.Database;

        [NotNull]
        public TemplateField InnerTemplateField { get; }

        [NotNull]
        public string LongHelp => InnerTemplateField.LongHelp;

        [NotNull]
        public string Name => InnerTemplateField.FieldName;

        [NotNull]
        public CodeProject Project { get; }

        public bool Shared => InnerTemplateField.Shared;

        [NotNull]
        public string ShortHelp => InnerTemplateField.ShortHelp;

        [NotNull]
        public string Source => InnerTemplateField.Source;

        [NotNull]
        public CodeTemplate Template { get; }

        [NotNull]
        public string Type => InnerTemplateField.Type;

        public bool Unversioned => InnerTemplateField.Unversioned;
    }
}
