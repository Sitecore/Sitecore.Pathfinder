// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

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
            return field.Value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase) >= 0 && !field.Item.IsImport;
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var value = field.Value.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (value.IndexOf('|') >= 0)
            {
                return CompilePipeList(context, field, value);
            }

            if (value.IndexOf('&') >= 0)
            {
                return CompileUrlString(context, field, value);
            }

            if (value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
            {
                return CompileDirectLink(context, field, value);
            }

            return CompileInlineValue(context, field, value);
        }

        [NotNull]
        private string CompileDirectLink([NotNull] IFieldCompileContext context, [NotNull] Field field, [NotNull] string value)
        {
            if (!PathHelper.IsProbablyItemPath(value))
            {
                return value;
            }

            var item = field.Item.Project.FindQualifiedItem<IProjectItem>(value);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                return value;
            }

            return item.Uri.Guid.Format();
        }

        [NotNull]
        private string CompileInlineValue([NotNull] IFieldCompileContext context, [NotNull] Field field, [NotNull] string value)
        {
            var start = value.IndexOf("/sitecore", StringComparison.OrdinalIgnoreCase);
            var end = value.IndexOf(',', start);
            if (end < 0)
            {
                end = value.IndexOf('|', start);
            }

            if (end < 0)
            {
                end = value.IndexOf('&', start);
            }

            if (end < 0)
            {
                end = value.IndexOf(';', start);
            }

            if (end < 0)
            {
                end = value.Length;
            }

            var qualifiedName = value.Mid(start, end - start);

            if (!PathHelper.IsProbablyItemPath(value))
            {
                return value;
            }

            var item = field.Item.Project.FindQualifiedItem<IProjectItem>(qualifiedName);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
                return value;
            }

            return value.Left(start) + item.Uri.Guid.Format() + value.Mid(end);
        }

        [NotNull]
        private string CompilePipeList([NotNull] IFieldCompileContext context, [NotNull] Field field, [NotNull] string value)
        {
            var sb = new StringBuilder();
            foreach (var itemPath in value.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var path = itemPath;

                if (PathHelper.IsProbablyItemPath(path))
                {
                    var i = field.Item.Project.FindQualifiedItem<IProjectItem>(path);
                    if (i != null)
                    {
                        path = i.Uri.Guid.Format();
                    }
                    else
                    {
                        context.Trace.TraceError(Msg.C1046, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), path);
                    }
                }

                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append(path);
            }

            return sb.ToString();
        }

        [NotNull]
        private string CompileUrlString([NotNull] IFieldCompileContext context, [NotNull] Field field, [NotNull] string value)
        {
            var url = new UrlString(value);
            var result = new UrlString();

            foreach (string key in url.Parameters)
            {
                var v = url.Parameters[key];

                if (PathHelper.IsProbablyItemPath(v))
                {
                    var i = field.Item.Project.FindQualifiedItem<IProjectItem>(v);
                    if (i != null)
                    {
                        v = i.Uri.Guid.Format();
                    }
                    else
                    {
                        context.Trace.TraceError(Msg.C1046, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), v);
                    }
                }

                result[key] = v;
            }

            return result.ToString();
        }
    }
}
