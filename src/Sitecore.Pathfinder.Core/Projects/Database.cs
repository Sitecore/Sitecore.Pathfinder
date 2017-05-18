// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    public class Database
    {
        [NotNull]
        public static readonly Database Empty = new Database(Projects.Project.Empty, string.Empty, new string[0]);

        [NotNull]
        private readonly Dictionary<string, Language> _languages = new Dictionary<string, Language>();

        [NotNull]
        private readonly object _languagesSyncObject = new object();

        public Database([NotNull] IProjectBase project, [NotNull] string databaseName, [ItemNotNull, NotNull] IEnumerable<string> languageNames)
        {
            Project = project;
            DatabaseName = databaseName;

            foreach (var languageName in languageNames)
            {
                _languages[languageName] = new Language(languageName);
            }
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull, ItemNotNull]
        public IEnumerable<Item> Items => Project.Items.Where(i => string.Equals(i.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase));

        [ItemNotNull, NotNull]
        public IEnumerable<Language> Languages => _languages.Values;

        [NotNull, Obsolete("Use DatabaseName property", false)]
        public string Name => DatabaseName;

        [NotNull]
        public IProjectBase Project { get; }

        [NotNull, ItemNotNull]
        public IEnumerable<Template> Templates => Project.Templates.Where(i => string.Equals(i.DatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase));

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
        public Item GetItem([NotNull] string itemPath)
        {
            if (itemPath.IsGuid())
            {
                return Project.FindQualifiedItem<Item>(new ProjectItemUri(DatabaseName, Guid.Parse(itemPath)));
            }

            return Project.FindQualifiedItem<Item>(this, itemPath);
        }

        [CanBeNull]
        public Item GetItem(Guid guid)
        {
            return Project.FindQualifiedItem<Item>(new ProjectItemUri(DatabaseName, guid));
        }

        [CanBeNull]
        public Item GetItem([NotNull] ProjectItemUri uri)
        {
            if (!string.Equals(uri.FileOrDatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Project.FindQualifiedItem<Item>(uri);
        }

        [NotNull]
        public Language GetLanguage([NotNull] string languageName)
        {
            var key = languageName.ToUpperInvariant();

            Language language;
            lock (_languagesSyncObject)
            {
                if (!_languages.TryGetValue(key, out language))
                {
                    language = new Language(languageName);
                    _languages[key] = language;
                }
            }

            return language;
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
