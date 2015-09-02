// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines
{
    public class HtmlTemplateItemParser : PipelineProcessorBase<ItemParserPipeline>
    {
        public HtmlTemplateItemParser() : base(1000)
        {
        }

        protected override void Process(ItemParserPipeline pipeline)
        {
            pipeline.Item.LayoutHtmlFileProperty.Parse(pipeline.TextNode);

            var fieldValue = pipeline.Item.LayoutHtmlFile;
            if (string.IsNullOrEmpty(fieldValue))
            {
                return;
            }

            /*
            if (!fieldValue.StartsWith("~/"))
            {
                pipeline.Context.ParseContext.Trace.TraceWarning(Texts.File_path_must_start_with____, pipeline.Item.LayoutHtmlFile.Source ?? pipeline.TextNode, fieldValue);
            }
            */

            var field = pipeline.Context.ParseContext.Factory.Field(pipeline.Item, TextNode.Empty, "__Renderings", fieldValue);
            field.ValueHintProperty.SetValue("HtmlTemplate");
            pipeline.Item.Fields.Add(field);

            pipeline.Item.References.Add(pipeline.Context.ParseContext.Factory.FileReference(pipeline.Item, pipeline.Item.LayoutHtmlFileProperty, fieldValue));
        }
    }
}
