// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Helpers;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        [NotNull]
        public string ProjectDirectory { get; private set; } = string.Empty;

        [NotNull]
        public Services Services { get; private set; }

        protected void Mock<T>(T value)
        {
            Services.CompositionService.Set<T>(value);
        }                                   

        protected T Resolve<T>()
        {
            return Services.CompositionService.Resolve<T>();
        }

        protected void Start([NotNull] string website = "Website", [CanBeNull] Action mock = null)
        {
            ProjectDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, website);

            Services = new Services();
            Services.Start(mock);

            Services.ConfigurationService.Load(LoadConfigurationOptions.None, ProjectDirectory);
        }
    }
}
