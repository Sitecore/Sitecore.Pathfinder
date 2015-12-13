// © 2015 Sitecore Corporation A/S. All rights reserved.

using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public interface IParseContext
    {
        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string DatabaseName { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull]
        string FilePath { get; }

        [NotNull]
        string ItemName { get; }

        [NotNull]
        string ItemPath { get; }

        [NotNull]
        IPipelineService PipelineService { get; }

        [NotNull]
        IProject Project { get; }

        [NotNull]
        IReferenceParserService ReferenceParser { get; }

        [NotNull]
        ISnapshot Snapshot { get; }

        [NotNull]
        ITraceService Trace { get; }

        bool UploadMedia { get; }

        [NotNull]
        ISchemaService SchemaService { get; }

        [NotNull]
        IParseContext With([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] PathMappingContext pathMappingContext);
    }
}
