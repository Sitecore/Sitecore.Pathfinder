// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProjectDirectoryVisitor
    {
        [ImportingConstructor]
        public ProjectDirectoryVisitor([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;

            FileSearchPattern = configuration.GetString(Constants.Configuration.MappingFileSearchPattern, "*");
            IgnoreDirectories = Enumerable.Empty<string>();
            IgnoreFileNames = Enumerable.Empty<string>();
        }

        [NotNull]
        public string FileSearchPattern { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<string> IgnoreDirectories { get; set; }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<string> IgnoreFileNames { get; set; }

        public virtual void Visit([NotNull] ProjectOptions projectOptions, [NotNull] [ItemNotNull] ICollection<string> sourceFileNames)
        {
            if (!FileSystem.DirectoryExists(projectOptions.ProjectDirectory))
            {
                return;
            }

            Visit(projectOptions, sourceFileNames, projectOptions.ProjectDirectory);
        }

        [NotNull]
        public ProjectDirectoryVisitor With([NotNull] [ItemNotNull] IEnumerable<string> ignoreDirectories, [NotNull] [ItemNotNull] IEnumerable<string> ignoreFileNames)
        {
            IgnoreDirectories = ignoreDirectories;
            IgnoreFileNames = ignoreFileNames;
            return this;
        }

        protected virtual bool IgnoreDirectory([NotNull] string directory)
        {
            var directoryName = Path.GetFileName(directory);

            return IgnoreDirectories.Contains(directoryName, StringComparer.OrdinalIgnoreCase);
        }

        protected virtual bool IgnoreFileName([NotNull] string fileName)
        {
            var name = Path.GetFileName(fileName);

            return IgnoreFileNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        protected virtual void Visit([NotNull] ProjectOptions projectOptions, [NotNull] [ItemNotNull] ICollection<string> sourceFileNames, [NotNull] string directory)
        {
            var fileNames = FileSystem.GetFiles(directory, FileSearchPattern);
            foreach (var fileName in fileNames)
            {
                if (!IgnoreFileName(fileName))
                {
                    sourceFileNames.Add(fileName);
                }
            }

            var subdirectories = FileSystem.GetDirectories(directory);
            foreach (var subdirectory in subdirectories)
            {
                if (!IgnoreDirectory(subdirectory))
                {
                    Visit(projectOptions, sourceFileNames, subdirectory);
                }
            }
        }
    }
}
