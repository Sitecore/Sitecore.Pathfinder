// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Initializing
{
    [Export(typeof(ITask))]
    public class InitProject : TaskBase
    {
        public InitProject() : base("init-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.SolutionDirectory;
            if (!context.FileSystem.DirectoryExists(projectDirectory))
            {
                CreateProjectDirectory(context, projectDirectory);
                return;
            }

            var configFileName = Path.Combine(projectDirectory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
            if (!context.FileSystem.FileExists(configFileName))
            {
                CreateConfigurationFile(context, projectDirectory);
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Initializes the project.");
        }

        protected virtual void CopyResourceFiles([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\project\\*");

            context.FileSystem.XCopy(sourceDirectory, projectDirectory);
        }

        protected virtual void CreateConfigurationFile([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            CopyResourceFiles(context, projectDirectory);
            context.Trace.Writeline(Texts.Your_configuration_file_and_sample_files_were_missing__so_I_have_created_them__You_must_update_the__project_unique_id____wwwroot__and__hostname__in_the___0___configuration_file_before_continuing_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        }

        protected virtual void CreateProjectDirectory([NotNull] IBuildContext context, [NotNull] string projectDirectory)
        {
            context.FileSystem.CreateDirectory(projectDirectory);
            CopyResourceFiles(context, projectDirectory);

            context.Trace.Writeline(Texts.Hi_there_);
            context.Trace.Writeline(Texts.Your_project_directory_was_missing__so_I_have_created_it__You_must_update_the__project_unique_id____wwwroot__and__hostname__in_the___0___configuration_file_before_continuing_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        }
    }
}
