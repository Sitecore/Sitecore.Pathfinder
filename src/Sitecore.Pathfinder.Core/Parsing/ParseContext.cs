// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    public class ParseContext : IParseContext
    {
        [FactoryConstructor(typeof(IParseContext))]
        public ParseContext([NotNull] IConfiguration configuration, [NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] PathMappingContext pathMappingContext)
        {
            Snapshot = Snapshots.Snapshot.Empty;

            Culture = configuration.GetCulture();

            Project = project;
            Snapshot = snapshot;

            FilePath = pathMappingContext.FilePath;
            ItemName = pathMappingContext.ItemName;
            ItemPath = pathMappingContext.ItemPath;
            Database = pathMappingContext.Database;
            UploadMedia = pathMappingContext.UploadMedia;
        }

        public CultureInfo Culture { get; }

        public virtual IDatabase Database { get; } = Projects.Database.Empty;

        public virtual string FilePath { get; }

        public bool IsParsed { get; set; }

        public virtual string ItemName { get; }

        public virtual string ItemPath { get; }

        public IProject Project { get; }

        public ISnapshot Snapshot { get; }

        public bool UploadMedia { get; }
    }
}
