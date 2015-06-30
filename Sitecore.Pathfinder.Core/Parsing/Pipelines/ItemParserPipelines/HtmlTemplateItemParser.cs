// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.References;

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

            var fieldValue = pipeline.Item.LayoutHtmlFile.Value;
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

            var field = pipeline.Context.ParseContext.Factory.Field(pipeline.Item, "__Renderings", fieldValue);
            field.ValueHint.SetValue("HtmlTemplate");
            pipeline.Item.Fields.Add(field);

            pipeline.Item.References.Add(pipeline.Context.ParseContext.Factory.FileReference(pipeline.Item, pipeline.Item.LayoutHtmlFile, fieldValue));
        }
    }
}
