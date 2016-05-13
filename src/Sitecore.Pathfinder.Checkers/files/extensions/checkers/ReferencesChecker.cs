// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class ReferencesChecker : Checker
    {
        [Export("Check")]
        public IEnumerable<Diagnostic> ReferenceNotFound(ICheckerContext context)
        {
            return from projectItem in context.Project.ProjectItems
                from reference in projectItem.References
                where !reference.IsValid
                select Error(Msg.C1000, "Reference not found", TraceHelper.GetTextNode(reference.SourceProperty), (reference is FileReference ? "file:/" : string.Empty) + reference.ReferenceText);
        }
    }
}
