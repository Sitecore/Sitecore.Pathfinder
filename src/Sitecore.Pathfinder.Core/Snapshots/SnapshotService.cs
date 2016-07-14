// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots.Directives;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export(typeof(ISnapshotService))]
    public class SnapshotService : ISnapshotService
    {
        [ImportingConstructor]
        public SnapshotService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull] IEnumerable<ISnapshotLoader> loaders, [ImportMany, NotNull, ItemNotNull] IEnumerable<ISnapshotDirective> directives)
        {
            Configuration = configuration;
            Factory = factory;
            FileSystem = fileSystem;
            Loaders = loaders;
            Directives = directives;
        }

        public IEnumerable<ISnapshotDirective> Directives { get; }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<ISnapshotLoader> Loaders { get; }

        public virtual ITextNode LoadIncludeFile(SnapshotParseContext snapshotParseContext, ISnapshot snapshot, string includeFileName)
        {
            var extension = PathHelper.GetExtension(snapshot.SourceFile.AbsoluteFileName);
            var projectDirectory = snapshot.SourceFile.AbsoluteFileName.Left(snapshot.SourceFile.AbsoluteFileName.Length - snapshot.SourceFile.ProjectFileName.Length - extension.Length + 1);

            string absoluteFileName;
            if (includeFileName.StartsWith("~/"))
            {
                absoluteFileName = PathHelper.Combine(projectDirectory, includeFileName.Mid(2));
            }
            else
            {
                absoluteFileName = PathHelper.Combine(Path.GetDirectoryName(snapshot.SourceFile.AbsoluteFileName) ?? string.Empty, includeFileName);
            }

            if (!FileSystem.FileExists(absoluteFileName))
            {
                throw new FileNotFoundException("Include file not found", absoluteFileName);
            }

            var sourceFile = Factory.SourceFile(FileSystem, absoluteFileName);

            var includeSnapshot = LoadSnapshot(snapshotParseContext, sourceFile) as TextSnapshot;
            Assert.Cast(includeSnapshot, nameof(includeSnapshot));

            return includeSnapshot.Root;
        }

        public virtual ISnapshot LoadSnapshot(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            foreach (var loader in Loaders.OrderBy(l => l.Priority))
            {
                if (loader.CanLoad(sourceFile))
                {
                    return loader.Load(snapshotParseContext, sourceFile);
                }
            }

            return Factory.Snapshot(sourceFile);
        }

        public virtual ISnapshot LoadSnapshot(IProjectBase project, ISourceFile sourceFile, PathMappingContext pathMappingContext)
        {
            var itemName = PathHelper.GetItemName(sourceFile);
            var filePath = pathMappingContext.FilePath;
            if (filePath.StartsWith("~/"))
            {
                filePath = filePath.Mid(1);
            }

            var filePathWithExtensions = PathHelper.NormalizeItemPath(PathHelper.GetDirectoryAndFileNameWithoutExtensions(filePath));
            var fileName = Path.GetFileName(filePath);
            var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(fileName);
            var directoryName = string.IsNullOrEmpty(filePath) ? string.Empty : PathHelper.NormalizeItemPath(Path.GetDirectoryName(filePath) ?? string.Empty);

            var tokens = new Dictionary<string, string>
            {
                ["ItemPath"] = itemName,
                ["FilePathWithoutExtensions"] = filePathWithExtensions,
                ["FilePath"] = filePath,
                ["Database"] = project.Options.DatabaseName,
                ["FileNameWithoutExtensions"] = fileNameWithoutExtensions,
                ["FileName"] = fileName,
                ["DirectoryName"] = directoryName,
                ["ProjectDirectory"] = project.ProjectDirectory
            };

            tokens.AddRange(project.Options.Tokens);

            var snapshotParseContext = new SnapshotParseContext(this, project, tokens, new Dictionary<string, List<ITextNode>>());
            var snapshot = LoadSnapshot(snapshotParseContext, sourceFile);

            return snapshot;
        }
    }
}
