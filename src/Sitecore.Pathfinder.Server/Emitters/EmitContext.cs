// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Data.Items;
using Sitecore.Data.Serialization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters
{
    [Export(typeof(IEmitContext))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EmitContext : IEmitContext
    {
        [ImportingConstructor]
        public EmitContext([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] ITraceService traceService, [Diagnostics.NotNull] IFileSystemService fileSystemService)
        {
            Configuration = configuration;
            Trace = traceService;
            FileSystem = fileSystemService;

            ForceUpdate = Configuration.GetBool(Constants.Configuration.ForceUpdate, true);

            var projectDirectory = Configuration.GetString(Constants.Configuration.ProjectDirectory);
            UninstallDirectory = PathHelper.Combine(projectDirectory, Configuration.GetString(Constants.Configuration.UninstallDirectory, "..\\.uninstall"));
        }

        public ICollection<string> AddedFiles { get; } = new List<string>();

        public ICollection<string> AddedItems { get; } = new List<string>();

        public IConfiguration Configuration { get; }

        public ICollection<string> DeletedFiles { get; } = new List<string>();

        public ICollection<string> DeletedItems { get; } = new List<string>();

        public IFileSystemService FileSystem { get; }

        public IProject Project { get; private set; }

        public ITraceService Trace { get; }

        public string UninstallDirectory { get; }

        public ICollection<string> UpdatedFiles { get; } = new List<string>();

        public ICollection<string> UpdatedItems { get; } = new List<string>();

        public bool ForceUpdate { get; }

        public virtual void RegisterAddedFile(Projects.Files.File projectItem, string destinationFileName)
        {
            AddedFiles.Add(projectItem.FilePath);
        }

        public void RegisterAddedItem(Item newItem)
        {
            DeletedItems.Add(newItem.Database.Name + "|" + newItem.ID);
        }

        public virtual void RegisterDeletedFile(Projects.Files.File projectItem, string destinationFileName)
        {
            // BackupFile(projectItem, destinationFileName);
            DeletedFiles.Add(projectItem.FilePath);
        }

        public void RegisterDeletedItem(Item deletedItem)
        {
            // BackupItem(deletedItem);
            DeletedItems.Add(deletedItem.Database.Name + "|" + deletedItem.ID);
        }

        public virtual void RegisterUpdatedFile(Projects.Files.File projectItem, string destinationFileName)
        {
            // BackupFile(projectItem, destinationFileName);
            UpdatedFiles.Add(projectItem.FilePath);
        }

        public virtual void RegisterUpdatedItem(Item item)
        {
            // BackupItem(item);
            UpdatedItems.Add(item.Database.Name + "|" + item.ID);
        }

        public IEmitContext With(IProject project)
        {
            Project = project;

            return this;
        }

        protected virtual void BackupFile([Diagnostics.NotNull] Projects.Files.File projectItem, [Diagnostics.NotNull] string destinationFileName)
        {
            if (!FileSystem.FileExists(destinationFileName))
            {
                return;
            }

            var filePath = PathHelper.NormalizeFilePath(projectItem.FilePath).TrimStart('\\');

            var uninstallFileName = Path.Combine(UninstallDirectory, "content");
            uninstallFileName = Path.Combine(uninstallFileName, filePath);

            FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
            FileSystem.Copy(destinationFileName, uninstallFileName);
        }

        protected virtual void BackupItem([Diagnostics.NotNull] Item item)
        {
            var uninstallFileName = Path.Combine(UninstallDirectory, "content\\serialization");
            uninstallFileName = Path.Combine(uninstallFileName, item.Database.Name);
            uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString().Left(1));
            uninstallFileName = Path.Combine(uninstallFileName, item.ID.ToShortID().ToString());

            FileSystem.CreateDirectory(Path.GetDirectoryName(uninstallFileName) ?? string.Empty);
            Manager.DumpItem(uninstallFileName, item);
        }
    }
}
