// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    public static class ProjectExtensions
    {
        public static bool DirectoryExists([NotNull] this IProject project, [NotNull] string directory)
        {
            Assert.IsNotNullOrEmpty(directory);

            if (!directory.StartsWith("~/"))
            {
                throw new ArgumentException("Directory must start with '~/'");
            }

            directory = Path.Combine(project.Options.ProjectDirectory, PathHelper.NormalizeFilePath(directory.Mid(2)));

            return project.FileSystem.DirectoryExists(directory);
        }

        public static bool FileExists([NotNull] this IProject project, [NotNull] string fileName)
        {
            Assert.IsNotNullOrEmpty(fileName);

            if (!fileName.StartsWith("~/"))
            {
                throw new ArgumentException("File name must start with '~/'");
            }

            fileName = Path.Combine(project.Options.ProjectDirectory, PathHelper.NormalizeFilePath(fileName.Mid(2)));

            return project.FileSystem.FileExists(fileName);
        }
    }
}
