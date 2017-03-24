// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Emitting.Emitters.SitecorePackageEmitter;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitting.Emitters.DirectoryEmitter
{
    [Export(typeof(IEmitter)), Shared]
    public class ItemEmitter : EmitterBase
    {
        public ItemEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return context.ProjectEmitter is DirectoryProjectEmitter && projectItem is Item;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var item = (Item)projectItem;
            var sourcePropertyBag = (ISourcePropertyBag)item;
            var projectEmitter = (DirectoryProjectEmitter)context.ProjectEmitter;

            if (item.IsEmittable || sourcePropertyBag.GetValue<string>("__origin_reason") == nameof(CreateItemsFromTemplates))
            {
                projectEmitter.EmitItem(context, item);
            }
        }
    }
}
