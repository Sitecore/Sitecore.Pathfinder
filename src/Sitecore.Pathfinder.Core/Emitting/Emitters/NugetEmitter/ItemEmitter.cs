// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Emitting.Emitters.NugetEmitter
{
    [Export(typeof(IEmitter)), Shared]
    public class ItemEmitter : EmitterBase
    {
        public ItemEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return context.ProjectEmitter is NugetProjectEmitter && projectItem is Item;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var item = (Item)projectItem;
            var projectEmitter = (NugetProjectEmitter)context.ProjectEmitter;

            if (item.IsEmittable)
            {
                projectEmitter.EmitItem(context, item);
            }
        }
    }
}
