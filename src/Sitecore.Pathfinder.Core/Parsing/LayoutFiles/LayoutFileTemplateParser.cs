// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.Pipelines.TemplateParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.LayoutFiles
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class LayoutFileTemplateParser : PipelineProcessorBase<TemplateParserPipeline>
    {
        [NotNull]
        protected ITraceService Trace { get; }

        [ImportingConstructor]
        public LayoutFileTemplateParser([NotNull] ITraceService trace) : base(1000)
        {
            Trace = trace;
        }

        protected override void Process(TemplateParserPipeline pipeline)
        {
            var layoutFileTextNode = pipeline.TextNode.GetAttribute(LayoutFileItemParser.LayoutFile);
            if (layoutFileTextNode == null || string.IsNullOrEmpty(layoutFileTextNode.Value))
            {
                return;
            }

            var template = pipeline.Template;

            var standardValuesItem = template.StandardValuesItem;
            if (standardValuesItem == null)
            {
                return;
            }

            var layoutFileProperty = ((ISourcePropertyBag)standardValuesItem).GetSourceProperty<string>(LayoutFileItemParser.LayoutFile) ?? ((ISourcePropertyBag)standardValuesItem).NewSourceProperty(LayoutFileItemParser.LayoutFile, string.Empty);
            layoutFileProperty.SetValue(layoutFileTextNode);

            var fieldValue = layoutFileProperty.GetValue();
            if (!fieldValue.StartsWith("~/"))
            {
                Trace.TraceWarning(Msg.P1016, Texts.File_path_must_start_with____, TraceHelper.GetTextNode(layoutFileProperty), fieldValue);
            }
        }
    }
}
