// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class ItemParser : ParserBase
    {
        [NotNull, ItemNotNull]
        private static readonly string[] FileExtensions =
        {
            ".item.xml",
            ".content.xml",
            ".layout.xml",
            ".item.json",

            // ".content.json",
            ".layout.json",
            ".item.yaml",
            ".content.yaml",
            ".layout.yaml"
        };

        [ImportingConstructor]
        public ItemParser([NotNull] ISchemaService schemaService, [ImportMany, NotNull, ItemNotNull] IEnumerable<ITextNodeParser> textNodeParsers) : base(Constants.Parsers.Items)
        {
            SchemaService = schemaService;
            TextNodeParsers = textNodeParsers;
        }

        [NotNull]
        protected ISchemaService SchemaService { get; }

        [NotNull, ItemNotNull]
        public IEnumerable<ITextNodeParser> TextNodeParsers { get; }

        public override bool CanParse(IParseContext context)
        {
            var fileName = context.Snapshot.SourceFile.AbsoluteFileName;
            return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) && context.Snapshot is ITextSnapshot;
        }

        public override void Parse(IParseContext context)
        {
            var textSnapshot = context.Snapshot as ITextSnapshot;
            Assert.Cast(textSnapshot, nameof(textSnapshot));

            var textNode = textSnapshot.Root;
            if (textNode == TextNode.Empty)
            {
                var textSpan = textSnapshot.ParseErrorTextSpan != TextSpan.Empty ? textSnapshot.ParseErrorTextSpan : textSnapshot.Root.TextSpan;
                var text = !string.IsNullOrEmpty(textSnapshot.ParseError) ? textSnapshot.ParseError : Texts.Source_file_is_empty;
                context.Trace.TraceWarning(Msg.P1009, text, textSnapshot.SourceFile.AbsoluteFileName, textSpan);
                return;
            }

            if (!SchemaService.ValidateSnapshotSchema(context, textSnapshot))
            {
                return;
            }

            var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
            var itemParseContext = context.Factory.ItemParseContext(context, this, context.DatabaseName, parentItemPath, false);

            ParseTextNode(itemParseContext, textNode);
        }

        public virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
        {
            foreach (var childNode in textNode.ChildNodes)
            {
                ParseTextNode(context, childNode);
            }
        }

        public virtual void ParseTextNode([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
        {
            try
            {
                var parsed = false;

                foreach (var textNodeParser in TextNodeParsers.OrderBy(p => p.Priority))
                {
                    if (!textNodeParser.CanParse(context, textNode))
                    {
                        continue;
                    }

                    parsed = true;

                    if (SchemaService.ValidateTextNodeSchema(textNode))
                    {
                        textNodeParser.Parse(context, textNode);
                    }

                    break;
                }

                if (!parsed)
                {
                    context.ParseContext.Trace.TraceError(Msg.P1025, "Unknown text node", textNode, textNode.Key);

                }
            }
            catch (Exception ex)
            {
                context.ParseContext.Trace.TraceError(Msg.P1004, string.Empty, context.ParseContext.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty, ex.Message);
            }
        }
    }
}
