using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects
{
    public class Database : IDatabase
    {
        [NotNull]
        public static readonly IDatabase Empty = new Database();

        [NotNull]
        private readonly Dictionary<string, Language> _languages = new Dictionary<string, Language>();

        [NotNull]
        private readonly object _languagesSyncObject = new object();

        [FactoryConstructor]
        public Database([NotNull] IFactory factory, [NotNull] IProjectBase project, [NotNull] string databaseName, [ItemNotNull, NotNull] IEnumerable<string> languageNames)
        {
            Factory = factory;
            Project = project;
            DatabaseName = databaseName;

            foreach (var languageName in languageNames)
            {
                _languages[languageName] = Factory.Language(languageName);
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Database()
        {
        }

        public string DatabaseName { get; } = string.Empty;

        public IEnumerable<Item> Items => Project.Items.Where(i => Equals(i.Database));

        public IEnumerable<Language> Languages => _languages.Values;

        [NotNull, Obsolete("Use DatabaseName property", false)]
        public string Name => DatabaseName;

        public IProjectBase Project { get; } = Projects.Project.Empty;

        public IEnumerable<Template> Templates => Project.Templates.Where(t => Equals(t.Database));

        [NotNull]
        protected IFactory Factory { get; }

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

            return Equals((IDatabase) obj);
        }

        public T FindByIdOrPath<T>(string idOrPath) where T : DatabaseProjectItem
        {
            if (idOrPath.IsGuidOrSoftGuid() || idOrPath.IndexOf('/') >= 0)
            {
                return FindQualifiedItem<T>(idOrPath);
            }

            var items = GetByShortName<T>(idOrPath).ToArray();
            return items.Length == 1 ? items[0] : null;
        }

        public virtual T FindQualifiedItem<T>(string qualifiedName) where T : DatabaseProjectItem
        {
            if (!qualifiedName.IsGuidOrSoftGuid())
            {
                return Project.Indexes.QualifiedNameDatabaseIndex.FirstOrDefault<T>(this, qualifiedName);
            }

            if (!Guid.TryParse(qualifiedName, out Guid guid))
            {
                guid = StringHelper.ToGuid(qualifiedName);
            }

            return Project.Indexes.GuidDatabaseIndex.FirstOrDefault<T>(this, guid.Format());
        }

        public IEnumerable<T> GetByQualifiedName<T>(string qualifiedName) where T : DatabaseProjectItem => Project.Indexes.QualifiedNameDatabaseIndex.Where<T>(this, qualifiedName);

        public IEnumerable<T> GetByShortName<T>(string shortName) where T : DatabaseProjectItem => Project.Indexes.ShortNameDatabaseIndex.Where<T>(this, shortName);

        public override int GetHashCode() => DatabaseName.GetHashCode();

        public Item GetItem(string itemPath)
        {
            if (Guid.TryParse(itemPath, out Guid guid))
            {
                return Project.Indexes.FindQualifiedItem<Item>(new ProjectItemUri(DatabaseName, guid));
            }

            return FindQualifiedItem<Item>(itemPath);
        }

        public Item GetItem(Guid guid) => Project.Indexes.FindQualifiedItem<Item>(new ProjectItemUri(DatabaseName, guid));

        public Item GetItem(ProjectItemUri uri)
        {
            if (!string.Equals(uri.FileOrDatabaseName, DatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Project.Indexes.FindQualifiedItem<Item>(uri);
        }

        public Language GetLanguage(string languageName)
        {
            var key = languageName.ToUpperInvariant();

            Language language;
            lock (_languagesSyncObject)
            {
                if (!_languages.TryGetValue(key, out language))
                {
                    language = Factory.Language(languageName);
                    _languages[key] = language;
                }
            }

            return language;
        }

        public static bool operator ==([CanBeNull] Database left, [CanBeNull] Database right) => Equals(left, right);

        public static bool operator !=([CanBeNull] Database left, [CanBeNull] Database right) => !Equals(left, right);

        protected bool Equals([NotNull] IDatabase other) => string.Equals(DatabaseName, other.DatabaseName);
    }
}
