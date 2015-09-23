// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export(typeof(ISnapshotService))]
    public class SnapshotService : ISnapshotService
    {
        [ImportingConstructor]
        public SnapshotService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] ITextTokenService textTokenService)
        {
            Configuration = configuration;
            Factory = factory;
            TextTokenService = textTokenService;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        [ImportMany]
        [ItemNotNull]
        protected IEnumerable<ISnapshotLoader> Loaders { get; private set; }

        [NotNull]
        protected ITextTokenService TextTokenService { get; }

        public ISnapshot LoadSnapshot(IProject project, ISourceFile sourceFile)
        {
            foreach (var loader in Loaders.OrderBy(l => l.Priority))
            {
                if (loader.CanLoad(this, project, sourceFile))
                {
                    return loader.Load(this, project, sourceFile);
                }
            }

            return Factory.Snapshot(sourceFile);
        }

        public virtual string ReplaceTokens(IProject project, ISourceFile sourceFile, string contents)
        {
            var itemName = PathHelper.GetItemName(sourceFile);

            var fileContext = FileContext.GetFileContext(project, Configuration, sourceFile);

            var filePath = fileContext.FilePath;
            var filePathWithExtensions = PathHelper.NormalizeItemPath(PathHelper.GetDirectoryAndFileNameWithoutExtensions(filePath));
            var fileName = Path.GetFileName(filePath);
            var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(fileName);
            var directoryName = string.IsNullOrEmpty(filePath) ? string.Empty : PathHelper.NormalizeFilePath(Path.GetDirectoryName(filePath) ?? string.Empty);

            var contextTokens = new Dictionary<string, string>
            {
                ["ItemPath"] = itemName,
                ["FilePathWithoutExtensions"] = filePathWithExtensions,
                ["FilePath"] = filePath,
                ["Database"] = project.Options.DatabaseName,
                ["FileNameWithoutExtensions"] = fileNameWithoutExtensions,
                ["FileName"] = fileName,
                ["DirectoryName"] = directoryName,
            };

            return TextTokenService.Replace(contents, contextTokens);
        }
    }
}
