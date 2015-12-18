// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class WebsiteItemPathToProjectDirectoryMapper : IItemPathToProjectFileNameMapper
    {
        public WebsiteItemPathToProjectDirectoryMapper([NotNull] string databaseName, [NotNull] string itemPath, [NotNull] string projectDirectory, [NotNull] string format, [NotNull] string itemNameInclude, [NotNull] string itemNameExclude, [NotNull] string templateNameInclude, [NotNull] string templateNameExclude)
        {
            ItemPath = '/' + PathHelper.NormalizeItemPath(itemPath).Trim('/');
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            DatabaseName = databaseName;
            Format = format;
            ItemNameInclude = itemNameInclude;
            ItemNameExclude = itemNameExclude;
            TemplateNameInclude = templateNameInclude;
            TemplateNameExclude = templateNameExclude;

            if (!string.IsNullOrEmpty(ItemNameInclude) || !string.IsNullOrEmpty(ItemNameExclude))
            {
                if (!string.IsNullOrEmpty(itemNameInclude))
                {
                    itemNameInclude = ItemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(itemNameInclude).Trim('/');
                }

                if (!string.IsNullOrEmpty(itemNameExclude))
                {
                    itemNameExclude = ItemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(itemNameExclude).Trim('/');
                }

                ItemNamePathMatcher = new PathMatcher(itemNameInclude, itemNameExclude);
            }

            if (!string.IsNullOrEmpty(TemplateNameInclude) || !string.IsNullOrEmpty(TemplateNameExclude))
            {
                if (!string.IsNullOrEmpty(templateNameInclude))
                {
                    templateNameInclude = ItemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(templateNameInclude).Trim('/');
                }

                if (!string.IsNullOrEmpty(templateNameExclude))
                {
                    templateNameExclude = ItemPath.TrimEnd('/') + '/' + PathHelper.NormalizeItemPath(templateNameExclude).Trim('/');
                }

                TemplateNamePathMatcher = new PathMatcher(templateNameInclude, templateNameExclude);
            }
        }

        public string DatabaseName { get; }

        public string Format { get; }

        [NotNull]
        public string ItemNameExclude { get; }

        [NotNull]
        public string ItemNameInclude { get; }

        public string ItemPath { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [NotNull]
        public string TemplateNameExclude { get; }

        [NotNull]
        public string TemplateNameInclude { get; }

        [CanBeNull]
        protected PathMatcher ItemNamePathMatcher { get; }

        [CanBeNull]
        protected PathMatcher TemplateNamePathMatcher { get; }

        public bool TryGetProjectFileName(string itemPath, string templateName, out string projectFileName, out string format)
        {
            projectFileName = string.Empty;
            format = string.Empty;

            if (!itemPath.StartsWith(ItemPath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (ItemNamePathMatcher != null && !ItemNamePathMatcher.IsMatch(itemPath))
            {
                return false;
            }

            if (TemplateNamePathMatcher != null && !TemplateNamePathMatcher.IsMatch(templateName))
            {
                return false;
            }

            projectFileName = PathHelper.RemapDirectory(itemPath, ItemPath, ProjectDirectory).TrimStart('\\') + "." + Format;
            format = Format;

            return true;
        }
    }
}
