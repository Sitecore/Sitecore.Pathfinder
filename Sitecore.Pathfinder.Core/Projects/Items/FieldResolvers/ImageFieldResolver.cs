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

        public override bool CanResolve(Field field)
        {
            return string.Compare(field.TemplateField.Type, "image", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(Field field)
        {
            var qualifiedName = field.Value.Trim();
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            var item = field.Item.Project.FindQualifiedItem(qualifiedName);
            if (item == null)
            {
                field.WriteDiagnostic(Severity.Error, "Image reference not found", qualifiedName);
                return string.Empty;
            }

            return $"<image mediapath=\"\" alt=\"Vista15\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{item.Guid.Format()}\" />";
        }
    }
}
