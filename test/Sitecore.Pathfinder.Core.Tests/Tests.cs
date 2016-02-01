﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Helpers;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        public const string BadWebsite = "Website.Bad\\bin";

        public const string GoodWebsite = "Website.Good\\bin";

        [Diagnostics.NotNull]
        public string ProjectDirectory { get; private set; } = string.Empty;

        [Diagnostics.NotNull]
        public Services Services { get; private set; }

        protected void Mock<T>([Diagnostics.NotNull] T value)
        {
            Services.CompositionService.Set(value);
        }

        [Diagnostics.NotNull]
        protected T Resolve<T>()
        {
            return Services.CompositionService.Resolve<T>();
        }

        protected void Start([Diagnostics.NotNull] string website, [Diagnostics.CanBeNull] Action mock = null)
        {
            var toolsDirectory = Directory.GetCurrentDirectory();

            // 3 levels up
            var rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            ProjectDirectory = PathHelper.Combine(rootDirectory ?? string.Empty, website);

            var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(ProjectDirectory).WithConfiguration(ConfigurationOptions.None).Start();
            if (app == null)
            {
                throw new ConfigurationException(@"Oh no, nothing works!");
            }

            Services = new Services().Start(app, mock);
        }
    }
}
