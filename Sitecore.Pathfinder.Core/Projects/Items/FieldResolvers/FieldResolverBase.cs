// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    public abstract class FieldResolverBase : IFieldResolver
    {
        protected FieldResolverBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanResolve(Field field);

        public abstract string Resolve(Field field);
    }
}
