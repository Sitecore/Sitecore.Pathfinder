// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.HtmlFile.HtmlFiles
{
    public class HtmlFileTemplateParser : PipelineProcessorBase<TemplateParserPipeline>
    {
        public HtmlFileTemplateParser() : base(1000)
        {
        }

        protected override void Process(TemplateParserPipeline pipeline)
        {
            var htmlFileTextNode = pipeline.TextNode.GetAttribute(HtmlFilePropertyCompiler.LayoutHtmlfile);
            if (htmlFileTextNode == null || string.IsNullOrEmpty(htmlFileTextNode.Value))
            {
                return;
            }

            var template = pipeline.Template;

            var standardValuesItem = template.StandardValuesItem;
            if (standardValuesItem == null)
            {
                return;
            }

            var htmlFileProperty = standardValuesItem.GetProperty<string>(HtmlFilePropertyCompiler.LayoutHtmlfile) ?? standardValuesItem.NewProperty(HtmlFilePropertyCompiler.LayoutHtmlfile, string.Empty);
            htmlFileProperty.SetValue(htmlFileTextNode);

            var fieldValue = htmlFileProperty.GetValue();
            if (!fieldValue.StartsWith("~/"))
            {
                pipeline.Context.ParseContext.Trace.TraceWarning(Msg.P1016, Texts.File_path_must_start_with____, TraceHelper.GetTextNode(htmlFileProperty), fieldValue);
            }
        }
    }
}
