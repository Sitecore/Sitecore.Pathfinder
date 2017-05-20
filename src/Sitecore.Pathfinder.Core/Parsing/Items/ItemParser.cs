// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    [Export(typeof(IParser)), Shared]
    public class ItemParser : ParserBase
    {
        [ImportingConstructor]
        public ItemParser([NotNull] IConfiguration configuration, [NotNull] ISchemaService schemaService, [ImportMany, NotNull, ItemNotNull] IEnumerable<ITextNodeParser> textNodeParsers) : base(Constants.Parsers.Items)
        {
            SchemaService = schemaService;
            TextNodeParsers = textNodeParsers;

            PathMatcher = new PathMatcher(configuration.GetString(Constants.Configuration.Items.Include), configuration.GetString(Constants.Configuration.Items.Exclude));
        }

        [NotNull, ItemNotNull]
        public IEnumerable<ITextNodeParser> TextNodeParsers { get; }

        [NotNull]
        protected PathMatcher PathMatcher { get; }

        [NotNull]
        protected ISchemaService SchemaService { get; }

        public override bool CanParse(IParseContext context)
        {
            var fileName = context.Snapshot.SourceFile.AbsoluteFileName;
            return context.Snapshot is ITextSnapshot && PathMatcher.IsMatch(fileName);
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
            var itemParseContext = context.Factory.ItemParseContext(context, this, context.Database, parentItemPath, false);

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
                    context.ParseContext.Trace.TraceError(Msg.P1025, Texts.Unknown_text_node, textNode, textNode.Key);
                }
            }
            catch (Exception ex)
            {
                var details = ex.Message;
                if (context.ParseContext.Configuration.GetBool(Constants.Configuration.System.ShowStackTrace))
                {
                    details += Environment.NewLine + ex.StackTrace;
                }

                context.ParseContext.Trace.TraceError(Msg.P1004, string.Empty, context.ParseContext.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty, details);
            }
        }
    }
}
