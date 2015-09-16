// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Helpers;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        public const string GoodWebsite = "..\\..\\..\\Website.Good\\bin";
        public const string BadWebsite = "..\\..\\..\\Website.Bad\\bin";

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

        protected void Start([NotNull] string website, [CanBeNull] Action mock = null)
        {
            ProjectDirectory = PathHelper.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, website);

            Services = new Services();
            Services.Start(ProjectDirectory, mock);

            Services.ConfigurationService.Load(LoadConfigurationOptions.None, ProjectDirectory);
        }
    }
}
