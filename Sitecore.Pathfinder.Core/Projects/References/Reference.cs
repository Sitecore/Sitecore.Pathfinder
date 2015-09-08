// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.References
{
    [DebuggerDisplay("{GetType().Name,nq}: {SourceProperty.GetValue()}")]
    public class Reference : IReference
    {
        private bool _isValid;

        public Reference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty)
        {
            Owner = owner;
            SourceProperty = sourceProperty;
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

        public SourceProperty<string> SourceProperty { get; set; }

        protected ProjectItemUri ResolvedUri { get; set; }

        public void Invalidate()
        {
            IsResolved = false;
            IsValid = false;
            ResolvedUri = null;
        }

        public virtual IProjectItem Resolve()
        {
            if (IsResolved)
            {
                if (!IsValid)
                {
                    return null;
                }

                var result = Owner.Project.Items.FirstOrDefault(i => i.Uri == ResolvedUri);
                if (result == null)
                {
                    IsValid = false;
                    ResolvedUri = null;
                }

                return result;
            }

            IsResolved = true;

            var projectItem = Owner.Project.FindQualifiedItem(SourceProperty.GetValue());
            if (projectItem == null)
            {
                IsValid = false;
                ResolvedUri = null;
                return null;
            }

            ResolvedUri = projectItem.Uri;
            IsValid = true;

            return projectItem;
        }
    }
}
