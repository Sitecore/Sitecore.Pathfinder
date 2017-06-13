// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public class WebsiteItemPathToProjectDirectoryMapper : IItemPathToProjectFileNameMapper
    {
        public WebsiteItemPathToProjectDirectoryMapper([CanBeNull] IPathMatcher itemNamePathMatcher, [CanBeNull] IPathMatcher templateNamePathMatcher, [NotNull] string databaseName, [NotNull] string itemPath, [NotNull] string projectDirectory, [NotNull] string format)
        {
            IsMapped = !string.IsNullOrEmpty(projectDirectory);

            ItemPath = '/' + PathHelper.NormalizeItemPath(itemPath).Trim('/');
            ProjectDirectory = '\\' + PathHelper.NormalizeFilePath(projectDirectory).Trim('\\');
            ItemNamePathMatcher = itemNamePathMatcher;
            TemplateNamePathMatcher = templateNamePathMatcher;
            DatabaseName = databaseName;
            Format = format;
        }

        public string DatabaseName { get; }

        public string Format { get; }

        public bool IsMapped { get; }

        public string ItemPath { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [CanBeNull]
        protected IPathMatcher ItemNamePathMatcher { get; }

        [CanBeNull]
        protected IPathMatcher TemplateNamePathMatcher { get; }

        public bool TryGetProjectFileName(string itemPath, string templateName, out string projectFileName, out string format)
        {
            projectFileName = string.Empty;
            format = string.Empty;

            if (!IsMapped)
            {
                return false;
            }

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
