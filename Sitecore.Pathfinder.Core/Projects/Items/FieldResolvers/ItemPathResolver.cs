// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class ItemPathResolver : FieldResolverBase
    {
        public ItemPathResolver() : base(Constants.FieldResolvers.Normal + 10)
        {
        }

        public override bool CanResolve(Field field)
        {
            return field.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string Resolve(Field field)
        {
            var value = field.Value.Trim();
            if (value.IndexOf('|') < 0)
            {
                var item = field.Item.Project.FindQualifiedItem(value);
                if (item == null)
                {
                    field.WriteDiagnostic(Severity.Error, "Item path reference not found", value);
                    return string.Empty;
                }

                return item.Uri.Guid.Format();
            }

            var sb = new StringBuilder();
            foreach (var itemPath in value.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = field.Item.Project.FindQualifiedItem(itemPath);
                if (item == null)
                {
                    field.WriteDiagnostic(Severity.Error, "Item path reference not found", itemPath);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('|');
                    }

                    sb.Append(item.Uri.Guid.Format());
                }
            }

            return sb.ToString();
        }
    }
}
