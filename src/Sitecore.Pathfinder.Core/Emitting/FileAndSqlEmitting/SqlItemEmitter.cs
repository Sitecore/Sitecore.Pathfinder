// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
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
            return projectItem is Item && context.ProjectEmitter is FileAndSqlProjectEmitter;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            if (!item.IsEmittable || item.IsImport)
            {
                return;
            }
        }
    }
}
