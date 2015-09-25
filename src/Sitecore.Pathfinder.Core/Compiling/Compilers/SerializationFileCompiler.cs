// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    [Export(typeof(ICompiler))]
    public class SerializationFileCompiler : CompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is SerializationFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var serializationFile = projectItem as SerializationFile;
            if (serializationFile == null)
            {
                return;
            }

            var textDocument = (ITextSnapshot)serializationFile.Snapshots.First();
            var rootTextNode = textDocument.Root;
            if (rootTextNode == TextNode.Empty)
            {
                context.Trace.TraceError(Texts.Document_is_not_valid, textDocument.SourceFile.FileName, TextSpan.Empty);
                return;
            }

            var lines = textDocument.SourceFile.ReadAsLines();
            var itemBuilder = context.Factory.ItemBuilder();

            ParseItem(context, textDocument, itemBuilder, lines, 0);

            var item = itemBuilder.Build(serializationFile.Project, rootTextNode);
            item.IsEmittable = false;

            var addedItem = serializationFile.Project.AddOrMerge(item);
            serializationFile.SerializationItemUri = addedItem.Uri;
        }

        protected virtual int ParseField([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull] VersionContext versionContext, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
        {
            var textNode = context.Factory.TextNode(textSnapshot, new TextSpan(lineNumber, 0, 0), string.Empty, string.Empty, null);

            var fieldBuilder = context.Factory.FieldBuilder().With(itemBuilder, textNode);
            itemBuilder.Fields.Add(fieldBuilder);

            fieldBuilder.Language = versionContext.Language;
            fieldBuilder.LanguageTextNode = versionContext.LanguageTextNode;

            fieldBuilder.Version = versionContext.Version;
            fieldBuilder.VersionTextNode = versionContext.VersionTextNode;

            var lineLength = 0;

            int n;
            for (n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                lineLength += line.Length;

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    break;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "field":
                        break;
                    case "name":
                        fieldBuilder.FieldName = value;
                        fieldBuilder.FieldNameTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(lineNumber, 0, lineLength), "name", value, null);
                        break;
                    case "key":
                        break;
                }

                if (name == "content-length")
                {
                    var contentLength = int.Parse(value);
                    n = ParseFieldValue(context, textSnapshot, lines, fieldBuilder, n + 2, contentLength, ref lineLength);
                    break;
                }
            }

            return n;
        }

        protected virtual int ParseFieldValue([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] [ItemNotNull] string[] lines, [NotNull] FieldBuilder fieldBuilder, int startIndex, int contentLength, ref int lineLength)
        {
            string value;
            var sb = new StringBuilder();

            for (var n = startIndex; n < lines.Length; n++)
            {
                var line = lines[n];
                lineLength += line.Length;

                if (sb.Length < contentLength)
                {
                    sb.Append(line);
                    sb.Append("\r\n");
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                value = sb.ToString().Trim().TrimEnd('\n', '\r');
                fieldBuilder.Value = value;
                fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(startIndex, 0, contentLength), string.Empty, value, null);
                return n - 1;
            }

            value = sb.ToString().Trim().TrimEnd('\n', '\r');
            fieldBuilder.Value = value;
            fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(startIndex, 0, contentLength), string.Empty, value, null);
            return lines.Length;
        }

        protected virtual int ParseItem([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
        {
            var versionContext = new VersionContext();

            for (var n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "----field----")
                {
                    n = ParseField(context, textSnapshot, itemBuilder, versionContext, lines, n + 1);
                    continue;
                }

                if (line == "----version----")
                {
                    n = ParseVersion(context, textSnapshot, versionContext, lines, n + 1);
                    continue;
                }

                if (line == "----item----")
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    return n;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "id":
                        itemBuilder.Guid = value;
                        break;
                    case "database":
                        itemBuilder.DatabaseName = value;
                        break;
                    case "path":
                        itemBuilder.ItemIdOrPath = value;
                        break;
                    case "parent":
                        break;
                    case "name":
                        itemBuilder.ItemName = value;
                        itemBuilder.ItemNameTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(n, 0, line.Length), "name", value, null);
                        break;
                    case "master":
                        break;
                    case "template":
                        itemBuilder.TemplateIdOrPath = value;
                        itemBuilder.TemplateIdOrPathTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(n, 0, line.Length), "template", value, null);
                        break;
                    case "templatekey":
                        break;
                    case "version":
                        break;
                }
            }

            return lines.Length;
        }

        protected virtual int ParseVersion([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] VersionContext versionContext, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
        {
            for (var n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    return n - 1;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "language":
                        versionContext.Language = value;
                        versionContext.LanguageTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(n, 0, line.Length), "language", value, null);
                        break;
                    case "version":
                        versionContext.Version = int.Parse(value);
                        versionContext.VersionTextNode = context.Factory.TextNode(textSnapshot, new TextSpan(n, 0, line.Length), "version", value, null);
                        break;
                    case "revision":
                        break;
                }
            }

            return lines.Length;
        }

        protected class VersionContext
        {
            [NotNull]
            public string Language { get; set; } = string.Empty;

            [NotNull]
            public ITextNode LanguageTextNode { get; set; } = TextNode.Empty;

            public int Version { get; set; }

            [NotNull]
            public ITextNode VersionTextNode { get; set; } = TextNode.Empty;
        }
    }
}
