// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using RazorLight;
using RazorLight.Extensions;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.CodeGeneration.Razor
{
    [Export(typeof(ITextTemplatingEngine)), Shared]
    public class RazorTextTemplatingEngine : ITextTemplatingEngine
    {
        [CanBeNull]
        private IRazorLightEngine _engine;

        [ImportingConstructor]
        public RazorTextTemplatingEngine([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public string Generate(string template, object model)
        {
            if (_engine == null)
            {
                _engine = EngineFactory.CreatePhysical(Configuration.GetProjectDirectory());
                Assert.IsNotNull(_engine);

                _engine.Configuration.Namespaces.AddRange(Configuration.GetArray(Constants.Configuration.GenerateCode.Imports));
            }

            return _engine.ParseString(template, model);
        }
    }
}
