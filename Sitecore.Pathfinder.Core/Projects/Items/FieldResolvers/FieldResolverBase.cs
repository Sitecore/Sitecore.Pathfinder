// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    public abstract class FieldResolverBase : IFieldResolver
    {
        protected FieldResolverBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanResolve(ITraceService trace, IProject project, Field field);

        public abstract string Resolve(ITraceService trace, IProject project, Field field);
    }
}
