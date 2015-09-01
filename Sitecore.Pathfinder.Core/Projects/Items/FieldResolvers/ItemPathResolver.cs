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
        public ItemPathResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(ITraceService trace, IProject project, Field field)
        {
            return field.Value.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string Resolve(ITraceService trace, IProject project, Field field)
        {
            var value = field.Value.Value;
            if (value.IndexOf('|') < 0)
            {
                var item = project.FindQualifiedItem(value);
                if (item == null)
                {
                    trace.Writeline("Item path reference not found", value);
                }

                return item.Guid.Format();
            }

            var sb = new StringBuilder();
            foreach (var itemPath in value.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = project.FindQualifiedItem(itemPath);
                if (item == null)
                {
                    trace.Writeline("Item path reference not found", itemPath);
                }

                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append(item.Guid.Format());
            }

            return sb.ToString();
        }
    }
}
