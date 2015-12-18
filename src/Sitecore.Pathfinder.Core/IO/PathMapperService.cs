// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO.PathMappers;
using Sitecore.Pathfinder.IO.PathMappers.Pipelines;

namespace Sitecore.Pathfinder.IO
{
    [InheritedExport(typeof(IPathMapperService))]
    public class PathMapperService : IPathMapperService
    {
        public PathMapperService()
        {
        }

        [ImportingConstructor]
        public PathMapperService([NotNull] IConfiguration configuration, [NotNull] IPipelineService pipelines)
        {
            Pipelines = pipelines;
            LoadFromConfiguration(configuration);
        }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        public ICollection<IProjectToWebsiteFileNameMapper> ProjectDirectoryToWebsiteDirectories { get; } = new List<IProjectToWebsiteFileNameMapper>();

        public ICollection<IProjectFileNameToItemPathMapper> ProjectDirectoryToWebsiteItemPaths { get; } = new List<IProjectFileNameToItemPathMapper>();

        public ICollection<IProjectToWebsiteFileNameMapper> ProjectFileNameToWebsiteFileNames { get; } = new List<IProjectToWebsiteFileNameMapper>();

        public ICollection<IWebsiteToProjectFileNameMapper> WebsiteDirectoryToProjectDirectories { get; } = new List<IWebsiteToProjectFileNameMapper>();

        public ICollection<IItemPathToProjectFileNameMapper> WebsiteItemPathToProjectDirectories { get; } = new List<IItemPathToProjectFileNameMapper>();

        public void Clear()
        {
            ProjectDirectoryToWebsiteDirectories.Clear();
            ProjectFileNameToWebsiteFileNames.Clear();
            ProjectDirectoryToWebsiteItemPaths.Clear();
            WebsiteDirectoryToProjectDirectories.Clear();
            WebsiteItemPathToProjectDirectories.Clear();
        }

        public virtual bool TryGetProjectFileName(string itemPath, string templateName, out string projectFileName, out string format)
        {
            projectFileName = string.Empty;
            format = string.Empty;

            itemPath = '/' + PathHelper.NormalizeItemPath(itemPath).TrimStart('/');

            foreach (var mapper in WebsiteItemPathToProjectDirectories)
            {
                if (mapper.TryGetProjectFileName(itemPath, templateName, out projectFileName, out format))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryGetProjectFileName(string websiteFileName, out string projectFileName)
        {
            projectFileName = string.Empty;

            websiteFileName = '\\' + PathHelper.NormalizeFilePath(websiteFileName).TrimStart('\\');

            foreach (var mapper in WebsiteDirectoryToProjectDirectories)
            {
                if (mapper.TryGetProjectFileName(websiteFileName, out projectFileName))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryGetWebsiteFileName(string projectFileName, out string websiteFileName)
        {
            websiteFileName = string.Empty;

            projectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).TrimStart('\\');

            foreach (var mapper in ProjectFileNameToWebsiteFileNames)
            {
                if (mapper.TryGetWebsiteFileName(projectFileName, out websiteFileName))
                {
                    return true;
                }
            }

            foreach (var mapper in ProjectDirectoryToWebsiteDirectories)
            {
                if (mapper.TryGetWebsiteFileName(projectFileName, out websiteFileName))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryGetWebsiteItemPath(string projectFileName, out string databaseName, out string itemPath, out bool isImport, out bool uploadMedia)
        {
            itemPath = string.Empty;
            databaseName = string.Empty;
            isImport = false;
            uploadMedia = true;

            projectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).TrimStart('\\');

            foreach (var mapper in ProjectDirectoryToWebsiteItemPaths)
            {
                if (mapper.TryGetWebsiteItemPath(projectFileName, out databaseName, out itemPath, out isImport, out uploadMedia))
                {
                    return true;
                }
            }

            return false;
        }

        protected void LoadFromConfiguration([NotNull] IConfiguration configuration)
        {
            Clear();

            var pipeline = Pipelines.Resolve<ParsePathMappersPipeline>().Execute(configuration);

            ProjectDirectoryToWebsiteDirectories.AddRange(pipeline.ProjectDirectoryToWebsiteDirectories);
            ProjectDirectoryToWebsiteItemPaths.AddRange(pipeline.ProjectDirectoryToWebsiteItemPaths);
            ProjectFileNameToWebsiteFileNames.AddRange(pipeline.ProjectFileNameToWebsiteFileNames);
            WebsiteDirectoryToProjectDirectories.AddRange(pipeline.WebsiteDirectoryToProjectDirectories);
            WebsiteItemPathToProjectDirectories.AddRange(pipeline.WebsiteItemPathToProjectDirectories);
        }
    }
}
