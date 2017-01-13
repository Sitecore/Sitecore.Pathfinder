// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting.ItemsAndFilesEmitting;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Emitting.FileAndSqlEmitting
{
    public class SqlItemEmitter : EmitterBase
    {
        public SqlItemEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is Item && context.ProjectEmitter is ItemsAndFilesProjectEmitter;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            if (item.IsImport)
            {
                return;
            }

            // emit items, that are created from templates in the project, even if they are marked as not emittable
            var templateId = item.Template.Uri.Guid;
            var isTemplatePart = templateId == Constants.Templates.Template || templateId == Constants.Templates.TemplateSection || templateId == Constants.Templates.TemplateField;

            // emit missing parent items
            var isMissingParent = ((ISourcePropertyBag)item).GetValue<string>("__origin_reason") == nameof(CreateMissingParentItems);

            if (!isMissingParent && !isTemplatePart && !item.IsEmittable)
            {
                return;
            }

            var projectEmitter = context.ProjectEmitter as ItemsAndFilesProjectEmitter;
            Assert.Cast(projectEmitter, nameof(projectEmitter));

            context.Trace.TraceInformation(Msg.I1011, "Installing item", item.Paths.Path);

            projectEmitter.WriteItem(item);
        }
    }
}
