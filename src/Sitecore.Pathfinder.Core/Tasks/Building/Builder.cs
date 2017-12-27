// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks.Building
{
    [Export]
    public class Builder : TaskRunnerBase
    {
        [ImportingConstructor]
        public Builder([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFactory factory, [NotNull] IFileSystem fileSystem, [NotNull] IProjectService projectService, [NotNull, ItemNotNull, ImportMany] IEnumerable<ITask> tasks) : base(configuration, tasks)
        {
            Console = console;
            FileSystem = fileSystem;
            ProjectService = projectService;
            Factory = factory;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        [NotNull]
        protected IFactory Factory { get; }

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
            var context = Factory.BuildContext().With(LoadProject);

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
                var host = new Startup().WithStopWatch().WithProjectDirectory(projectDirectory).Start();
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

        protected virtual bool IsSolution() => FileSystem.FileExists(Path.Combine(Configuration.GetProjectDirectory(), "scconfig.solution.json"));

        [NotNull]
        protected virtual IProject LoadProject() => ProjectService.LoadProjectFromConfiguration();
    }
}
