// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO.PathMappers;

namespace Sitecore.Pathfinder.IO
{
    [InheritedExport(typeof(IPathMapperService))]
    public class PathMapperService : IPathMapperService
    {
        public PathMapperService()
        {
        }

        [ImportingConstructor]
        public PathMapperService([NotNull] IConfiguration configuration)
        {
            LoadFromConfiguration(configuration);
        }

        public ICollection<ProjectDirectoryToWebsiteDirectoryMapper> ProjectDirectoryToWebsiteDirectories { get; } = new List<ProjectDirectoryToWebsiteDirectoryMapper>();

        public ICollection<ProjectDirectoryToWebsiteItemPathMapper> ProjectDirectoryToWebsiteItemPaths { get; } = new List<ProjectDirectoryToWebsiteItemPathMapper>();

        public ICollection<ProjectFileNameToWebsiteFileNameMapper> ProjectFileNameToWebsiteFileNames { get; } = new List<ProjectFileNameToWebsiteFileNameMapper>();

        public ICollection<WebsiteDirectoryToProjectDirectoryMapper> WebsiteDirectoryToProjectDirectories { get; } = new List<WebsiteDirectoryToProjectDirectoryMapper>();

        public ICollection<WebsiteItemPathToProjectDirectoryMapper> WebsiteItemPathToProjectDirectories { get; } = new List<WebsiteItemPathToProjectDirectoryMapper>();

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

            foreach (var pair in configuration.GetSubKeys("project-website-mappings:project-to-website"))
            {
                var key = "project-website-mappings:project-to-website:" + pair.Key;

                var projectDirectoryToItemPath = configuration.GetString(key + ":project-directory-to-item-path");
                if (!string.IsNullOrEmpty(projectDirectoryToItemPath))
                {
                    var n = projectDirectoryToItemPath.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var projectDirectory = projectDirectoryToItemPath.Left(n).Trim();
                    var itemPath = projectDirectoryToItemPath.Mid(n + 2).Trim();
                    var databaseName = configuration.GetString(key + ":database", "master");
                    var include = configuration.GetString(key + ":file-name-include");
                    var exclude = configuration.GetString(key + ":file-name-exclude");
                    var isImport = configuration.GetBool(key + ":is-import");
                    var uploadMedia = configuration.GetBool(key + ":upload-media", true);

                    ProjectDirectoryToWebsiteItemPaths.Add(new ProjectDirectoryToWebsiteItemPathMapper(projectDirectory, databaseName, itemPath, include, exclude, isImport, uploadMedia));
                }

                var projectDirectoryToWebsiteDirectory = configuration.GetString(key + ":project-directory-to-website-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var projectDirectory = projectDirectoryToWebsiteDirectory.Left(n).Trim();
                    var websiteDirectory = projectDirectoryToWebsiteDirectory.Mid(n + 2).Trim();
                    var include = configuration.GetString(key + ":file-name-include");
                    var exclude = configuration.GetString(key + ":file-name-exclude");

                    ProjectDirectoryToWebsiteDirectories.Add(new ProjectDirectoryToWebsiteDirectoryMapper(projectDirectory, websiteDirectory, include, exclude));
                }

                foreach (var fileNamePair in configuration.GetSubKeys(key))
                {
                    if (!fileNamePair.Key.StartsWith("project-file-name-to-website-file-name", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var projectFileNameToWebsiteFileName = configuration.GetString(key + ":" + fileNamePair.Key);
                    var n = projectFileNameToWebsiteFileName.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var sourceFileName = projectFileNameToWebsiteFileName.Left(n).Trim();
                    var targetFileName = projectFileNameToWebsiteFileName.Mid(n + 2).Trim();

                    ProjectFileNameToWebsiteFileNames.Add(new ProjectFileNameToWebsiteFileNameMapper(sourceFileName, targetFileName));
                }
            }

            foreach (var pair in configuration.GetSubKeys("project-website-mappings:website-to-project"))
            {
                var key = "project-website-mappings:website-to-project:" + pair.Key;

                var itemPathToProjectDirectory = configuration.GetString(key + ":item-path-to-project-directory");
                if (!string.IsNullOrEmpty(itemPathToProjectDirectory))
                {
                    var n = itemPathToProjectDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var databaseName = configuration.GetString(key + ":database", "master");
                    var itemPath = itemPathToProjectDirectory.Left(n).Trim();
                    var projectDirectory = itemPathToProjectDirectory.Mid(n + 2).Trim();
                    var format = configuration.GetString(key + ":format", "item.json");
                    var itemNameInclude = configuration.GetString(key + ":item-name-include");
                    var itemNameExclude = configuration.GetString(key + ":item-name-exclude");
                    var templateNameInclude = configuration.GetString(key + ":template-name-include");
                    var templateNameExclude = configuration.GetString(key + ":template-name-exclude");

                    WebsiteItemPathToProjectDirectories.Add(new WebsiteItemPathToProjectDirectoryMapper(databaseName, itemPath, projectDirectory, format, itemNameInclude, itemNameExclude, templateNameInclude, templateNameExclude));
                }

                var projectDirectoryToWebsiteDirectory = configuration.GetString(key + ":website-directory-to-project-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var websiteDirectory = projectDirectoryToWebsiteDirectory.Left(n).Trim();
                    var projectDirectory = projectDirectoryToWebsiteDirectory.Mid(n + 2).Trim();
                    var include = configuration.GetString(key + ":file-name-include");
                    var exclude = configuration.GetString(key + ":file-name-exclude");

                    WebsiteDirectoryToProjectDirectories.Add(new WebsiteDirectoryToProjectDirectoryMapper(websiteDirectory, projectDirectory, include, exclude));
                }
            }
        }
    }
}
