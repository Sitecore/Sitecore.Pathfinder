# Checking a project
As a compiler, one of the primary goals for Pathfinder is to ensure that the project does not contain errors and provide warnings for 
potential problems. The is one of the main reasons, a Pathfinder project must contain the whole truth about the project.

After the project is loaded and parsed, Pathfinder will invoke a number of checkers, e.g. the Reference checker, that ensures that all
references between items and between items and files are valid.

A checker can be disabled, if it is not appropriate for a particular project.

New checkers can be implemented by adding extensions (see Extensions).

## Project roles
A classic website project is very different from a SPEAK project, e.g. a classic website lives in the Master database, while SPEAK lives
in the Core database under /sitecore/client.

You can configure a project to have a certain role in the scconfig.json by setting the "project-role" option.

The project role may be used in various tasks; it may enable or disable certain checkers, affect how code is generated
in the `generate-code` task, change the deployments targets etc.

The `project-role-checkers` settings map a project role to a number of checkers.

## Checking a website
Pathfinder can also check a website - even without having a Pathfinder project. 

First the Pathfinder files must be installed on the website. To do so, use the `update-website-files` task. Next you can run the `check-website` task,
which will execute all the checker on the website.

Alternative once the Pathfinder files have been installed, you can open the http://[Website]/pathfinder/check-website webpage, which allows you to 
enable and disable checkers to run and run the selected checkers.

Since this task runs on the website, Pathfinder can run additional task, that cannot be run in a project.

## Checkers
Checkers are pieces of code that validate a certain part of a project. Usually they depend on the project role. 

Project roles are configured in the `project-role-checkers` settings. Checkers can be either C# code in extensions.

A C# checker is a method that accepts a `ICheckerContext` parameters and returns a list of diagnostics and is marked with the Check
attribute. C# checkers are located in the [Tools]/files/extensions or [Project]/sitecore.project/extensions directory. Typically 
checkers are coded as Linq statements, for example:

```cs
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class SitecoreTemplateConventions : Checker
    {
        [Check]
        public IEnumerable<Diagnostic> Check(ICheckerContext context)
        {
            return 
                from item in context.Project.Items
                where 
                    !item.ItemIdOrPath.StartsWithIgnoreCase("/sitecore/templates/") &&
                    (
                        item.TemplateName.EqualsIgnoreCase("Template") ||
                        item.TemplateName.EqualsIgnoreCase("Template Section") ||
                        item.TemplateName.EqualsIgnoreCase("Template Field") ||
                        item.TemplateName.EqualsIgnoreCase("Template Folder")
                    )
                select Warning(Msg.C1000, "All items with template 'Template', 'Template section', 'Template field' and 'Template folder' should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", TraceHelper.GetTextNode(item));
        }
    }
}
```
