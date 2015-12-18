// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.IO.PathMappers.Pipelines
{
    public class ParsePathMappers : PipelineProcessorBase<ParsePathMappersPipeline>
    {
        public ParsePathMappers() : base(1000)
        {
        }

        protected virtual void AddProjectToWebsiteMappings([NotNull] ParsePathMappersPipeline pipeline)
        {
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
                    var databaseName = pipeline.Configuration.GetString(key + ":database", "master");
                    var include = pipeline.Configuration.GetString(key + ":file-name-include");
                    var exclude = pipeline.Configuration.GetString(key + ":file-name-exclude");
                    var isImport = pipeline.Configuration.GetBool(key + ":is-import");
                    var uploadMedia = pipeline.Configuration.GetBool(key + ":upload-media", true);

                    pipeline.ProjectDirectoryToWebsiteItemPaths.Add(new ProjectDirectoryToWebsiteItemPathMapper(projectDirectory, databaseName, itemPath, include, exclude, isImport, uploadMedia));
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

                    pipeline.ProjectDirectoryToWebsiteDirectories.Add(new ProjectDirectoryToWebsiteDirectoryMapper(projectDirectory, websiteDirectory, include, exclude));
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

                    var databaseName = pipeline.Configuration.GetString(key + ":database", "master");
                    var itemPath = itemPathToProjectDirectory.Left(n).Trim();
                    var projectDirectory = itemPathToProjectDirectory.Mid(n + 2).Trim();
                    var format = pipeline.Configuration.GetString(key + ":format", "item.json");
                    var itemNameInclude = pipeline.Configuration.GetString(key + ":item-name-include");
                    var itemNameExclude = pipeline.Configuration.GetString(key + ":item-name-exclude");
                    var templateNameInclude = pipeline.Configuration.GetString(key + ":template-name-include");
                    var templateNameExclude = pipeline.Configuration.GetString(key + ":template-name-exclude");

                    pipeline.WebsiteItemPathToProjectDirectories.Add(new WebsiteItemPathToProjectDirectoryMapper(databaseName, itemPath, projectDirectory, format, itemNameInclude, itemNameExclude, templateNameInclude, templateNameExclude));
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

                    pipeline.WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper(websiteDirectory, projectDirectory, include, exclude));
                }
            }
        }

        protected override void Process(ParsePathMappersPipeline pipeline)
        {
            AddProjectToWebsiteMappings(pipeline);
            AddWebsiteToProjectMappings(pipeline);
        }
    }
}
