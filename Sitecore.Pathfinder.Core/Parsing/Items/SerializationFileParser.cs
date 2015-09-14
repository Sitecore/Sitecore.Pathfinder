// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.Builders;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    [Export(typeof(IParser))]
    public class SerializationFileParser : ParserBase
    {
        private const string FileExtension = ".item";

        public SerializationFileParser() : base(Constants.Parsers.ContentFiles)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            return context.Snapshot.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var textDocument = (ITextSnapshot)context.Snapshot;
            var rootTextNode = textDocument.Root;
            if (rootTextNode == TextNode.Empty)
            {
                context.Trace.TraceError(Texts.Document_is_not_valid, textDocument.SourceFile.FileName, TextPosition.Empty);
                return;
            }

            var lines = context.Snapshot.SourceFile.ReadAsLines();
            var itemBuilder = new ItemBuilder();

            ParseItem(context, itemBuilder, lines, 0);

            var item = itemBuilder.Build(context, rootTextNode);
            item.IsEmittable = false;

            context.Project.AddOrMerge(context, item);

            var serializationFile = context.Factory.SerializationFile(context.Project, context.Snapshot, item.Uri, context.FilePath);
            context.Project.AddOrMerge(context, serializationFile);
        }

        protected virtual int ParseField([NotNull] IParseContext context, [NotNull] ItemBuilder itemBuilder, [NotNull] VersionContext versionContext, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
        {
            var textNode = context.Factory.TextNode(context.Snapshot, new TextPosition(lineNumber, 0, 0), string.Empty, string.Empty, null);

            var fieldBuilder = new FieldBuilder(itemBuilder, textNode);
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
                        fieldBuilder.FieldNameTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(lineNumber, 0, lineLength), "name", value, null);
                        break;
                    case "key":
                        break;
                }

                if (name == "content-length")
                {
                    var contentLength = int.Parse(value);
                    n = ParseFieldValue(context, lines, fieldBuilder, n + 2, contentLength, ref lineLength);
                    break;
                }
            }

            return n;
        }

        protected virtual int ParseFieldValue([NotNull] IParseContext context, [NotNull] [ItemNotNull] string[] lines, [NotNull] FieldBuilder fieldBuilder, int startIndex, int contentLength, ref int lineLength)
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
                fieldBuilder.ValueTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(startIndex, 0, contentLength), string.Empty, value, null);
                return n - 1;
            }

            value = sb.ToString().Trim().TrimEnd('\n', '\r');
            fieldBuilder.Value = value;
            fieldBuilder.ValueTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(startIndex, 0, contentLength), string.Empty, value, null);
            return lines.Length;
        }

        protected virtual int ParseItem([NotNull] IParseContext context, [NotNull] ItemBuilder itemBuilder, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
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
                    n = ParseField(context, itemBuilder, versionContext, lines, n + 1);
                    continue;
                }

                if (line == "----version----")
                {
                    n = ParseVersion(context, versionContext, lines, n + 1);
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
                        itemBuilder.ItemNameTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "name", value, null);
                        break;
                    case "master":
                        break;
                    case "template":
                        itemBuilder.TemplateIdOrPath = value;
                        itemBuilder.TemplateIdOrPathTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "template", value, null);
                        break;
                    case "templatekey":
                        break;
                    case "version":
                        break;
                }
            }

            return lines.Length;
        }

        protected virtual int ParseVersion([NotNull] IParseContext context, [NotNull] VersionContext versionContext, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
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
                        versionContext.LanguageTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "language", value, null);
                        break;
                    case "version":
                        versionContext.Version = int.Parse(value);
                        versionContext.VersionTextNode = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "version", value, null);
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
