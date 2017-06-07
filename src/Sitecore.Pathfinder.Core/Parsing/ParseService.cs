// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
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
        public ParseService([NotNull] IConfiguration configuration, [NotNull] ITraceService trace, [NotNull] IFactory factory, [NotNull] ISnapshotService snapshotService, [NotNull] IPathMapperService pathMapper, [ImportMany, NotNull, ItemNotNull] IEnumerable<IParser> parsers)
        {
            Configuration = configuration;
            Trace = trace;
            Factory = factory;
            SnapshotService = snapshotService;
            PathMapper = pathMapper;
            Parsers = parsers;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IParser> Parsers { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual void Parse(IProject project, ISourceFile sourceFile)
        {
            var pathMappingContext = Factory.PathMappingContext(PathMapper);
            pathMappingContext.Parse(project, sourceFile);

            var snapshot = SnapshotService.LoadSnapshot(project, sourceFile, pathMappingContext);

            var parseContext = Factory.ParseContext(project, snapshot, pathMappingContext);
            foreach (var parser in Parsers.OrderBy(p => p.Priority))
            {
                try
                {
                    if (!parser.CanParse(parseContext))
                    {
                        continue;
                    }

                    parser.Parse(parseContext);
                    parseContext.IsParsed = true;
                }
                catch (Exception ex)
                {
                    var text = ex.Message;
                    if (Configuration.GetBool(Constants.Configuration.System.ShowStackTrace))
                    {
                        text += Environment.NewLine + ex.StackTrace;
                    }

                    Trace.TraceError(Msg.P1013, text, sourceFile);
                }
            }

            if (!parseContext.IsParsed)
            {
                Trace.TraceWarning(Msg.P1024, Texts.No_parser_found_for_file__If_the_file_is_a_content_file__add_the_file_extension_to_the__project_website_mappings_content_files__setting, sourceFile);
            }
        }
    }
}
