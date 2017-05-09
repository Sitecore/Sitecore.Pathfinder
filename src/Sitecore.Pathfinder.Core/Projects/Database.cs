// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    public class Database
    {
        public Database([NotNull] IProjectBase project, [NotNull] string databaseName)
        {
            Project = project;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull, Obsolete("Use DatabaseName property", false)]
        public string Name => DatabaseName;

        [NotNull]
        public IProjectBase Project { get; }

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
            return Project.FindQualifiedItem<Item>(this, path);
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Item> GetItems()
        {
            return Project.Items.Where(i => string.Equals(i.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase));
        }

        [NotNull]
        public Language GetLanguage([NotNull] string languageName)
        {
            return Project.GetLanguage(languageName);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Language> GetLanguages()
        {
            return GetItems().SelectMany(i => i.GetLanguages()).Distinct();
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Template> GetTemplates()
        {
            return Project.Templates.Where(i => string.Equals(i.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase));
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
