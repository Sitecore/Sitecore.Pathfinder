// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class File : ProjectItem
    {
        private string _filePath;

        private string _shortName;

        public File([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, GetProjectUniqueId(project, snapshot), snapshot)
        {
        }

        public string FilePath => _filePath ?? (_filePath = PathHelper.GetFilePath(Project, Snapshots.First().SourceFile));

        public override string QualifiedName => Snapshots.First().SourceFile.FileName;

        public override string ShortName => _shortName ?? (_shortName = Path.GetFileName(Snapshots.First().SourceFile.FileName));

        public override void Rename(string newShortName)
        {
            var fileName = Snapshots.First().SourceFile.FileName;

            var n = fileName.LastIndexOf('.');
            var extension = n >= 0 ? fileName.Mid(n) : string.Empty;

            var newFileName = Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, newShortName + extension);

            Project.FileSystem.Rename(fileName, newFileName);

            // todo: update Project Unique ID
        }

        [NotNull]
        private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] ISnapshot snapshot)
        {
            return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, snapshot.SourceFile.FileName));
        }
    }
}
