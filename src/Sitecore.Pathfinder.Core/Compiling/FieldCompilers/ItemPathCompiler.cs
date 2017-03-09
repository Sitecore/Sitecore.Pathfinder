// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class ItemPathCompiler : FieldCompilerBase
    {
        [ImportingConstructor]
        public ItemPathCompiler([NotNull] IReferenceParserService referenceParser) : base(Constants.FieldCompilers.Normal + 10)
        {
            ReferenceParser = referenceParser;
        }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

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

            if (ReferenceParser.IsIgnoredReference(value))
            {
                return value;
            }

            var item = field.Item.Project.FindQualifiedItem<IProjectItem>(value);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), value);
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

            if (ReferenceParser.IsIgnoredReference(value))
            {
                return value;
            }

            var item = field.Item.Project.FindQualifiedItem<IProjectItem>(qualifiedName);
            if (item == null)
            {
                context.Trace.TraceError(Msg.C1045, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), value);
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

                if (PathHelper.IsProbablyItemPath(path) && !ReferenceParser.IsIgnoredReference(value))
                {
                    var i = field.Item.Project.FindQualifiedItem<IProjectItem>(path);
                    if (i != null)
                    {
                        path = i.Uri.Guid.Format();
                    }
                    else
                    {
                        context.Trace.TraceError(Msg.C1046, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), path);
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
            Database database = null;

            var project = field.Item.Project;

            foreach (string key in url.Parameters.Keys)
            {
                if (string.Equals(key, "database", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "databasename", StringComparison.OrdinalIgnoreCase))
                {
                    database = project.GetDatabase(url.Parameters[key]);
                }
            }

            foreach (string key in url.Parameters)
            {
                var v = url.Parameters[key];

                if (PathHelper.IsProbablyItemPath(v) && !ReferenceParser.IsIgnoredReference(v))
                {
                    var i = database == null ? project.FindQualifiedItem<IProjectItem>(v) : project.FindQualifiedItem<DatabaseProjectItem>(database, v);
                    if (i != null)
                    {
                        v = i.Uri.Guid.Format();
                    }
                    else
                    {
                        if (database != null && !string.Equals(database.DatabaseName, field.Item.DatabaseName, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Trace.TraceWarning(Msg.C1064, Texts.Item_path_reference_not_found__but_may_be_in_another_database, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), database.DatabaseName + ":/" + v);
                        }
                        else
                        {
                            context.Trace.TraceError(Msg.C1046, Texts.Item_path_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty, field), v);
                        }
                    }
                }

                result[key] = v;
            }

            return result.ToString();
        }
    }
}
