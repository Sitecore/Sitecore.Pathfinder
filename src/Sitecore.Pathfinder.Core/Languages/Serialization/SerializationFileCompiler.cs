// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Text;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    public class SerializationFileCompiler : CompilerBase
    {
        public SerializationFileCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is SerializationFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var serializationFile = projectItem as SerializationFile;
            Assert.Cast(serializationFile, nameof(serializationFile));

            var textDocument = (ITextSnapshot)serializationFile.Snapshots.First();
            var rootTextNode = textDocument.Root;
            if (rootTextNode == TextNode.Empty)
            {
                context.Trace.TraceError(Msg.C1050, Texts.Document_is_not_valid, textDocument.SourceFile.AbsoluteFileName, TextSpan.Empty);
                return;
            }

            var lines = textDocument.SourceFile.ReadAsLines();
            var itemBuilder = context.Factory.ItemBuilder();

            ParseItem(context, textDocument, itemBuilder, lines);

            var item = itemBuilder.Build(serializationFile.Project, rootTextNode);
            item.IsEmittable = false;

            var addedItem = serializationFile.Project.AddOrMerge(item);
            serializationFile.SerializationItemUri = addedItem.Uri;
        }

        protected virtual TextSpan GetTextSpan(int lineNumber, int linePosition, int length)
        {
            return new TextSpan(lineNumber + 1, linePosition, length);
        }

        protected virtual int ParseField([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull] LanguageVersionBuilder languageVersionBuilder, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
        {
            var textNode = context.Factory.TextNode(textSnapshot, GetTextSpan(lineNumber, 0, 0), string.Empty, string.Empty);

            var fieldBuilder = context.Factory.FieldBuilder().With(itemBuilder, textNode);
            itemBuilder.Fields.Add(fieldBuilder);

            fieldBuilder.Language = languageVersionBuilder.Language;
            fieldBuilder.LanguageTextNode = languageVersionBuilder.LanguageTextNode;

            fieldBuilder.Version = languageVersionBuilder.Version;
            fieldBuilder.VersionTextNode = languageVersionBuilder.VersionTextNode;

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
                        fieldBuilder.FieldId = value;
                        fieldBuilder.FieldIdTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(lineNumber, 0, lineLength), "field", value);
                        break;
                    case "name":
                        fieldBuilder.FieldName = value;
                        fieldBuilder.FieldNameTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(lineNumber, 0, lineLength), "name", value);
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
                fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(startIndex, 0, contentLength), string.Empty, value);
                return n - 1;
            }

            value = sb.ToString().Trim().TrimEnd('\n', '\r');
            fieldBuilder.Value = value;
            fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(startIndex, 0, contentLength), string.Empty, value);
            return lines.Length;
        }

        protected virtual int ParseItem([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull] [ItemNotNull] string[] lines)
        {
            var languageVersionBuilder = new LanguageVersionBuilder();

            for (var n = 0; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "----field----")
                {
                    n = ParseField(context, textSnapshot, itemBuilder, languageVersionBuilder, lines, n + 1);
                    continue;
                }

                if (line == "----version----")
                {
                    n = ParseVersion(context, textSnapshot, languageVersionBuilder, lines, n + 1);
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
                        itemBuilder.ItemNameTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(n, 0, line.Length), "name", value);
                        break;
                    case "master":
                        break;
                    case "template":
                        itemBuilder.TemplateIdOrPath = value;
                        itemBuilder.TemplateIdOrPathTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(n, 0, line.Length), "template", value);
                        break;
                    case "templatekey":
                        break;
                    case "version":
                        break;
                }
            }

            return lines.Length;
        }

        protected virtual int ParseVersion([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] LanguageVersionBuilder languageVersionBuilder, [NotNull] [ItemNotNull] string[] lines, int lineNumber)
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
                        languageVersionBuilder.Language = value;
                        languageVersionBuilder.LanguageTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(n, 0, line.Length), "language", value);
                        break;
                    case "version":
                        languageVersionBuilder.Version = int.Parse(value);
                        languageVersionBuilder.VersionTextNode = context.Factory.TextNode(textSnapshot, GetTextSpan(n, 0, line.Length), "version", value);
                        break;
                    case "revision":
                        break;
                }
            }

            return lines.Length;
        }
    }
}
