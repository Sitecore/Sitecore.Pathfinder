// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;

namespace Sitecore.Pathfinder.Emitters
{
    public class EmitService
    {
        public EmitService([Diagnostics.NotNull] string toolsDirectory, [NotNull] string projectDirectory)
        {
            ProjectDirectory = projectDirectory;
            ToolsDirectory = toolsDirectory;
        }

        [NotNull]
        public string ProjectDirectory { get; }

        [Diagnostics.NotNull]
        public string ToolsDirectory { get; }

        public virtual void Start()
        {
            var startup = new Startup();

            var configuration = ConfigurationStartup.RegisterConfiguration(ToolsDirectory, ProjectDirectory, ConfigurationOptions.Noninteractive);
            if (configuration == null)
            {
                return;
            }

            var compositionService = startup.RegisterCompositionService(configuration);

            var emitter = compositionService.GetExportedValue<Emitter>();
            emitter.Start();
        }
    }
}
