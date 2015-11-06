// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Configuration;

namespace Sitecore.Pathfinder.Emitters
{
    public enum EmitSource
    {
        NugetPackage,

        Directory
    }

    public class EmitService
    {
        public EmitService([NotNull] string projectDirectory, EmitSource emitSource)
        {
            ProjectDirectory = projectDirectory;
            EmitSource = emitSource;
        }

        public EmitSource EmitSource { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        public virtual void Start()
        {
            var startup = new Startup();

            var configuration = ConfigurationStartup.RegisterConfiguration(ProjectDirectory, ConfigurationOptions.Noninteractive);
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
