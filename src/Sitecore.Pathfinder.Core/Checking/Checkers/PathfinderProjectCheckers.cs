// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class PathfinderProjectCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingCoreSitecoreDirectory([NotNull] ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/core") && !DirectoryExists(context, "~/items/core/sitecore"))
            {
                yield return Warning(Msg.C1107, "The ~/items/core directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/core/sitecore directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingItemsDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/items"))
            {
                yield return Warning(Msg.C1108, "Missing ~/items directory for containing Sitecore items.", "To fix, create the ~/items directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingMasterOrCoreItemsDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/items/master") && !DirectoryExists(context, "~/items/core"))
            {
                yield return Warning(Msg.C1109, "The ~/items directory should have either a 'master' or 'core' subdirectory for containing Sitecore items. To fix, create either a ~/items/master or a ~/items/core directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingMasterSitecoreDirectory([NotNull] ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/master") && !DirectoryExists(context, "~/items/master/sitecore"))
            {
                yield return Warning(Msg.C1110, "The ~/items/master directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/master/sitecore directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingSccCmdFile([NotNull] ICheckerContext context)
        {
            if (!FileExists(context, "~/scc.cmd"))
            {
                yield return Warning(Msg.C1111, "Missing the ~/scc.cmd file. To fix, copy the scc.cmd file from the [Tools]/files/project directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingScconfigJsonFile([NotNull] ICheckerContext context)
        {
            if (!FileExists(context, "~/scconfig.json"))
            {
                yield return Warning(Msg.C1112, "Missing the ~/scconfig.json file. The file contains the Pathfinder project configuration. To fix, copy the scconfig.json file from the [Tools]/files/project directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingSitecoreProjectDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/sitecore.project"))
            {
                yield return Warning(Msg.C1113, "Missing ~/sitecore.project directory. To fix, create the ~/sitecore.project directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingSitecoreProjectSchemasDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/sitecore.project/schemas"))
            {
                yield return Warning(Msg.C1114, "Missing the ~/sitecore.project/schemas directory. This directory contains Xml and Json schema files that help text editors with Completion and Validation. To fix, create the ~/sitecore.project/schemas directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingWebSitecoreDirectory([NotNull] ICheckerContext context)
        {
            if (DirectoryExists(context, "~/items/web") && !DirectoryExists(context, "~/items/web/sitecore"))
            {
                yield return Warning(Msg.C1115, "The ~/items/web directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/web/sitecore directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> MissingWwwRootDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/wwwroot"))
            {
                yield return Warning(Msg.C1116, "Missing the ~/wwwroot directory. This directory contains files that are copied to the website directory without change. To fix, create the ~/wwwroot directory");
            }
        }
    }
}
