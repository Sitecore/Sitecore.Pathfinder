// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        public const string BadWebsite = "Website.Bad\\bin";

        public const string GoodWebsite = "Website.Good\\bin";

        [NotNull]
        public string ProjectDirectory { get; private set; } = string.Empty;

        [NotNull]
        public Helpers.Services Services { get; private set; }

        protected void Mock<T>(T value)
        {
            Services.CompositionService.Set(value);
        }

        protected T Resolve<T>()
        {
            return Services.CompositionService.Resolve<T>();
        }

        protected void Start([NotNull] string website, [CanBeNull] Action mock = null)
        {
            var workingDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            ProjectDirectory = PathHelper.Combine(Path.GetDirectoryName(workingDirectory?.FullName) ?? string.Empty, website);

            Services = new Helpers.Services();
            Services.Start(ProjectDirectory, mock);

            Services.ConfigurationService.Load(ConfigurationOptions.None, ProjectDirectory);
        }
    }
}
