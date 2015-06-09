// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
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

        public string FilePath => _filePath ?? (_filePath = PathHelper.GetFilePath(Project, Snapshot.SourceFile));

        public override string QualifiedName => Snapshot.SourceFile.FileName;

        public override string ShortName => _shortName ?? (_shortName = Path.GetFileName(Snapshot.SourceFile.FileName));

        public override void Rename(string newQualifiedName)
        {
            // this.Project.FileSystem.Rename();
        }

        [NotNull]
        private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] ISnapshot snapshot)
        {
            return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, snapshot.SourceFile.FileName));
        }
    }
}
