// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileEmitter : EmitterBase
    {
        public UnicornFileEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {

        }
    }
}
