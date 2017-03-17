// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class ReferenceCheckers : Checker
    {
        [Check]
        public IEnumerable<Diagnostic> ReferenceNotFound(ICheckerContext context)
        {
            return from projectItem in context.Project.ProjectItems
                from reference in projectItem.References
                where !reference.IsValid
                select Error(Msg.C1000, "Reference not found", reference.TextNode, (reference is FileReference ? "file:/" : string.Empty) + reference.ReferenceText + (!string.IsNullOrEmpty(reference.DatabaseName) ? " [" + reference.DatabaseName + "]" : string.Empty));
        }
    }
}
