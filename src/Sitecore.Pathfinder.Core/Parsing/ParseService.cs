// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(IParseService))]
    public class ParseService : IParseService
    {
        [ImportingConstructor]
        public ParseService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] ISnapshotService snapshotService, [NotNull] IPathMapperService pathMapper, [ImportMany, NotNull, ItemNotNull] IEnumerable<IParser> parsers)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Factory = factory;
            SnapshotService = snapshotService;
            PathMapper = pathMapper;
            Parsers = parsers;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IParser> Parsers { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual void Parse(IProject project, ISourceFile sourceFile)
        {
            var pathMappingContext = new PathMappingContext(PathMapper);
            pathMappingContext.Parse(project, sourceFile);

            if (!pathMappingContext.IsMapped)
            {
                return;
            }

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
                ["ProjectDirectory"] = project.Options.ProjectDirectory
            };

            tokens.AddRange(project.Options.Tokens);

            var snapshotParseContext = new SnapshotParseContext(SnapshotService, tokens, new Dictionary<string, List<ITextNode>>());
            var snapshot = SnapshotService.LoadSnapshot(snapshotParseContext, sourceFile);

            var parseContext = Factory.ParseContext(project, snapshot, pathMappingContext);

            var parsed = false;
            foreach (var parser in Parsers.OrderBy(p => p.Priority))
            {
                try
                {
                    if (parser.CanParse(parseContext))
                    {
                        parser.Parse(parseContext);
                        parsed = true;
                    }
                }
                catch (Exception ex)
                {
                    parseContext.Trace.TraceError(Msg.P1013, ex.Message, sourceFile);
                }
            }

            if (!parsed)
            {
                parseContext.Trace.TraceWarning(Msg.P1024, "No parser found for file. If the file is a content file, add the file extension to the 'project-website-mappings:content-files' setting", sourceFile);
            }
        }

        [NotNull]
        protected IPathMapperService PathMapper { get; }
    }
}
