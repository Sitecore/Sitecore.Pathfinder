// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Emitters
{
    public class EmitService
    {
        public EmitService([NotNull] string solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        [NotNull]
        public string SolutionDirectory { get; }

        public virtual void Start()
        {
            var startup = new Startup();

            var configuration = startup.RegisterConfiguration(SolutionDirectory);
            var compositionService = startup.RegisterCompositionService(configuration);

            var emitter = compositionService.GetExportedValue<Emitter>();
            emitter.Start();
        }
    }
}
