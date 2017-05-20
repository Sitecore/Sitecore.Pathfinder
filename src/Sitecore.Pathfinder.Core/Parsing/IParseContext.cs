// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
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
        CultureInfo Culture { get; }

        [NotNull]
        Database Database { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull]
        string FilePath { get; }

        bool IsParsed { get; set; }

        [NotNull]
        string ItemName { get; }

        [NotNull]
        string ItemPath { get; }

        [NotNull]
        IPipelineService Pipelines { get; }

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
        IParseContext With([NotNull] IProject project, [NotNull] IDiagnosticCollector diagnosticColletor, [NotNull] ISnapshot snapshot, [NotNull] PathMappingContext pathMappingContext);
    }
}
