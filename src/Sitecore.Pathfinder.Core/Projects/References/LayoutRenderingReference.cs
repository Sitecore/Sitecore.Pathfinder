// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects.References
{
    public class LayoutRenderingReference : Reference
    {
        [FactoryConstructor]
        public LayoutRenderingReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText, [NotNull] string databaseName) : base(owner, sourceProperty, referenceText, databaseName)
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

                return projectItem;
            }

            return null;
        }
    }
}
