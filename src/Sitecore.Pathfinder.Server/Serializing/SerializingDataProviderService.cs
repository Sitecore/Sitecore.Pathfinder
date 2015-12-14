// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Serializing
{
    public static class SerializingDataProviderService
    {
        private static int _disabled;

        private static bool _isLoaded;

        public static bool Disabled
        {
            get { return _disabled != 0; }
            set
            {
                if (value)
                {
                    _disabled++;
                }
                else
                {
                    _disabled--;
                    if (_disabled < 0)
                    {
                        throw new InvalidOperationException("Calls to Disabled are not balanced");
                    }
                }
            }
        }

        [Diagnostics.NotNull, ItemNotNull]
        public static ICollection<WebsiteSerializer> WebsiteSerializers { get; } = new List<WebsiteSerializer>();

        public static void Initialize()
        {
            _isLoaded = false;
        }

        public static void Reload()
        {
            Load();
        }

        public static void RemoveItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] ID itemId)
        {
            if (Disabled)
            {
                return;
            }

            if (!_isLoaded)
            {
                Load();
            }

            foreach (var serializer in WebsiteSerializers)
            {
                try
                {
                    serializer.RemoveItem(databaseName, itemId);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to remove item", ex, typeof(SerializingDataProviderService));
                }
            }
        }

        public static void RemoveItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] ID itemId, [Diagnostics.NotNull] string oldName)
        {
            if (Disabled)
            {
                return;
            }

            if (!_isLoaded)
            {
                Load();
            }

            foreach (var serializer in WebsiteSerializers)
            {
                try
                {
                    serializer.RemoveItem(databaseName, itemId, oldName);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to remove item", ex, typeof(SerializingDataProviderService));
                }
            }
        }

        public static void SerializeItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] ID itemId)
        {
            if (Disabled)
            {
                return;
            }

            if (!_isLoaded)
            {
                Load();
            }

            foreach (var serializer in WebsiteSerializers)
            {
                try
                {
                    serializer.SerializeItem(databaseName, itemId);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to serialize item", ex, typeof(SerializingDataProviderService));
                }
            }
        }

        public static void SerializeItem([Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] ID itemId, [Diagnostics.NotNull] ID newParentId)
        {
            if (Disabled)
            {
                return;
            }

            if (!_isLoaded)
            {
                Load();
            }

            foreach (var serializer in WebsiteSerializers)
            {
                try
                {
                    serializer.SerializeItem(databaseName, itemId, newParentId);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to serialize item", ex, typeof(SerializingDataProviderService));
                }
            }
        }

        private static void Load()
        {
            _isLoaded = true;

            WebsiteSerializers.Clear();

            var dataFolder = FileUtil.MapPath(Settings.DataFolder);
            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");
            if (!FileUtil.FileExists(fileName))
            {
                return;
            }

            var xml = FileUtil.ReadFromFile(fileName);       
            var root = xml.ToXElement();
            if (root == null)
            {
                return;
            }

            foreach (var element in root.Elements())
            {
                var toolsDirectory = element.GetAttributeValue("toolsdirectory");
                var projectDirectory = element.GetAttributeValue("projectdirectory");
                var binDirectory = FileUtil.MapPath("/bin");

                if (!Directory.Exists(projectDirectory))
                {
                    continue;
                }

                if (!Directory.Exists(toolsDirectory))
                {
                    continue;
                }

                var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).WithExtensionsDirectory(binDirectory).Start();
                if (app == null)
                {
                    throw new ConfigurationException("Failed to load configuration");
                }

                WebsiteSerializers.Add(app.CompositionService.Resolve<WebsiteSerializer>().With(toolsDirectory, projectDirectory));
            }
        }
    }
}
