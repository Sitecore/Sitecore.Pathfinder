// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeDatabase
    {
        public CodeDatabase([NotNull] CodeProject project, [NotNull] string databaseName)
        {
            Project = project;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public CodeProject Project { get; }

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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((CodeDatabase)obj);
        }

        public override int GetHashCode()
        {
            return DatabaseName.GetHashCode();
        }

        [CanBeNull]
        public CodeItem GetItem([NotNull] string path)
        {
            var item = Project.InnerProject.FindQualifiedItem(path) as Item;
            return item == null ? null : new CodeItem(Project, item);
        }

        public static bool operator ==([CanBeNull] CodeDatabase left, [CanBeNull] CodeDatabase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] CodeDatabase left, [CanBeNull] CodeDatabase right)
        {
            return !Equals(left, right);
        }

        protected bool Equals([NotNull] CodeDatabase other)
        {
            return string.Equals(DatabaseName, other.DatabaseName);
        }
    }
}
