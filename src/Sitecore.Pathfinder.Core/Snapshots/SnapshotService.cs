// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots.Directives;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export(typeof(ISnapshotService))]
    public class SnapshotService : ISnapshotService
    {
        [ImportingConstructor]
        public SnapshotService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull]   IEnumerable<ISnapshotLoader> loaders, [ImportMany, NotNull, ItemNotNull]   IEnumerable<ISnapshotDirective> directives)
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

            string sourceFileName;
            if (includeFileName.StartsWith("~/"))
            {
                sourceFileName = PathHelper.Combine(projectDirectory, includeFileName.Mid(2));
            }
            else
            {
                sourceFileName = PathHelper.Combine(Path.GetDirectoryName(snapshot.SourceFile.AbsoluteFileName) ?? string.Empty, includeFileName);
            }

            if (!FileSystem.FileExists(sourceFileName))
            {
                throw new FileNotFoundException("Include file not found", sourceFileName);
            }

            var projectFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(projectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(sourceFileName))).TrimStart('/');
            var sourceFile = Factory.SourceFile(FileSystem, sourceFileName, projectFileName);

            var includeSnapshot = LoadSnapshot(snapshotParseContext, sourceFile) as TextSnapshot;
            Assert.Cast(includeSnapshot, nameof(includeSnapshot));

            return includeSnapshot.Root;
        }

        public ISnapshot LoadSnapshot(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
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
    }
}
