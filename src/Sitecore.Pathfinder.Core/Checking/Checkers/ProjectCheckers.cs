// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Checking.Checkers;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checkers
{
    [Export(typeof(Checker)), Shared]
    public class ProjectCheckers : Checker
    {
        [Check]
        public IEnumerable<Diagnostic> ProjectMustBeOutsideWebsite(ICheckerContext context)
        {
            if (FileExists(context, "~/sitecore/shell/sitecore.version.xml"))
            {
                yield return Warning(Msg.C1117, "Project should not be located inside the website. To fix, move the project to a new directory");
            }
        }
    }
}
