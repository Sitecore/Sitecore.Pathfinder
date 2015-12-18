// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.IO.PathMappers.Pipelines
{
    public class ParsePathMappersPipeline : PipelineBase<ParsePathMappersPipeline>
    {
        [NotNull]
        public IConfiguration Configuration { get; private set; }

        [NotNull, ItemNotNull]
        public ICollection<IProjectToWebsiteFileNameMapper> ProjectDirectoryToWebsiteDirectories { get; } = new List<IProjectToWebsiteFileNameMapper>();

        [NotNull, ItemNotNull]
        public ICollection<IProjectFileNameToItemPathMapper> ProjectDirectoryToWebsiteItemPaths { get; } = new List<IProjectFileNameToItemPathMapper>();

        [NotNull, ItemNotNull]
        public ICollection<IProjectToWebsiteFileNameMapper> ProjectFileNameToWebsiteFileNames { get; } = new List<IProjectToWebsiteFileNameMapper>();

        [NotNull, ItemNotNull]
        public ICollection<IWebsiteToProjectFileNameMapper> WebsiteDirectoryToProjectDirectories { get; } = new List<IWebsiteToProjectFileNameMapper>();

        [NotNull, ItemNotNull]
        public ICollection<IItemPathToProjectFileNameMapper> WebsiteItemPathToProjectDirectories { get; } = new List<IItemPathToProjectFileNameMapper>();

        [NotNull]
        public ParsePathMappersPipeline Execute([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;

            Execute();

            return this;
        }
    }
}
