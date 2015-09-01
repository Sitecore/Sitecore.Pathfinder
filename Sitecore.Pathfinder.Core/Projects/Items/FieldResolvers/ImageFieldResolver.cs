// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class ImageFieldResolver : FieldResolverBase
    {
        public ImageFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(ITraceService trace, IProject project, Field field)
        {
            return string.Compare(field.TemplateField.Type, "image", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(ITraceService trace, IProject project, Field field)
        {
            var qualifiedName = field.Value.Value;
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            var item = project.FindQualifiedItem(qualifiedName);
            if (item == null)
            {
                trace.Writeline("Image reference not found", qualifiedName);
            }

            return $"<image mediapath=\"\" alt=\"Vista15\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{item.Guid.Format()}\" />";
        }
    }
}
