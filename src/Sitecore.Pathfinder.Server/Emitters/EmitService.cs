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
        public EmitService([NotNull] string solutionDirectory, EmitSource emitSource)
        {
            SolutionDirectory = solutionDirectory;
            EmitSource = emitSource;
        }

        [NotNull]
        public string SolutionDirectory { get; }

        public EmitSource EmitSource { get; }

        public virtual void Start()
        {
            var startup = new Startup();

            var configuration = startup.RegisterConfiguration(SolutionDirectory, EmitSource);
            var compositionService = startup.RegisterCompositionService(configuration);

            var emitter = compositionService.GetExportedValue<Emitter>();
            emitter.Start();                    
        }
    }
}
