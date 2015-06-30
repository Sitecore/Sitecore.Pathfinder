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
            pipeline.Item.LayoutHtmlFile.Parse(pipeline.TextNode);
            if (string.IsNullOrEmpty(pipeline.Item.LayoutHtmlFile.Value))
            {
                return;
            }

            var field = pipeline.Context.ParseContext.Factory.Field(pipeline.Item, "__Renderings", pipeline.Item.LayoutHtmlFile?.Value);
            field.ValueHint.SetValue("HtmlTemplate");
            pipeline.Item.Fields.Add(field);
        }
    }
}
