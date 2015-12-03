// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Text;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class ItemPathCompiler : FieldCompilerBase
    {
        public ItemPathCompiler() : base(Constants.FieldCompilers.Normal + 10)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            // if the value contains a dot (.) it is probably a file name
            return field.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0 && field.Value.IndexOf('.') < 0 && !field.Item.IsImport;
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (value.IndexOf('|') < 0)
            {
                var item = field.Item.Project.FindQualifiedItem<IProjectItem>(value);
                if (item == null)
                {
                    context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                    return string.Empty;
                }

                return item.Uri.Guid.Format();
            }

            var sb = new StringBuilder();
            foreach (var itemPath in value.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = field.Item.Project.FindQualifiedItem<IProjectItem>(itemPath);
                if (item == null)
                {
                    context.Trace.TraceError(Msg.C1046, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), itemPath);
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
