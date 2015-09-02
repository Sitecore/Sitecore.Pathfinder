// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.References
{
    [DebuggerDisplay("{GetType().Name,nq}: {TargetQualifiedName}")]
    public class Reference : IReference
    {
        private bool _isValid;

        public Reference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName)
        {
            Owner = owner;
            TargetQualifiedName = targetQualifiedName;
        }

        public Reference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceSourceProperty, [NotNull] string targetQualifiedName)
        {
            Owner = owner;
            SourceSourceProperty = sourceSourceProperty;
            TargetQualifiedName = targetQualifiedName;
        }

        public bool IsResolved { get; set; }

        public bool IsValid
        {
            get
            {
                if (!IsResolved)
                {
                    Resolve();
                }

                return _isValid;
            }

            protected set
            {
                _isValid = value;
            }
        }

        public IProjectItem Owner { get; }

        public SourceProperty<string> SourceSourceProperty { get; set; }

        public string TargetQualifiedName { get; }

        protected Guid TargetProjectItemGuid { get; set; } = Guid.Empty;

        public void Invalidate()
        {
            IsResolved = false;
            IsValid = false;
            TargetProjectItemGuid = Guid.Empty;
        }

        public virtual IProjectItem Resolve()
        {
            if (IsResolved)
            {
                if (!IsValid)
                {
                    return null;
                }

                var result = Owner.Project.Items.FirstOrDefault(i => i.Guid == TargetProjectItemGuid);
                if (result == null)
                {
                    IsValid = false;
                    TargetProjectItemGuid = Guid.Empty;
                }

                return result;
            }

            IsResolved = true;

            var projectItem = Owner.Project.FindQualifiedItem(TargetQualifiedName);
            if (projectItem == null)
            {
                IsValid = false;
                TargetProjectItemGuid = Guid.Empty;
                return null;
            }

            TargetProjectItemGuid = projectItem.Guid;
            IsValid = true;

            return projectItem;
        }
    }
}
