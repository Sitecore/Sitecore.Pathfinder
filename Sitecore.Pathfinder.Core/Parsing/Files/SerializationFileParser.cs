// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Parsing.Files
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
            var root = textDocument.Root;
            if (root == TextNode.Empty)
            {
                context.Trace.TraceError(Texts.Document_is_not_valid, textDocument.SourceFile.FileName, TextPosition.Empty);
                return;
            }

            var lines = context.Snapshot.SourceFile.ReadAsLines();

            var serializationItemBuilder = new SerializationItemBuilder();
            ParseLines(context, serializationItemBuilder, lines, 0);

            var item = context.Factory.Item(context.Project, serializationItemBuilder.ProjectUniqueId, root, serializationItemBuilder.DatabaseName, serializationItemBuilder.ItemName, serializationItemBuilder.ItemIdOrPath, serializationItemBuilder.TemplateIdOrPath);
            item.ItemName.Source = serializationItemBuilder.ItemNameSource;
            item.TemplateIdOrPath.Source = serializationItemBuilder.TemplateIdOrPathSource;
            item.Icon.SetValue(serializationItemBuilder.Icon);
            item.IsEmittable = false;

            foreach (var field in serializationItemBuilder.Fields)
            {
                field.Item = item;
                item.Fields.Add(field);
            }

            context.Project.AddOrMerge(context, item);

            var serializationFile = context.Factory.SerializationFile(context.Project, context.Snapshot);
            context.Project.AddOrMerge(context, serializationFile);
        }

        protected virtual int ParseContent([NotNull] string[] lines, int startIndex, int contentLength, out string value, ref int lineLength)
        {
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

                if (!string.IsNullOrEmpty(line))
                {
                    value = sb.ToString().Trim().TrimEnd('\n', '\r');
                    return n - 1;
                }
            }

            value = sb.ToString().Trim().TrimEnd('\n', '\r');
            return lines.Length;
        }

        protected virtual int ParseField([NotNull] IParseContext context, [NotNull] SerializationItemBuilder serializationItemBuilder, [NotNull] string[] lines, int lineNumber, [NotNull] string language, int version)
        {
            var fieldName = string.Empty;
            var fieldValue = string.Empty;
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
                        fieldName = value;
                        break;
                    case "key":
                        break;
                }

                if (name == "content-length")
                {
                    var contentLength = int.Parse(value);
                    n = ParseContent(lines, n + 2, contentLength, out fieldValue, ref lineLength);
                    break;
                }
            }

            var field = context.Factory.Field(Item.Empty, fieldName, language, version, fieldValue);
            field.Value.Source = context.Factory.TextNode(serializationItemBuilder.ItemNameSource.Snapshot, new TextPosition(lineNumber, 0, lineLength), fieldName, fieldValue, serializationItemBuilder.ItemNameSource);

            serializationItemBuilder.Fields.Add(field);

            return n;
        }

        protected virtual int ParseLines([NotNull] IParseContext context, [NotNull] SerializationItemBuilder serializationItemBuilder, [NotNull] string[] lines, int lineNumber)
        {
            var language = string.Empty;
            var version = 0;

            for (var n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "----field----")
                {
                    n = ParseField(context, serializationItemBuilder, lines, n + 1, language, version);
                    continue;
                }

                if (line == "----version----")
                {
                    n = ParseVersion(lines, n + 1, ref language, ref version);
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
                        serializationItemBuilder.ProjectUniqueId = value;
                        break;
                    case "database":
                        serializationItemBuilder.DatabaseName = value;
                        break;
                    case "path":
                        serializationItemBuilder.ItemIdOrPath = value;
                        break;
                    case "parent":
                        break;
                    case "name":
                        serializationItemBuilder.ItemName = value;
                        serializationItemBuilder.ItemNameSource = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "name", value, null);
                        break;
                    case "master":
                        break;
                    case "template":
                        serializationItemBuilder.TemplateIdOrPath = value;
                        serializationItemBuilder.TemplateIdOrPathSource = context.Factory.TextNode(context.Snapshot, new TextPosition(n, 0, line.Length), "template", value, null);
                        break;
                    case "templatekey":
                        break;
                    case "version":
                        break;
                }
            }

            return lines.Length;
        }

        protected virtual int ParseVersion([NotNull] string[] lines, int lineNumber, [NotNull] ref string language, ref int version)
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
                        language = value;
                        break;
                    case "version":
                        version = int.Parse(value);
                        break;
                    case "revision":
                        break;
                }
            }

            return lines.Length;
        }

        protected class SerializationItemBuilder
        {
            [NotNull]
            public string DatabaseName { get; set; } = string.Empty;

            [NotNull]
            public List<Field> Fields { get; } = new List<Field>();

            [NotNull]
            public string Icon { get; set; } = string.Empty;

            [NotNull]
            public string ItemIdOrPath { get; set; } = string.Empty;

            [NotNull]
            public string ItemName { get; set; } = string.Empty;

            [NotNull]
            public ITextNode ItemNameSource { get; set; } = TextNode.Empty;

            [NotNull]
            public string ProjectUniqueId { get; set; } = string.Empty;

            [NotNull]
            public string TemplateIdOrPath { get; set; } = string.Empty;

            [NotNull]
            public ITextNode TemplateIdOrPathSource { get; set; } = TextNode.Empty;
        }
    }
}
