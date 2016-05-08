// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectIndexer))]
    public class ProjectIndexer : IProjectIndexer
    {
        [NotNull]
        protected IDictionary<string, IProjectItem> QualifiedNameIndex { get; } = new Dictionary<string, IProjectItem>();

        public void Add(IProjectItem projectItem)
        {
            QualifiedNameIndex.Add(projectItem.QualifiedName.ToUpperInvariant(), projectItem);
        }

        public T FindQualifiedItem<T>(string qualifiedName) where T : class, IProjectItem
        {
            IProjectItem projectItem;
            return QualifiedNameIndex.TryGetValue(qualifiedName.ToUpperInvariant(), out projectItem) ? projectItem as T : null;
        }
    }
}