// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    public abstract class EmitterBase : IEmitter
    {
        protected EmitterBase(double sortorder)
        {
            Sortorder = sortorder;
        }

        public double Sortorder { get; }

        public abstract bool CanEmit(IEmitContext context, IProjectItem projectItem);

        public abstract void Emit(IEmitContext context, IProjectItem projectItem);
    }
}
