// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects.References
{
    public class LayoutRenderingReference : Reference
    {
        public LayoutRenderingReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText) : base(owner, sourceProperty, referenceText)
        {
        }

        public override IProjectItem Resolve()
        {
            var layoutName = ReferenceText;
            foreach (var projectItem in Owner.Project.ProjectItems.Where(i => string.Equals(i.ShortName, layoutName, StringComparison.OrdinalIgnoreCase)))
            {
                var item = projectItem as Item;
                if (item == null)
                {
                    continue;
                }

                // var templateIdOrPath = item.TemplateIdOrPath;
                // if (templateIdOrPath != Constants.Templates.ViewRendering && templateIdOrPath != Constants.Templates.Sublayout)
                // {
                //     continue;
                // }

                IsResolved = true;
                IsValid = true;
                return projectItem;
            }

            return null;
        }
    }
}
