// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines
{
    public class ItemParserPipeline : PipelineBase<ItemParserPipeline>
    {
        [NotNull]
        public ItemParseContext Context { get; private set; }

        [NotNull]
        public Item Item { get; private set; } = Item.Empty;

        [NotNull]
        public ITextNode TextNode { get; private set; } = Snapshots.TextNode.Empty;

        public void Execute([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode)
        {
            Context = context;
            Item = item;
            TextNode = textNode;

            Execute();
        }
    }
}
