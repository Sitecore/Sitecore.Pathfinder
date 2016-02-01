// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder
{
    public abstract class Tests
    {
        public const string BadWebsite = "Website.Bad\\bin";

        public const string GoodWebsite = "Website.Good\\bin";

        public string ProjectDirectory { get; private set; } = string.Empty;

        public Helpers.Services Services { get; private set; }

        protected void Mock<T>(T value)
        {
            Services.CompositionService.Set(value);
        }

        protected T Resolve<T>()
        {
            return Services.CompositionService.Resolve<T>();
        }

        protected void Start(string website, Action mock = null)
        {
            var rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));

            ProjectDirectory = PathHelper.Combine(rootDirectory ?? string.Empty, website);

            var toolsDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine("ToolsDirectory: " + toolsDirectory);
            foreach (var file in Directory.GetFiles(toolsDirectory))
            {
                Console.WriteLine(file);
            }

            Console.WriteLine("ProjectDirectory: " + ProjectDirectory);
            foreach (var file in Directory.GetFiles(ProjectDirectory))
            {
                Console.WriteLine(file);
            }

            Services = new Helpers.Services();
            Services.Start(toolsDirectory, ProjectDirectory, mock);

            Services.ConfigurationService.Load(ConfigurationOptions.None, ProjectDirectory);
        }
    }
}
