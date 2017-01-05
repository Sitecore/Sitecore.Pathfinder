// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Building
{
    [Export]
    public class Builder : TaskRunnerBase
    {
        [ImportingConstructor]
        public Builder([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem, [NotNull] IProjectService projectService, [NotNull] ExportFactory<IBuildContext> buildContextFactory, [NotNull, ItemNotNull, ImportMany] IEnumerable<ITask> tasks) : base(configuration, tasks)
        {
            Console = console;
            FileSystem = fileSystem;
            ProjectService = projectService;
            BuildContextFactory = buildContextFactory;
        }

        [NotNull]
        protected ExportFactory<IBuildContext> BuildContextFactory { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        public override int Start()
        {
            if (IsSolution())
            {
                return BuildSolution();
            }

            return BuildProject();
        }

        protected virtual int BuildProject()
        {
            var context = BuildContextFactory.New().With(LoadProject);

            RegisterProjectDirectory(context);

            RunTasks(context);

            return context.ErrorCode;
        }

        protected virtual int BuildSolution()
        {
            var failed = 0;
            var succeeded = 0;
            var solutionDirectory = Configuration.GetProjectDirectory();

            foreach (var pair in Configuration.GetSubKeys("projects"))
            {
                Console.WriteLine($"========== Compiling project: {pair.Key} ==========");

                var relativePath = Configuration.GetString("projects:" + pair.Key);
                var projectDirectory = PathHelper.Combine(solutionDirectory, PathHelper.NormalizeFilePath(relativePath));

                // create a new host for each project, so they do not interfer
                var host = new Startup().WithStopWatch().WithTraceListeners().AsInteractive().WithWebsiteAssemblyResolver().WithProjectDirectory(projectDirectory).Start();
                if (host == null)
                {
                    return -1;
                }

                var builder = host.GetTaskRunner<Builder>();

                var code = builder.Start();
                if (code != 0)
                {
                    failed++;
                }
                else
                {
                    succeeded++;
                }

                Console.WriteLine();
            }

            Console.WriteLine($"========== Build solution: {succeeded} succeeded, {failed} failed ==========");

            return failed > 0 ? -1 : 0;
        }

        protected override IList<string> GetTaskNames(ITaskContext context)
        {
            var tasks = base.GetTaskNames(context);

            // insert setup tasks
            foreach (var task in Tasks.OrderBy(t => t.TaskName))
            {
                if (task is ISetupTask)
                {
                    tasks.Insert(0, task.TaskName);
                }
            }

            return tasks;
        }

        protected virtual bool IsSolution()
        {
            return FileSystem.FileExists(Path.Combine(Configuration.GetProjectDirectory(), "scconfig.solution.json"));
        }

        [NotNull]
        protected virtual IProject LoadProject()
        {
            return ProjectService.LoadProjectFromConfiguration();
        }

        protected virtual void RegisterProjectDirectory([NotNull] IBuildContext context)
        {
            // registering a project directory in the website Data Folder allows the website and other tools
            // to locate the project 
            var dataFolder = context.Configuration.GetString(Constants.Configuration.DataFolderDirectory);
            if (!FileSystem.DirectoryExists(dataFolder))
            {
                return;
            }

            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");
            FileSystem.CreateDirectory(pathfinderFolder);

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");

            var xml = FileSystem.FileExists(fileName) ? FileSystem.ReadAllText(fileName) : "<projects />";

            var root = xml.ToXElement() ?? "<projects />".ToXElement();
            if (root == null)
            {
                // silent
                return;
            }

            // check if already registered
            if (root.Elements().Any(e => string.Equals(e.GetAttributeValue("projectdirectory"), context.ProjectDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            root.Add(new XElement("project", new XAttribute("toolsdirectory", context.ToolsDirectory), new XAttribute("projectdirectory", context.ProjectDirectory)));

            if (root.Document != null)
            {
                root.Document.Save(fileName);
            }
        }
    }
}
