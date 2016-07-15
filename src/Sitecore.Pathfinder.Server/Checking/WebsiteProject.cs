// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Checking
{
    [Export]
    public class WebsiteProject : IProjectBase, IDiagnosticCollector
    {
        [NotNull, ItemNotNull]
        private readonly IList<Diagnostic> _diagnostics = new SynchronizedCollection<Diagnostic>();

        [NotNull, ItemNotNull]
        private readonly List<IProjectItem> _projectItems = new List<IProjectItem>();

        [NotNull]
        private Database _database;

        [ImportingConstructor]
        public WebsiteProject([NotNull] IProjectIndexer index, [NotNull] IReferenceParserService referenceParser)
        {
            Index = index;
            ReferenceParser = referenceParser;
            Locking = Locking.ReadWrite;
        }

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        public IEnumerable<File> Files => Enumerable.Empty<File>();

        public IEnumerable<Item> Items => Index.Items;

        public Locking Locking { get; set; }

        public ProjectOptions Options { get; private set; }

        public string ProjectDirectory { get; private set; }

        public IEnumerable<IProjectItem> ProjectItems => _projectItems;

        public string ProjectUniqueId { get; private set; }

        public IEnumerable<Template> Templates => Index.Templates;

        [NotNull]
        protected Data.Database Database { get; private set; }

        [NotNull]
        protected IProjectIndexer Index { get; }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        public T FindQualifiedItem<T>(IProjectItemUri uri) where T : class, IProjectItem
        {
            return Index.FindQualifiedItem<T>(uri);
        }

        public T FindQualifiedItem<T>(string qualifiedName) where T : class, IProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Index.FindQualifiedItem<T>(qualifiedName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Index.FindQualifiedItem<T>(guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Index.FindQualifiedItem<T>(guid);
        }

        public T FindQualifiedItem<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            if (!qualifiedName.StartsWith("{") || !qualifiedName.EndsWith("}"))
            {
                return Index.FindQualifiedItem<T>(database, qualifiedName);
            }

            Guid guid;
            if (Guid.TryParse(qualifiedName, out guid))
            {
                return Index.FirstOrDefault<T>(database, guid);
            }

            guid = StringHelper.ToGuid(qualifiedName);
            return Index.FirstOrDefault<T>(database, guid);
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

        [NotNull]
        public WebsiteProject With([NotNull] Data.Database database, [NotNull] ProjectOptions options, [NotNull] string projectDirectory, [NotNull] string projectUniqueId)
        {
            Database = database;
            Options = options;
            ProjectDirectory = projectDirectory;
            ProjectUniqueId = projectUniqueId;

            _database = new Database(this, database.Name);

            LoadDatabase(database);

            return this;
        }

        void IDiagnosticCollector.Add(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        [NotNull]
        private readonly object _syncObject = new object();

        private void Add([NotNull] IProjectItem projectItem)
        {
            lock (_syncObject)
            {
                _projectItems.Add(projectItem);
                Index.Add(projectItem);
            }
        }

        private void AddField([NotNull] ISnapshot snapshot, [NotNull] Item item, [NotNull] Data.Fields.Field databaseField)
        {
            var field = new Field(item).With(new TextNode(snapshot, databaseField.Name, databaseField.Value, TextSpan.Empty));
            field.FieldId = databaseField.ID.ToGuid();
            field.FieldNameProperty.SetValue(new TextNode(snapshot, "Name", databaseField.Name, TextSpan.Empty));
            field.ValueProperty.SetValue(new TextNode(snapshot, "Value", databaseField.Value, TextSpan.Empty));

            item.Fields.Add(field);

            item.References.AddRange(ReferenceParser.ParseReferences(field));
        }

        void IDiagnosticCollector.Clear()
        {
            _diagnostics.Clear();
        }

        private void LoadDatabase([NotNull] Data.Database database)
        {
            // todo: reconsider using SecurityDisabler
            using (new SecurityDisabler())
            {
                LoadTemplates(database);
                LoadItems(database.GetRootItem());
            }
        }

        private void LoadItems([NotNull] Data.Items.Item databaseItem)
        {
            var templateItem = databaseItem.Database.GetItem(databaseItem.TemplateID);
            var templateIdOrPath = templateItem != null ? templateItem.Paths.Path : databaseItem.Template.FullName;

            var snapshot = new Snapshot().With(new ItemSourceFile(databaseItem));
            var item = new Item(this, databaseItem.ID.ToGuid(), databaseItem.Database.Name, databaseItem.Name, databaseItem.Paths.Path, templateIdOrPath).With(new SnapshotTextNode(snapshot));

            item.IconProperty.SetValue(new TextNode(snapshot, "__Icon", databaseItem.Appearance.Icon, TextSpan.Empty));

            databaseItem.Fields.ReadAll();

            foreach (Data.Fields.Field databaseField in databaseItem.Fields)
            {
                if (databaseField.Shared && !databaseField.ContainsStandardValue && !string.IsNullOrEmpty(databaseField.Value))
                {
                    AddField(snapshot, item, databaseField);
                }
            }

            foreach (var language in databaseItem.Database.GetLanguages())
            {
                var languageItem = databaseItem.Database.GetItem(databaseItem.ID, language);
                if (language == null)
                {
                    continue;
                }

                languageItem.Fields.ReadAll();

                foreach (Data.Fields.Field databaseField in languageItem.Fields)
                {
                    if (!databaseField.Shared && databaseField.Unversioned && !databaseField.ContainsStandardValue && !string.IsNullOrEmpty(databaseField.Value))
                    {
                        AddField(snapshot, item, databaseField);
                    }
                }

                foreach (var versionItem in languageItem.Versions.GetVersions())
                {
                    versionItem.Fields.ReadAll();

                    foreach (Data.Fields.Field databaseField in versionItem.Fields)
                    {
                        if (!databaseField.Shared && !databaseField.Unversioned && !databaseField.ContainsStandardValue && !string.IsNullOrEmpty(databaseField.Value))
                        {
                            AddField(snapshot, item, databaseField);
                        }
                    }
                }
            }

            Add(item);

            foreach (Data.Items.Item child in databaseItem.Children)
            {
                LoadItems(child);
            }
        }

        private void LoadTemplates([NotNull] Data.Database database)
        {
            foreach (var pair in TemplateManager.GetTemplates(database))
            {
                var databaseTemplate = pair.Value;

                var databaseTemplateItem = database.GetItem(databaseTemplate.ID);
                if (databaseTemplateItem == null)
                {
                    continue;
                }

                var snapshot = new Snapshot().With(new ItemSourceFile(databaseTemplateItem));
                var template = new Template(this, databaseTemplate.ID.ToGuid(), database.Name, databaseTemplate.Name, databaseTemplateItem.Paths.Path).With(new SnapshotTextNode(snapshot));

                template.BaseTemplatesProperty.SetValue(new TextNode(snapshot, "__Base template", string.Join("|", databaseTemplate.BaseIDs.Select(baseId => baseId.ToString())), TextSpan.Empty));
                template.ShortHelpProperty.SetValue(new TextNode(snapshot, "__Short description", databaseTemplateItem.Help.ToolTip, TextSpan.Empty));
                template.LongHelpProperty.SetValue(new TextNode(snapshot, "__Long description", databaseTemplateItem.Help.Text, TextSpan.Empty));
                template.IconProperty.SetValue(new TextNode(snapshot, "__Icon", databaseTemplateItem.Appearance.Icon, TextSpan.Empty));

                template.References.AddRange(ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));

                foreach (var databaseTemplateSection in databaseTemplate.GetSections())
                {
                    var templateSection = new TemplateSection(template, databaseTemplateSection.ID.ToGuid());
                    templateSection.SectionNameProperty.SetValue(new TextNode(snapshot, "Name", databaseTemplateSection.Name, TextSpan.Empty));
                    templateSection.IconProperty.SetValue(new TextNode(snapshot, "__Icon", databaseTemplateSection.Icon, TextSpan.Empty));
                    templateSection.SortorderProperty.SetValue(new TextNode(snapshot, "__Sort order", databaseTemplateSection.Sortorder.ToString(), TextSpan.Empty));
                    ;

                    template.Sections.Add(templateSection);

                    foreach (var databaseTemplateField in databaseTemplateSection.GetFields())
                    {
                        var templateField = new TemplateField(template, databaseTemplateField.ID.ToGuid());
                        templateField.FieldNameProperty.SetValue(new TextNode(snapshot, "name", databaseTemplateField.Name, TextSpan.Empty));
                        templateField.Shared = databaseTemplateField.IsShared;
                        templateField.Unversioned = databaseTemplateField.IsUnversioned;
                        templateField.TypeProperty.SetValue(new TextNode(snapshot, "Type", databaseTemplateField.Type, TextSpan.Empty));
                        templateField.SourceProperty.SetValue(new TextNode(snapshot, "Source", databaseTemplateField.Source, TextSpan.Empty));
                        templateField.IconProperty.SetValue(new TextNode(snapshot, "__Icon", databaseTemplateField.Icon, TextSpan.Empty));

                        templateSection.Fields.Add(templateField);

                        template.References.AddRange(ReferenceParser.ParseReferences(template, templateField.SourceProperty));
                    }
                }

                Add(template);
            }
        }

        void IDiagnosticCollector.Remove(Diagnostic diagnostic)
        {
            _diagnostics.Remove(diagnostic);
        }
    }
}
