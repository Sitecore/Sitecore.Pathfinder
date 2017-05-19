// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects.ProjectIndexes
{
    public class ProjectIndexes
    {
        public ProjectIndexes([NotNull] IProjectBase project)
        {
            Project = project;
        }

        [NotNull]
        public DatabaseIndex<Item> ChildrenIndex { get; } = new DatabaseIndex<Item>(item => item.ParentItemPath);

        [NotNull]
        public DatabaseIndex<DatabaseProjectItem> DatabaseGuidIndex { get; } = new DatabaseIndex<DatabaseProjectItem>(item => item.Uri.Guid.Format());

        [NotNull]
        public DatabaseIndex<DatabaseProjectItem> DatabaseQualifiedNameIndex { get; } = new DatabaseIndex<DatabaseProjectItem>(item => item.QualifiedName);

        [NotNull]
        public DatabaseIndex<DatabaseProjectItem> DatabaseShortNameIndex { get; } = new DatabaseIndex<DatabaseProjectItem>(item => item.ShortName);

        [NotNull]
        public Index<IProjectItem> GuidIndex { get; } = new Index<IProjectItem>(projectItem => projectItem.Uri.Guid.Format());

        [ItemNotNull, NotNull]
        public ICollection<Item> Items { get; } = new List<Item>();

        [NotNull]
        public Index<IProjectItem> QualifiedNameIndex { get; } = new Index<IProjectItem>(projectItem => projectItem.QualifiedName);

        [NotNull]
        public Index<IProjectItem> ShortNameIndex { get; } = new Index<IProjectItem>(item => item.ShortName);

        [NotNull]
        public Index<IProjectItem> SourceFileIndex { get; } = new Index<IProjectItem>();

        [ItemNotNull, NotNull]
        public ICollection<Template> Templates { get; } = new List<Template>();

        [NotNull]
        public Index<IProjectItem> UriIndex { get; } = new Index<IProjectItem>(projectItem => projectItem.Uri.ToString());

        [NotNull]
        protected IProjectBase Project { get; }

        public virtual void Add([NotNull] IProjectItem projectItem)
        {
            lock (this)
            {
                GuidIndex.Add(projectItem);
                UriIndex.Add(projectItem);
                QualifiedNameIndex.Add(projectItem);
                ShortNameIndex.Add(projectItem);

                foreach (var snapshot in projectItem.GetSnapshots())
                {
                    SourceFileIndex.Add(snapshot.SourceFile.GetFileNameWithoutExtensions(), projectItem);
                }

                if (projectItem is DatabaseProjectItem databaseProjectItem)
                {
                    DatabaseGuidIndex.Add(databaseProjectItem);
                    DatabaseQualifiedNameIndex.Add(databaseProjectItem);
                    DatabaseShortNameIndex.Add(databaseProjectItem);
                }

                if (projectItem is Item item)
                {
                    ChildrenIndex.Add(item);

                    if (!item.IsImport)
                    {
                        Items.Add(item);
                    }
                }

                if (projectItem is Template template && !template.IsImport)
                {
                    Templates.Add(template);
                }
            }
        }

        [CanBeNull]
        public virtual T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : class, IProjectItem
        {
            if (!qualifiedName.IsGuidOrSoftGuid())
            {
                return QualifiedNameIndex.FirstOrDefault<T>(qualifiedName);
            }

            if (!Guid.TryParse(qualifiedName, out Guid guid))
            {
                guid = StringHelper.ToGuid(qualifiedName);
            }

            return GuidIndex.FirstOrDefault<T>(guid.Format());
        }

        [CanBeNull]
        public virtual T FindQualifiedItem<T>([NotNull] IProjectItemUri uri) where T : class, IProjectItem
        {
            return UriIndex.FirstOrDefault<T>(uri.ToString());
        }

        [ItemNotNull, NotNull]
        public IEnumerable<T> GetByFileName<T>([NotNull] string fileName) where T : File
        {
            var relativeFileName = PathHelper.NormalizeFilePath(fileName);
            if (relativeFileName.StartsWith("~\\"))
            {
                relativeFileName = relativeFileName.Mid(2);
            }

            if (relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Mid(1);
            }

            return Project.ProjectItems.OfType<T>().Where(f => string.Equals(f.FilePath, fileName, StringComparison.OrdinalIgnoreCase) || f.GetSnapshots().Any(s => string.Equals(s.SourceFile.RelativeFileName, relativeFileName, StringComparison.OrdinalIgnoreCase)));
        }

        [ItemNotNull, NotNull]
        public virtual IEnumerable<T> GetBySourceFile<T>([NotNull] ISourceFile sourceFile) where T : class, IProjectItem
        {
            return SourceFileIndex.Where<T>(sourceFile.GetFileNameWithoutExtensions());
        }

        public virtual void Remove([NotNull] IProjectItem projectItem)
        {
            lock (this)
            {
                GuidIndex.Remove(projectItem);
                UriIndex.Remove(projectItem);
                QualifiedNameIndex.Remove(projectItem);
                ShortNameIndex.Remove(projectItem);

                foreach (var snapshot in projectItem.GetSnapshots())
                {
                    SourceFileIndex.Remove(snapshot.SourceFile.GetFileNameWithoutExtensions());
                }

                var databaseProjectItem = projectItem as DatabaseProjectItem;
                if (databaseProjectItem != null)
                {
                    DatabaseGuidIndex.Remove(databaseProjectItem);
                    DatabaseQualifiedNameIndex.Remove(databaseProjectItem);
                    DatabaseShortNameIndex.Remove(databaseProjectItem);
                }

                var item = projectItem as Item;
                if (item != null && !item.IsImport)
                {
                    Items.Remove(item);
                }

                if (item != null)
                {
                    ChildrenIndex.Remove(item);
                }

                var template = projectItem as Template;
                if (template != null && !template.IsImport)
                {
                    Templates.Remove(template);
                }
            }
        }
    }
}
