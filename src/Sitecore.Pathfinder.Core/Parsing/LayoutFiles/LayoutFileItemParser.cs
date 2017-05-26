// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.LayoutFiles
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class LayoutFileItemParser : PipelineProcessorBase<ItemParserPipeline>
    {
        public const string LayoutFile = "Layout.File";

        public LayoutFileItemParser() : base(1000)
        {
        }

        protected override void Process(ItemParserPipeline pipeline)
        {
            var layoutFileTextNode = pipeline.TextNode.GetAttribute(LayoutFile);
            if (layoutFileTextNode == null || string.IsNullOrEmpty(layoutFileTextNode.Value))
            {
                return;
            }

            var sourcePropertyBag = (ISourcePropertyBag)pipeline.Item;

            var layoutFileProperty = sourcePropertyBag.GetSourceProperty<string>(LayoutFile) ?? sourcePropertyBag.NewSourceProperty(LayoutFile, string.Empty);
            layoutFileProperty.SetValue(layoutFileTextNode);

            var fieldValue = layoutFileProperty.GetValue();
            if (!fieldValue.StartsWith("~/"))
            {
                pipeline.Context.ParseContext.Trace.TraceWarning(Msg.P1016, Texts.File_path_must_start_with____, TraceHelper.GetTextNode(layoutFileProperty), fieldValue);
            }
        }
    }
}
