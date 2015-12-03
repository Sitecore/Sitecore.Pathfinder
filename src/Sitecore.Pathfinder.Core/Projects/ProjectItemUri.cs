// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects
{
    public sealed class ProjectItemUri
    {
        [NotNull]
        public static readonly ProjectItemUri Empty = new ProjectItemUri();

        public ProjectItemUri([NotNull] IProject project, [NotNull] string filePath)
        {
            // guids exclude typeOrDatabase, so items have same id across databases
            // - compare uris when database is important
            // - compare guids when database is unimportant

            FileOrDatabaseName = "file";
            Guid = StringHelper.GetGuid(project, filePath);
        }

        public ProjectItemUri([NotNull] string databaseName, Guid guid)
        {
            FileOrDatabaseName = databaseName;
            Guid = guid;
        }

        private ProjectItemUri()
        {
            FileOrDatabaseName = string.Empty;
            Guid = Guid.Empty;
        }

        [NotNull]
        public string FileOrDatabaseName { get; }

        public Guid Guid { get; }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ProjectItemUri && Equals((ProjectItemUri)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.OrdinalIgnoreCase.GetHashCode(Guid) * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(FileOrDatabaseName);
            }
        }

        public static bool operator ==([CanBeNull] ProjectItemUri left, [CanBeNull] ProjectItemUri right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] ProjectItemUri left, [CanBeNull] ProjectItemUri right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{FileOrDatabaseName}:{Guid.ToString("B").ToUpperInvariant()}";
        }

        private bool Equals([NotNull] ProjectItemUri other)
        {
            return Guid == other.Guid && string.Equals(FileOrDatabaseName, other.FileOrDatabaseName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
