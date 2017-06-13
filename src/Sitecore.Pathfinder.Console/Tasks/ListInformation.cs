// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ListInformation : BuildTaskBase
    {
        [ImportingConstructor]
        public ListInformation([NotNull] IConsoleService console, [NotNull] ICheckerService checkerService) : base("list-information", "list")
        {
            Console = console;
            CheckerService = checkerService;

            Shortcut = "l";
        }

        [NotNull, Option("infomation", Alias = "i", IsRequired = true, PromptText = "Pick information to display", HelpText = "Information to display", PositionalArg = 1, HasOptions = true)]
        public string Information { get; set; } = string.Empty;

        [NotNull]
        protected ICheckerService CheckerService { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            switch (Information.ToLowerInvariant())
            {
                case "items":
                    ListItems(context);
                    break;
                case "templates":
                    ListTemplates(context);
                    break;
                case "files":
                    ListFiles(context);
                    break;
                case "output":
                    ListOutput(context);
                    break;
                case "project":
                    ListProject(context);
                    break;
                case "checkers":
                    ListCheckers(context);
                    break;
                case "roles":
                    ListRoles(context);
                    break;
                default:
                    Console.WriteLine("Information not found");
                    break;
            }
        }

        [NotNull, OptionValues("Information")]
        protected IEnumerable<(string Name, string Value)> GetInformationOptions([NotNull] ITaskContext context)
        {
            yield return ("Items", "items");
            yield return ("Templates", "templates");
            yield return ("Files", "files");
            yield return ("Output", "output");
            yield return ("Project", "project");
            yield return ("Checkers", "checkers");
            yield return ("Roles", "roles");
        }

        protected void ListCheckers([NotNull] IBuildContext context)
        {
            foreach (var checker in CheckerService.GetEnabledCheckers().OrderBy(c => c.Name))
            {
                var name = checker.Name + " [" + checker.Category + "]"; ;
                context.Trace.WriteLine(name);
            }
        }

        protected void ListFiles([NotNull] IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var item in project.Files.OrderBy(file => file.FilePath))
            {
                context.Trace.WriteLine(item.FilePath);
            }
        }

        protected void ListItems([NotNull] IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var item in project.Items.OrderBy(i => i.ItemIdOrPath))
            {
                context.Trace.WriteLine(item.ItemIdOrPath);
            }
        }

        protected void ListTemplates([NotNull] IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var item in project.Templates.OrderBy(t => t.ItemIdOrPath))
            {
                context.Trace.WriteLine(item.ItemIdOrPath);
            }
        }

        protected void ListOutput([NotNull] IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var item in project.Items.Where(i => !i.IsImport).OrderBy(i => i.ItemIdOrPath))
            {
                var text = item.ItemIdOrPath;
                if (string.IsNullOrEmpty(text))
                {
                    text = item.ItemName;
                }

                text += " [" + item.TemplateName + "]";

                if (string.IsNullOrEmpty(text))
                {
                    text = "? [Item]";
                }

                Console.WriteLine(text);
            }

            foreach (var filePath in project.Files.OrderBy(i => i.FilePath))
            {
                var fileName = filePath.FilePath;
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                Console.WriteLine(fileName);
            }
        }

        protected void ListProject([NotNull] IBuildContext context)
        {
            var project = context.LoadProject();

            foreach (var projectItem in project.ProjectItems.OrderBy(i => i.GetType().Name))
            {
                var qualifiedName = projectItem.QualifiedName;

                if (projectItem is File)
                {
                    qualifiedName = "\\" + PathHelper.UnmapPath(project.ProjectDirectory, qualifiedName);
                }

                context.Trace.WriteLine($"{qualifiedName} ({projectItem.GetType().Name})");
            }
        }

        protected void ListRoles([NotNull] IBuildContext context)
        {
            foreach (var pair in context.Configuration.GetSubKeys(Constants.Configuration.ProjectRoleCheckers).OrderBy(k => k.Key))
            {
                context.Trace.WriteLine(pair.Key);
            }
        }
    }
}
