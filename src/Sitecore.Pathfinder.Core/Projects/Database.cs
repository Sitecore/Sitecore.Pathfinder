// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects
{
    public class Database
    {
        public Database([NotNull] IProject project, [NotNull] string databaseName)
        {
            Project = project;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        [Obsolete("Use DatabaseName property", false)]
        public string Name => DatabaseName;

        [NotNull]
        public IProject Project { get; }

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
            return Equals((Database)obj);
        }

        public override int GetHashCode()
        {
            return DatabaseName.GetHashCode();
        }

        [CanBeNull]
        public Item GetItem([NotNull] string path)
        {
            return Project.FindQualifiedItem<Item>(DatabaseName, path);
        }

        public static bool operator ==([CanBeNull] Database left, [CanBeNull] Database right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([CanBeNull] Database left, [CanBeNull] Database right)
        {
            return !Equals(left, right);
        }

        protected bool Equals([NotNull] Database other)
        {
            return string.Equals(DatabaseName, other.DatabaseName);
        }
    }
}
