// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class ReferenceChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var projectItem in context.Project.ProjectItems)
            {
                foreach (var reference in projectItem.References)
                {
                    if (!reference.IsValid)
                    {
                        var details = reference.SourceProperty.GetValue();
                        context.Trace.TraceWarning("Reference not found", projectItem.Snapshots.First().SourceFile.AbsoluteFileName, reference.SourceProperty?.SourceTextNode?.TextSpan ?? TextSpan.Empty, details);
                    }
                }
            }
        }
    }
}
