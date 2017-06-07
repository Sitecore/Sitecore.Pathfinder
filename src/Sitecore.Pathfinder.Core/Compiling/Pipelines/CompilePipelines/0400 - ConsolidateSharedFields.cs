// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class ConsolidateSharedFields : PipelineProcessorBase<CompilePipeline>
    {
        public const int ConsolidateSharedFieldsPriority = 400;

        [ImportingConstructor]
        public ConsolidateSharedFields([NotNull] ITraceService trace) : base(ConsolidateSharedFieldsPriority)
        {
            Trace = trace;
        }

        [NotNull]
        protected ITraceService Trace { get; }

        protected override void Process(CompilePipeline pipeline)
        {
            foreach (var item in pipeline.Context.Project.Items)
            {
                var fields = item.Fields.ToArray();

                for (var i = 0; i < fields.Length - 1; i++)
                {
                    for (var j = i + 1; j < fields.Length; j++)
                    {
                        var field1 = fields[i];
                        var field2 = fields[j];

                        if (field1.FieldId != field2.FieldId)
                        {
                            continue;
                        }

                        if (field1.TemplateField.Shared)
                        {
                            if (field1.Value != field2.Value)
                            {
                                Trace.TraceError(Msg.C1127, "Shared field appears multiple times with different values", field1.SourceTextNode, field1.FieldName);
                                break;
                            }

                            item.Fields.Remove(field1);
                            break;
                        }

                        if (field1.TemplateField.Unversioned && field1.Language == field2.Language)
                        {
                            if (field1.Value != field2.Value)
                            {
                                Trace.TraceError(Msg.C1128, "Unversioned field appears multiple times with different values", field1.SourceTextNode, field1.FieldName);
                                break;
                            }

                            item.Fields.Remove(field1);
                            break;
                        }
                    }
                }
            }
        }
    }
}
