// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Checking
{
    public class WebsiteProject : IProjectBase
    {
        [NotNull]
        private readonly Database _database;

        [NotNull, ItemNotNull]
        private readonly List<IProjectItem> _projectItems = new List<IProjectItem>();

        public WebsiteProject([NotNull] Data.Database database, [NotNull] ProjectOptions options, [NotNull] string projectDirectory, [NotNull] string projectUniqueId)
        {
            Database = database;
            Options = options;
            ProjectDirectory = projectDirectory;
            ProjectUniqueId = projectUniqueId;

            _database = new Database(this, database.Name);

            Index = new ProjectIndexer();

            LoadDatabase(database);
        }

        public IEnumerable<File> Files => Enumerable.Empty<File>();

        public IEnumerable<Item> Items => Enumerable.Empty<Item>();

        public Locking Locking => Locking.ReadWrite;

        public ProjectOptions Options { get; }

        public string ProjectDirectory { get; }

        public IEnumerable<IProjectItem> ProjectItems => _projectItems;

        public string ProjectUniqueId { get; }

        public IEnumerable<Template> Templates => Index.Templates;

        [NotNull]
        protected Data.Database Database { get; }

        [NotNull]
        protected IProjectIndexer Index { get; }

        public T FindQualifiedItem<T>(IProjectItemUri uri) where T : class, IProjectItem
        {
            return Index.FindQualifiedItem<T>(uri);
        }

        public T FindQualifiedItem<T>(string qualifiedName) where T : class, IProjectItem
        {
            return Index.FindQualifiedItem<T>(qualifiedName);
        }

        public T FindQualifiedItem<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            return Index.FindQualifiedItem<T>(database, qualifiedName);
        }

        public IEnumerable<T> GetByFileName<T>(string fileName) where T : File
        {
            yield break;
        }

        public IEnumerable<T> GetByQualifiedName<T>(string qualifiedName) where T : class, IProjectItem
        {
            return Index.GetByQualifiedName<T>(qualifiedName);
        }

        public IEnumerable<T> GetByQualifiedName<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            return Index.GetByQualifiedName<T>(database, qualifiedName);
        }

        public IEnumerable<T> GetByShortName<T>(string shortName) where T : class, IProjectItem
        {
            return Index.GetByShortName<T>(shortName);
        }

        public IEnumerable<T> GetByShortName<T>(Database database, string shortName) where T : DatabaseProjectItem
        {
            return Index.GetByShortName<T>(database, shortName);
        }

        public IEnumerable<Item> GetChildren(Item item)
        {
            return Index.GetChildren(item);
        }

        public Database GetDatabase(string databaseName)
        {
            return _database;
        }

        public IEnumerable<IProjectItem> GetUsages(string qualifiedName)
        {
            return Index.FindUsages(qualifiedName).Select(r => r.Resolve());
        }

        private void Add([NotNull] IProjectItem projectItem)
        {
            _projectItems.Add(projectItem);
            Index.Add(projectItem);
        }

        private void LoadDatabase([NotNull] Data.Database database)
        {
            LoadTemplates(database);
        }

        private void LoadTemplates([NotNull] Data.Database database)
        {
            foreach (var pair in TemplateManager.GetTemplates(database))
            {
                var databaseTemplate = pair.Value;
                var template = new Template(this, databaseTemplate.ID.ToGuid(), database.Name, databaseTemplate.Name, databaseTemplate.FullName);

                template.BaseTemplates = string.Join("|", databaseTemplate.BaseIDs.Select(baseId => baseId.ToString()));

                foreach (var databaseTemplateSection in databaseTemplate.GetSections())
                {
                    var templateSection = new TemplateSection(template, databaseTemplateSection.ID.ToGuid());
                    templateSection.SectionName = databaseTemplateSection.Name;

                    template.Sections.Add(templateSection);

                    foreach (var databaseTemplateField in databaseTemplateSection.GetFields())
                    {
                        var templateField = new TemplateField(template, databaseTemplateField.ID.ToGuid());
                        templateField.FieldName = databaseTemplateField.Name;
                        templateField.Shared = databaseTemplateField.IsShared;
                        templateField.Unversioned = databaseTemplateField.IsUnversioned;
                        templateField.Type = databaseTemplateField.Type;
                        templateField.Source = databaseTemplateField.Source;

                        templateSection.Fields.Add(templateField);
                    }
                }

                Add(template);
            }
        }
    }
}
