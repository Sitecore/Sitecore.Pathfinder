// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export(typeof(ISnapshotService))]
    public class SnapshotService : ISnapshotService
    {
        [ImportingConstructor]
        public SnapshotService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull] IEnumerable<ISnapshotLoader> loaders)
        {
            Configuration = configuration;
            Factory = factory;
            FileSystem = fileSystem;
            Loaders = loaders;
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<ISnapshotLoader> Loaders { get; }

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
