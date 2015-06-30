// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class ItemPathResolver : FieldResolverBase
    {
        public ItemPathResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
        {
            return field.Value.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
        {
            var database = Factory.GetDatabase(field.Item.DatabaseName);

            var value = field.Value.Value;
            if (value.IndexOf('|') < 0)
            {
                var item = database.GetItem(value);
                if (item == null)
                {
                    throw new RetryableEmitException(Texts.Item_not_found, field.Value.Source ?? TextNode.Empty, value);
                }

                return item.ID.ToString();
            }

            var sb = new StringBuilder();
            foreach (var itemPath in value.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = database.GetItem(itemPath);
                if (item == null)
                {
                    throw new RetryableEmitException(Texts.Item_not_found, field.Value.Source ?? TextNode.Empty, itemPath);
                }

                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append(item.ID);
            }

            return sb.ToString();
        }
    }
}
