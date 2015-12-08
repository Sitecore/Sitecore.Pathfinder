// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Importing.ItemImporters;
using Sitecore.Pathfinder.Languages;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Serializing
{
    public class SerializationService : ISerializationService
    {
        [Diagnostics.NotNull, ItemNotNull]
        private readonly Queue<QueuedSerializationItem> _queue = new Queue<QueuedSerializationItem>();

        [Diagnostics.CanBeNull, ItemNotNull]
        private IEnumerable<ProjectSerializer> _projectSerializers;

        [ImportingConstructor]
        public SerializationService([Diagnostics.NotNull] ICompositionService compositionService, [Diagnostics.NotNull] IItemImporterService itemImporter, [Diagnostics.NotNull] ILanguageService languageService)
        {
            CompositionService = compositionService;
            ItemImporter = itemImporter;
            LanguageService = languageService;

            Instance = this;
        }

        [Diagnostics.CanBeNull]
        public static ISerializationService Instance { get; private set; }

        [Diagnostics.NotNull, ItemNotNull]
        public IEnumerable<ProjectSerializer> ProjectSerializers => _projectSerializers ?? (_projectSerializers = LoadProjectSerializers());

        [Diagnostics.NotNull]
        protected ICompositionService CompositionService { get; }

        [Diagnostics.NotNull]
        protected IItemImporterService ItemImporter { get; }

        [Diagnostics.NotNull]
        protected ILanguageService LanguageService { get; }

        public void SerializeItem(Database database, ID itemId)
        {
            _queue.Enqueue(new QueuedSerializationItem(database, itemId));

            // todo: put this in a new thread
            SerializeNextItem();
        }

        [Diagnostics.NotNull, ItemNotNull]
        protected virtual IEnumerable<ProjectSerializer> LoadProjectSerializers()
        {
            var projectSerializers = new List<ProjectSerializer>();

            var dataFolder = FileUtil.MapPath(Settings.DataFolder);
            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");
            if (!FileUtil.FileExists(fileName))
            {
                return projectSerializers;
            }

            var xml = FileUtil.ReadFromFile(fileName);
            var root = xml.ToXElement();
            if (root == null)
            {
                return projectSerializers;
            }

            foreach (var element in root.Elements())
            {
                var toolsDirectory = element.GetAttributeValue("toolsdirectory");
                var projectDirectory = element.GetAttributeValue("projectdirectory");

                try
                {
                    var project = new ProjectSerializer(CompositionService).With(toolsDirectory, projectDirectory);
                    projectSerializers.Add(project);
                }
                catch
                {
                    Log.Error("Failed to load Pathfinder project: " + projectDirectory, GetType());
                }
            }

            return projectSerializers;
        }

        protected virtual void SerializeItem([Diagnostics.NotNull] Item item)
        {
            foreach (var projectSerializer in ProjectSerializers)
            {
                projectSerializer.SerializeItem(ItemImporter, LanguageService, item);
            }
        }

        protected virtual void SerializeNextItem()
        {
            var queuedSerializationItem = _queue.Dequeue();
            if (queuedSerializationItem == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                var item = queuedSerializationItem.Database.GetItem(queuedSerializationItem.ItemId);
                if (item != null)
                {
                    SerializeItem(item);
                }
            }
        }
    }
}
