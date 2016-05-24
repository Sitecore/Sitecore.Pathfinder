// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectIndexer))]
    public class ProjectIndexer : IProjectIndexer
    {
        [NotNull]
        protected ProjectIndex<DatabaseProjectItem> DatabaseGuidIndex { get; } = new ProjectIndex<DatabaseProjectItem>(item => item.DatabaseName.ToUpperInvariant() + ":" + item.Uri.Guid.Format());

        [NotNull]
        protected ProjectIndex<DatabaseProjectItem> DatabaseQualifiedNameIndex { get; } = new ProjectIndex<DatabaseProjectItem>(item => item.DatabaseName.ToUpperInvariant() + ":" + item.QualifiedName.ToUpperInvariant());

        [NotNull]
        protected ProjectIndex<IProjectItem> GuidIndex { get; } = new ProjectIndex<IProjectItem>(projectItem => projectItem.Uri.Guid.Format());

        [NotNull]
        protected ProjectIndex<IProjectItem> QualifiedNameIndex { get; } = new ProjectIndex<IProjectItem>(projectItem => projectItem.QualifiedName.ToUpperInvariant());

        [NotNull]
        protected ProjectIndex<IProjectItem> UriIndex { get; } = new ProjectIndex<IProjectItem>(projectItem => projectItem.Uri.ToString());

        public virtual void Add(IProjectItem projectItem)
        {
            lock (this)
            {
                GuidIndex.Add(projectItem);
                QualifiedNameIndex.Add(projectItem);
                UriIndex.Add(projectItem);

                var item = projectItem as DatabaseProjectItem;
                if (item == null)
                {
                    return;
                }

                DatabaseGuidIndex.Add(item);
                DatabaseQualifiedNameIndex.Add(item);
            }
        }

        public virtual T GetByGuid<T>(Guid guid) where T : class, IProjectItem
        {
            return GuidIndex.FirstOrDefault<T>(guid.Format());
        }

        public virtual T GetByGuid<T>(Database database, Guid guid) where T : DatabaseProjectItem
        {
            return DatabaseGuidIndex.FirstOrDefault<T>(database.DatabaseName.ToUpperInvariant() + ":" + guid.Format());
        }

        public virtual T GetByQualifiedName<T>(string qualifiedName) where T : class, IProjectItem
        {
            return QualifiedNameIndex.FirstOrDefault<T>(qualifiedName.ToUpperInvariant());
        }

        public IEnumerable<T> GetAllByQualifiedName<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            return DatabaseQualifiedNameIndex.Where<T>(database.DatabaseName.ToUpperInvariant() + ":" + qualifiedName.ToUpperInvariant());
        }

        public virtual T GetByQualifiedName<T>(Database database, string qualifiedName) where T : DatabaseProjectItem
        {
            return DatabaseQualifiedNameIndex.FirstOrDefault<T>(database.DatabaseName.ToUpperInvariant() + ":" + qualifiedName.ToUpperInvariant());
        }

        public virtual T GetByUri<T>(ProjectItemUri uri) where T : class, IProjectItem
        {
            return UriIndex.FirstOrDefault<T>(uri.ToString());
        }

        public virtual void Remove(IProjectItem projectItem)
        {
            lock (this)
            {
                GuidIndex.Remove(projectItem);
                UriIndex.Remove(projectItem);
                QualifiedNameIndex.Remove(projectItem);

                var item = projectItem as DatabaseProjectItem;
                if (item == null)
                {
                    return;
                }

                DatabaseGuidIndex.Remove(item);
                DatabaseQualifiedNameIndex.Remove(item);
            }
        }
    }
}
