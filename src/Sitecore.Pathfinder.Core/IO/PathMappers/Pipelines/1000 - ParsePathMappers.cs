// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.IO.PathMappers.Pipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class ParsePathMappers : PipelineProcessorBase<ParsePathMappersPipeline>
    {
        [ImportingConstructor]
        public ParsePathMappers([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected virtual void AddProjectToWebsiteMappings([NotNull] ParsePathMappersPipeline pipeline)
        {
            var defaultDatabase = pipeline.Configuration.GetString(Constants.Configuration.Database, "master");

            foreach (var pair in pipeline.Configuration.GetSubKeys("project-website-mappings:project-to-website"))
            {
                var key = "project-website-mappings:project-to-website:" + pair.Key;

                var projectDirectoryToItemPath = pipeline.Configuration.GetString(key + ":project-directory-to-item-path");
                if (!string.IsNullOrEmpty(projectDirectoryToItemPath))
                {
                    var n = projectDirectoryToItemPath.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var projectDirectory = projectDirectoryToItemPath.Left(n).Trim();
                    var itemPath = projectDirectoryToItemPath.Mid(n + 2).Trim();
                    var databaseName = pipeline.Configuration.GetString(key + ":database", defaultDatabase);
                    var include = pipeline.Configuration.GetString(key + ":file-name-include");
                    var exclude = pipeline.Configuration.GetString(key + ":file-name-exclude");
                    var isImport = pipeline.Configuration.GetBool(key + ":is-import");
                    var uploadMedia = pipeline.Configuration.GetBool(key + ":upload-media", true);

                    var pathMatcher = GetFileNamePathMatcher(projectDirectory, include, exclude);

                    var projectDirectoryToWebsiteItemPathMapper = new ProjectDirectoryToWebsiteItemPathMapper(pathMatcher, projectDirectory, databaseName, itemPath, isImport, uploadMedia);
                    pipeline.ProjectDirectoryToWebsiteItemPaths.Add(projectDirectoryToWebsiteItemPathMapper);
                }

                var projectDirectoryToWebsiteDirectory = pipeline.Configuration.GetString(key + ":project-directory-to-website-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var projectDirectory = projectDirectoryToWebsiteDirectory.Left(n).Trim();
                    var websiteDirectory = projectDirectoryToWebsiteDirectory.Mid(n + 2).Trim();
                    var include = pipeline.Configuration.GetString(key + ":file-name-include");
                    var exclude = pipeline.Configuration.GetString(key + ":file-name-exclude");

                    var pathMatcher = GetFileNamePathMatcher(projectDirectory, include, exclude);

                    var projectDirectoryToWebsiteDirectoryMapper = new ProjectDirectoryToWebsiteDirectoryMapper(pathMatcher, projectDirectory, websiteDirectory);
                    pipeline.ProjectDirectoryToWebsiteDirectories.Add(projectDirectoryToWebsiteDirectoryMapper);
                }

                foreach (var fileNamePair in pipeline.Configuration.GetSubKeys(key))
                {
                    if (!fileNamePair.Key.StartsWith("project-file-name-to-website-file-name", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var projectFileNameToWebsiteFileName = pipeline.Configuration.GetString(key + ":" + fileNamePair.Key);
                    var n = projectFileNameToWebsiteFileName.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var sourceFileName = projectFileNameToWebsiteFileName.Left(n).Trim();
                    var targetFileName = projectFileNameToWebsiteFileName.Mid(n + 2).Trim();

                    pipeline.ProjectFileNameToWebsiteFileNames.Add(new ProjectFileNameToWebsiteFileNameMapper(sourceFileName, targetFileName));
                }
            }
        }

        protected virtual void AddWebsiteToProjectMappings([NotNull] ParsePathMappersPipeline pipeline)
        {
            var defaultDatabase = pipeline.Configuration.GetString(Constants.Configuration.Database, "master");

            foreach (var pair in pipeline.Configuration.GetSubKeys("project-website-mappings:website-to-project"))
            {
                var key = "project-website-mappings:website-to-project:" + pair.Key;

                var itemPathToProjectDirectory = pipeline.Configuration.GetString(key + ":item-path-to-project-directory");
                if (!string.IsNullOrEmpty(itemPathToProjectDirectory))
                {
                    var n = itemPathToProjectDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var databaseName = pipeline.Configuration.GetString(key + ":database", defaultDatabase);
                    var itemPath = itemPathToProjectDirectory.Left(n).Trim();
                    var projectDirectory = itemPathToProjectDirectory.Mid(n + 2).Trim();
                    var format = pipeline.Configuration.GetString(key + ":format", "item.json");
                    var itemNameInclude = pipeline.Configuration.GetString(key + ":item-name-include");
                    var itemNameExclude = pipeline.Configuration.GetString(key + ":item-name-exclude");
                    var templateNameInclude = pipeline.Configuration.GetString(key + ":template-name-include");
                    var templateNameExclude = pipeline.Configuration.GetString(key + ":template-name-exclude");

                    var itemNamePathMatcher = GetItemPathPathMatcher(itemPath, itemNameInclude, itemNameExclude); 
                    var templateNamePathMatcher = GetItemPathPathMatcher(itemPath, templateNameInclude, templateNameExclude); 

                    var websiteItemPathToProjectDirectoryMapper = new WebsiteItemPathToProjectDirectoryMapper(itemNamePathMatcher, templateNamePathMatcher, databaseName, itemPath, projectDirectory, format);
                    pipeline.WebsiteItemPathToProjectDirectories.Add(websiteItemPathToProjectDirectoryMapper);
                }

                var projectDirectoryToWebsiteDirectory = pipeline.Configuration.GetString(key + ":website-directory-to-project-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var websiteDirectory = projectDirectoryToWebsiteDirectory.Left(n).Trim();
                    var projectDirectory = projectDirectoryToWebsiteDirectory.Mid(n + 2).Trim();
                    var include = pipeline.Configuration.GetString(key + ":file-name-include");
                    var exclude = pipeline.Configuration.GetString(key + ":file-name-exclude");

                    var pathMatcher = GetFileNamePathMatcher(websiteDirectory, include, exclude);

                    var websiteDirectoryToProjectDirectoryMapper = new WebsiteDirectoryToProjectDirectoryMapper(pathMatcher, websiteDirectory, projectDirectory);
                    pipeline.WebsiteDirectoryToProjectDirectories.Add(websiteDirectoryToProjectDirectoryMapper);
                }
            }
        }

        [CanBeNull]
        protected virtual IPathMatcher GetItemPathPathMatcher([NotNull] string itemPath, [NotNull] string itemNameInclude, [NotNull] string itemNameExclude)
        {
            if (string.IsNullOrEmpty(itemNameInclude) && string.IsNullOrEmpty(itemNameExclude))
            {
                return null;
            }
             
            itemPath = '/' + PathHelper.NormalizeItemPath(itemPath).Trim('/');

            if (!string.IsNullOrEmpty(itemNameInclude))
            {
                itemNameInclude = itemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(itemNameInclude).Trim('/');
            }

            if (!string.IsNullOrEmpty(itemNameExclude))
            {
                itemNameExclude = itemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(itemNameExclude).Trim('/');
            }

            return Factory.PathMatcher(itemNameInclude, itemNameExclude);
        }

        [CanBeNull]
        protected virtual IPathMatcher GetFileNamePathMatcher([NotNull] string directory, [NotNull] string include, [NotNull] string exclude)
        {
            if (string.IsNullOrEmpty(include) && string.IsNullOrEmpty(exclude))
            {
                return null;
            }

            directory = '\\' + PathHelper.NormalizeFilePath(directory).Trim('\\');

            if (!string.IsNullOrEmpty(include))
            {
                include = directory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(include).Trim('\\');
            }

            if (!string.IsNullOrEmpty(exclude))
            {                          
                exclude = directory.TrimEnd('\\') + '\\' + PathHelper.NormalizeFilePath(exclude).Trim('\\');
            }

            return Factory.PathMatcher(include, exclude);

        }

        protected override void Process(ParsePathMappersPipeline pipeline)
        {
            AddProjectToWebsiteMappings(pipeline);
            AddWebsiteToProjectMappings(pipeline);
        }
    }
}
