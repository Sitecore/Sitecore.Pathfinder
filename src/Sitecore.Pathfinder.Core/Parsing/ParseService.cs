// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
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
        public ParseService([NotNull] IConfiguration configuration, [NotNull] ISnapshotService snapshotService, [NotNull] IPathMapperService pathMapper, [NotNull] ExportFactory<IParseContext> parseContextFactory, [ImportMany, NotNull, ItemNotNull] IEnumerable<IParser> parsers)
        {
            Configuration = configuration;
            SnapshotService = snapshotService;
            PathMapper = pathMapper;
            ParseContextFactory = parseContextFactory;
            Parsers = parsers;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ExportFactory<IParseContext> ParseContextFactory { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IParser> Parsers { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual void Parse(IProject project, IDiagnosticCollector diagnosticColletor, ISourceFile sourceFile)
        {
            var pathMappingContext = new PathMappingContext(PathMapper);
            pathMappingContext.Parse(project, sourceFile);

            var parseAllFiles = Configuration.GetBool(Constants.Configuration.BuildProject.ParseAllFiles);
            if (!parseAllFiles && !pathMappingContext.IsMapped)
            {
                return;
            }

            var snapshot = SnapshotService.LoadSnapshot(project, sourceFile, pathMappingContext);

            var parseContext = ParseContextFactory.New().With(project, diagnosticColletor, snapshot, pathMappingContext);
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
                    var text = ex.Message;
                    if (Configuration.GetBool(Constants.Configuration.System.ShowStackTrace))
                    {
                        text += Environment.NewLine + ex.StackTrace;
                    }

                    parseContext.Trace.TraceError(Msg.P1013, text, sourceFile);
                }
            }

            if (!parseAllFiles && !parsed)
            {
                parseContext.Trace.TraceWarning(Msg.P1024, Texts.No_parser_found_for_file__If_the_file_is_a_content_file__add_the_file_extension_to_the__project_website_mappings_content_files__setting, sourceFile);
            }
        }
    }
}
