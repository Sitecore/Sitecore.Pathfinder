// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.References
{
    [DebuggerDisplay("{GetType().Name,nq}: {ReferenceText}")]
    public class Reference : IReference
    {
        [NotNull]
        private readonly object _syncRoot = new object();

        public Reference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText, [NotNull] string databaseName)
        {
            // the reference text might be different from the source property value. 
            // e.g. the source property value might be a list of guids while the reference text is a single Guid.
            Owner = owner;
            SourceProperty = sourceProperty;
            TextNode = sourceProperty.SourceTextNode;
            ReferenceText = referenceText;
            DatabaseName = databaseName;
        }

        public Reference([NotNull] IProjectItem owner, [NotNull] ITextNode textNode, [NotNull] string referenceText, [NotNull] string databaseName)
        {
            // the reference text might be different from the source property value. 
            // e.g. the source property value might be a list of guids while the reference text is a single Guid.
            Owner = owner;
            SourceProperty = null;
            TextNode = textNode;
            ReferenceText = referenceText;
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; }

        public bool IsValid => Resolve() != null;

        public IProjectItem Owner { get; }

        public string ReferenceText { get; }

        public SourceProperty<string> SourceProperty { get; }

        public ITextNode TextNode { get; }

        public void Invalidate()
        {
        }

        public virtual IProjectItem Resolve()
        {
            return Owner.Project.FindQualifiedItem<IProjectItem>(ReferenceText);
        }
    }
}
