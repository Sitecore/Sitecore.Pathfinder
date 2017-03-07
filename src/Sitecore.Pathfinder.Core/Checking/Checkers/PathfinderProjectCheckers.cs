// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checkers
{
    public class PathfinderProjectCheckers : Checker
    {
        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingCoreSitecoreDirectory(ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/core") && !DirectoryExists(context, "~/items/core/sitecore"))
            {
                yield return Warning(Msg.C1107, "The ~/items/core directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/core/sitecore directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingItemsDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/items"))
            {
                yield return Warning(Msg.C1108, "Missing ~/items directory for containing Sitecore items.", "To fix, create the ~/items directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingMasterOrCoreItemsDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/items/master") && !DirectoryExists(context, "~/items/core"))
            {
                yield return Warning(Msg.C1109, "The ~/items directory should have either a 'master' or 'core' subdirectory for containing Sitecore items. To fix, create either a ~/items/master or a ~/items/core directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingMasterSitecoreDirectory(ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/master") && !DirectoryExists(context, "~/items/master/sitecore"))
            {
                yield return Warning(Msg.C1110, "The ~/items/master directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/master/sitecore directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingSccCmdFile(ICheckerContext context)
        {
            if (!FileExists(context, "~/scc.cmd"))
            {
                yield return Warning(Msg.C1111, "Missing the ~/scc.cmd file. To fix, copy the scc.cmd file from the [Tools]/files/project directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingScconfigJsonFile(ICheckerContext context)
        {
            if (!FileExists(context, "~/scconfig.json"))
            {
                yield return Warning(Msg.C1112, "Missing the ~/scconfig.json file. The file contains the Pathfinder project configuration. To fix, copy the scconfig.json file from the [Tools]/files/project directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingSitecoreProjectDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/sitecore.project"))
            {
                yield return Warning(Msg.C1113, "Missing ~/sitecore.project directory. To fix, create the ~/sitecore.project directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingSitecoreProjectSchemasDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/sitecore.project/schemas"))
            {
                yield return Warning(Msg.C1114, "Missing the ~/sitecore.project/schemas directory. This directory contains Xml and Json schema files that help text editors with Completion and Validation. To fix, create the ~/sitecore.project/schemas directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingWebSitecoreDirectory(ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/web") && !DirectoryExists(context, "~/items/web/sitecore"))
            {
                yield return Warning(Msg.C1115, "The ~/items/web directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/web/sitecore directory");
            }
        }

        [Export("Check")]
        protected IEnumerable<Diagnostic> MissingWwwRootDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/wwwroot"))
            {
                yield return Warning(Msg.C1116, "Missing the ~/wwwroot directory. This directory contains files that are copied to the website directory without change. To fix, create the ~/wwwroot directory");
            }
        }
    }
}
