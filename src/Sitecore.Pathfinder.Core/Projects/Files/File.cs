// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Files
{
    [DebuggerDisplay("{GetType().Name,nq}: {FilePath}")]
    public class File : ProjectItem
    {
        [CanBeNull]
        private string _shortName;

        public File([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath) : base(project, GetUri(project, snapshot), snapshot)
        {
            FilePath = filePath;
        }

        [NotNull]
        public string FilePath { get; }

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
        private static ProjectItemUri GetUri([NotNull] IProject project, [NotNull] ISnapshot snapshot)
        {
            // include file extensions in project unique ID for file, so they don't clash with items
            var filePath = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, snapshot.SourceFile.FileName)).TrimStart('/');
            return new ProjectItemUri(project, filePath);
        }
    }
}
