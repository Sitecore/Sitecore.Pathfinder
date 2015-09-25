// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Text;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler))]
    public class ItemPathCompiler : FieldCompilerBase
    {
        public ItemPathCompiler() : base(Constants.FieldResolvers.Normal + 10)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return field.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (value.IndexOf('|') < 0)
            {
                var item = field.Item.Project.FindQualifiedItem(value);
                if (item == null)
                {
                    context.Trace.TraceError("Item path reference not found", value);
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
                    context.Trace.TraceError("Item path reference not found", itemPath);
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
