// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

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
        private string _extension;

        [CanBeNull]
        private string _extensions;

        [CanBeNull]
        private string _shortName;

        public File([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath) : base(project, snapshot, GetUri(project, snapshot))
        {
            FilePath = filePath;
        }

        /// <summary>Gets the last extension of the FilePath property - including the period (".").</summary>
        [NotNull]
        public string Extension => _extension ?? (_extension = Path.GetExtension(FilePath));

        /// <summary>Gets all the extensions of the FilePath property - including the period (".").</summary>
        [NotNull]
        public string Extensions => _extensions ?? (_extensions = PathHelper.GetExtension(FilePath));

        [NotNull]
        public string FilePath { get; }

        public override string QualifiedName => Snapshots.First().SourceFile.AbsoluteFileName;

        public override string ShortName => _shortName ?? (_shortName = Path.GetFileName(Snapshots.First().SourceFile.AbsoluteFileName));

        [NotNull]
        private static IProjectItemUri GetUri([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot)
        {
            // include file extensions in project unique ID for file, so they don't clash with items
            var filePath = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.ProjectDirectory, snapshot.SourceFile.AbsoluteFileName)).TrimStart('/');
            return new ProjectItemUri(project, filePath);
        }
    }
}
