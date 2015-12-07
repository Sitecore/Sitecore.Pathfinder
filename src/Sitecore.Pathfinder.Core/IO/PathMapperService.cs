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

        [NotNull, ItemNotNull]
        public ICollection<ProjectFileNameToWebsiteFileNameMapper> ProjectFileNameToWebsiteFileNames { get; } = new List<ProjectFileNameToWebsiteFileNameMapper>();

        [NotNull, ItemNotNull]
        public ICollection<ProjectFileNameToWebsiteItemPathMapper> ProjectFileNameToWebsiteItemPaths { get; } = new List<ProjectFileNameToWebsiteItemPathMapper>();

        [NotNull, ItemNotNull]
        public ICollection<WebsiteFileNameToProjectFileNameMapper> WebsiteFileNameToProjectFileNames { get; } = new List<WebsiteFileNameToProjectFileNameMapper>();

        [NotNull, ItemNotNull]
        public ICollection<WebsiteItemPathToProjectFileNameMapper> WebsiteItemPathToProjectFileNames { get; } = new List<WebsiteItemPathToProjectFileNameMapper>();

        public void Clear()
        {
            ProjectFileNameToWebsiteFileNames.Clear();
            ProjectFileNameToWebsiteItemPaths.Clear();
            WebsiteFileNameToProjectFileNames.Clear();
            WebsiteItemPathToProjectFileNames.Clear();
        }

        public virtual bool TryGetProjectFileName(string itemPath, string templateName, out string projectFileName, out string format)
        {
            projectFileName = null;
            format = null;

            itemPath = '/' + PathHelper.NormalizeItemPath(itemPath).TrimStart('/');

            foreach (var mapper in WebsiteItemPathToProjectFileNames)
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
            projectFileName = null;

            websiteFileName = '\\' + PathHelper.NormalizeFilePath(websiteFileName).TrimStart('\\');

            foreach (var mapper in WebsiteFileNameToProjectFileNames)
            {
                if (mapper.TryGetWebsiteFileName(websiteFileName, out projectFileName))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryGetWebsiteFileName(string projectFileName, out string websiteFileName)
        {
            websiteFileName = null;

            projectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).TrimStart('\\');

            foreach (var mapper in ProjectFileNameToWebsiteFileNames)
            {
                if (mapper.TryGetWebsiteFileName(projectFileName, out websiteFileName))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryGetWebsiteItemPath(string projectFileName, out string itemPath)
        {
            itemPath = null;

            projectFileName = '\\' + PathHelper.NormalizeFilePath(projectFileName).TrimStart('\\');

            foreach (var mapper in ProjectFileNameToWebsiteItemPaths)
            {
                if (mapper.TryGetWebsiteItemPath(projectFileName, out itemPath))
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

                    ProjectFileNameToWebsiteItemPaths.Add(new ProjectFileNameToWebsiteItemPathMapper(projectDirectory, databaseName, itemPath, include, exclude));
                }

                var projectDirectoryToWebsiteDirectory = configuration.GetString(key + ":project-directory-to-website-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var projectDirectory = projectDirectoryToItemPath.Left(n).Trim();
                    var websiteDirectory = projectDirectoryToItemPath.Mid(n + 2).Trim();
                    var include = configuration.GetString(key + ":file-name-include");
                    var exclude = configuration.GetString(key + ":file-name-exclude");

                    ProjectFileNameToWebsiteFileNames.Add(new ProjectFileNameToWebsiteFileNameMapper(projectDirectory, websiteDirectory, include, exclude));
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

                    var itemPath = itemPathToProjectDirectory.Left(n).Trim();
                    var projectDirectory = itemPathToProjectDirectory.Mid(n + 2).Trim();
                    var format = configuration.GetString(key + ":format", "item.json");
                    var itemNameInclude = configuration.GetString(key + ":item-name-include");
                    var itemNameExclude = configuration.GetString(key + ":item-name-exclude");
                    var templateNameInclude = configuration.GetString(key + ":template-name-include");
                    var templateNameExclude = configuration.GetString(key + ":template-name-exclude");

                    WebsiteItemPathToProjectFileNames.Add(new WebsiteItemPathToProjectFileNameMapper(itemPath, projectDirectory, format, itemNameInclude, itemNameExclude, templateNameInclude, templateNameExclude));
                }

                var projectDirectoryToWebsiteDirectory = configuration.GetString(key + ":website-directory-to-project-directory");
                if (!string.IsNullOrEmpty(projectDirectoryToWebsiteDirectory))
                {
                    var n = projectDirectoryToWebsiteDirectory.IndexOf("=>", StringComparison.Ordinal);
                    if (n < 0)
                    {
                        throw new ConfigurationException(Texts.Missing_Mapping);
                    }

                    var websiteDirectory = itemPathToProjectDirectory.Left(n).Trim();
                    var projectDirectory = itemPathToProjectDirectory.Mid(n + 2).Trim();
                    var include = configuration.GetString(key + ":file-name-include");
                    var exclude = configuration.GetString(key + ":file-name-exclude");

                    WebsiteFileNameToProjectFileNames.Add(new WebsiteFileNameToProjectFileNameMapper(projectDirectory, websiteDirectory, include, exclude));
                }
            }
        }
    }
}
