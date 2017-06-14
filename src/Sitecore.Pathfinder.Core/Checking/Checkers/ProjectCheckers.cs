// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class ProjectCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> ProjectMustBeOutsideWebsite([NotNull] ICheckerContext context)
        {
            if (FileExists(context, "~/sitecore/shell/sitecore.version.xml"))
            {
                yield return Warning(context, Msg.C1117, "Project should not be located inside the website. To fix, move the project to a new directory");
            }
        }
    }
}
