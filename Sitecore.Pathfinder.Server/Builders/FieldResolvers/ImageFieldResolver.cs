// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Configuration;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class ImageFieldResolver : FieldResolverBase
    {
        public ImageFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
        {
            return string.Compare(templateField.Type, "image", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
        {
            if (!field.Value.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
            {
                return field.Value.Value;
            }

            var database = Factory.GetDatabase(field.Item.DatabaseName);
            var mediaItem = database.GetItem(field.Value.Value);
            if (mediaItem == null)
            {
                throw new RetryableEmitException(Texts.Media_item_not_found, field.Value.Source ?? TextNode.Empty, field.Value.Value);
            }

            return $"<image mediapath=\"\" alt=\"Vista15\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{mediaItem.ID}\" />";
        }
    }
}
