// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Templates
{
    public class UseIdInsteadOfPathChecker : CheckerBase
    {
        public UseIdInsteadOfPathChecker() : base("Use ID instead of path", TemplateFields)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var template in context.Project.Templates)
            {
                foreach (var templateField in template.Fields)
                {
                    if (templateField.Source.IndexOf('/') >= 0)
                    {
                        context.Trace.TraceWarning("Use IDs instead of paths in template fields", TraceHelper.GetTextNode(templateField.SourceProperty, templateField), $"The template field Source field contains the path \"{templateField.Source}\". It is recommended to use IDs instead.");
                    }
                }
            }
        }
    }
}
