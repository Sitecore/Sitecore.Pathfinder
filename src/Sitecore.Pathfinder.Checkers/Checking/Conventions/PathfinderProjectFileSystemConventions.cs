// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    public class PathfinderProjectFileSystemConventions : ConventionsBase
    {
        public PathfinderProjectFileSystemConventions()
        {
            ConventionCount = 11;
        }

        protected override IEnumerable<IEnumerable<Diagnostic>> Check()
        {
            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/items")
                select Warning("Missing ~/items directory for containing Sitecore items. To fix, create the ~/items directory");

            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/items/master") &&
                    !project.DirectoryExists("~/items/core")
                select Warning("The ~/items directory should have either a 'master' or 'core' subdirectory for containing Sitecore items. To fix, create either a ~/items/master or a ~/items/core directory");

            yield return 
                from project in Project
                where 
                    project.DirectoryExists("~/items/master") &&
                    !project.DirectoryExists("~/items/master/sitecore")
                select Warning("The ~/items/master directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/master/sitecore directory");

            yield return 
                from project in Project
                where 
                    project.DirectoryExists("~/items/core") &&
                    !project.DirectoryExists("~/items/core/sitecore")
                select Warning("The ~/items/core directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/core/sitecore directory");

            yield return 
                from project in Project
                where 
                    project.DirectoryExists("~/items/web") &&
                    !project.DirectoryExists("~/items/web/sitecore")
                select Warning("The ~/items/web directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/web/sitecore directory");

            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/sitecore.project")
                select Warning("Missing ~/sitecore.project directory. To fix, create the ~/sitecore.project directory");

            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/sitecore.project/packages")
                select Warning("Missing the ~/sitecore.project/packages directory. This directory contains imported packages. To fix, create the ~/sitecore.project/packages directory");

            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/sitecore.project/schemas")
                select Warning("Missing the ~/sitecore.project/schemas directory. This directory contains Xml and Json schema files that help text editors with Completion and Validation. To fix, create the ~/sitecore.project/schemas directory");

            yield return 
                from project in Project
                where 
                    !project.DirectoryExists("~/wwwroot")
                select Warning("Missing the ~/wwwroot directory. This directory contains files that are copied to the website directory without change. To fix, create the ~/wwwroot directory");

            yield return 
                from project in Project
                where 
                    !project.FileExists("~/scc.cmd")
                select Warning("Missing the ~/scc.cmd file. To fix, copy the scc.cmd file from the [Tools]/files/project directory");

            yield return 
                from project in Project
                where 
                    !project.FileExists("~/scconfig.json")
                select Warning("Missing the ~/scconfig.json file. The file contains the Pathfinder project configuration. To fix, copy the scconfig.json file from the [Tools]/files/project directory");
        }
    }
}