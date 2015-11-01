// © 2015 Sitecore Corporation A/S. All rights reserved.

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

        [NotNull]
        public string ProjectDirectory { get; }

        public EmitSource EmitSource { get; }

        public virtual void Start()
        {
            var startup = new Startup();

            var configuration = startup.RegisterConfiguration(ProjectDirectory, EmitSource);
            var compositionService = startup.RegisterCompositionService(configuration);

            var emitter = compositionService.GetExportedValue<Emitter>();
            emitter.Start();                    
        }
    }
}
