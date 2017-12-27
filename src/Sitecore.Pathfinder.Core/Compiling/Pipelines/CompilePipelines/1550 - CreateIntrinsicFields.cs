// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    [Export(typeof(IPipelineProcessor)), Shared]
    public class CreateIntrinsicFields : PipelineProcessorBase<CompilePipeline>
    {
        [ImportingConstructor]
        public CreateIntrinsicFields([NotNull] IFactory factory) : base(1550)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        protected virtual void CreateIconField([NotNull] Item item)
        {
            if (string.IsNullOrEmpty(item.Icon))
            {
                return;
            }

            var iconField = item.Fields.GetField(Constants.Fields.Icon);
            if (iconField != null)
            {
                iconField.Value = item.Icon;
                return;
            }

            iconField = Factory.Field(item, Constants.FieldNames.Icon, item.Icon);
            iconField.FieldId = Constants.Fields.Icon;

            item.Fields.Add(iconField);
        }

        protected virtual void CreateSortorderField([NotNull] Item item)
        {
            if (item.Sortorder == 0)
            {
                return;
            }

            var sortorderField = item.Fields.GetField(Constants.Fields.Sortorder);
            if (sortorderField != null)
            {
                sortorderField.Value = item.Sortorder.ToString();
                return;
            }

            sortorderField = Factory.Field(item, Constants.FieldNames.SortOrder, item.Sortorder.ToString());
            sortorderField.FieldId = Constants.Fields.Sortorder;

            item.Fields.Add(sortorderField);
        }

        protected override void Process(CompilePipeline pipeline)
        {
            foreach (var item in pipeline.Context.Project.Items)
            {
                CreateIconField(item);
                CreateSortorderField(item);
            }
        }
    }
}
                       