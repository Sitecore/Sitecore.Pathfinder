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
        [NotNull]
        [ItemNotNull]
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
        public ItemParser([ImportMany] [NotNull] [ItemNotNull] IEnumerable<ITextNodeParser> textNodeParsers) : base(Constants.Parsers.Items)
        {
            TextNodeParsers = textNodeParsers;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<ITextNodeParser> TextNodeParsers { get; }

        public override bool CanParse(IParseContext context)
        {
            var fileName = context.Snapshot.SourceFile.AbsoluteFileName;
            return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) && context.Snapshot is ITextSnapshot;
        }

        public override void Parse(IParseContext context)
        {
            var textDocument = (ITextSnapshot)context.Snapshot;

            var textNode = textDocument.Root;
            if (textNode == TextNode.Empty)
            {
                var textSpan = textDocument.ParseErrorTextSpan != TextSpan.Empty ? textDocument.ParseErrorTextSpan : textDocument.Root.TextSpan;
                var text = !string.IsNullOrEmpty(textDocument.ParseError) ? textDocument.ParseError : Texts.Source_file_is_empty;
                context.Trace.TraceWarning(text, textDocument.SourceFile.AbsoluteFileName, textSpan);
                return;
            }

            textDocument.ValidateSchema(context);

            var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
            var itemParseContext = context.Factory.ItemParseContext(context, this, context.DatabaseName, parentItemPath);

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
                foreach (var textNodeParser in TextNodeParsers.OrderBy(p => p.Priority))
                {
                    if (textNodeParser.CanParse(context, textNode))
                    {
                        textNodeParser.Parse(context, textNode);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                context.ParseContext.Trace.TraceError(string.Empty, context.ParseContext.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty, ex.Message);
            }
        }
    }
}
