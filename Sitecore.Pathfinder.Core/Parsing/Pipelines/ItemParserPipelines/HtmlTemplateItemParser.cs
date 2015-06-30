// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines
{
    public class HtmlTemplateItemParser : PipelineProcessorBase<ItemParserPipeline>
    {
        public HtmlTemplateItemParser() : base(1000)
        {
        }

        protected override void Process(ItemParserPipeline pipeline)
        {
            pipeline.Item.LayoutHtmlFile = pipeline.TextNode.GetAttribute<string>("Layout.HtmlFile");
            if (string.IsNullOrEmpty(pipeline.Item.LayoutHtmlFile.Value))
            {
                return;
            }

            var field = pipeline.Context.ParseContext.Factory.Field(pipeline.Item, "__Renderings", string.Empty, 0, pipeline.Item.LayoutHtmlFile?.Value, "HtmlTemplate");
            pipeline.Item.Fields.Add(field);
        }
    }
}
