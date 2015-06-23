// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    public abstract class FieldResolverBase : IFieldResolver
    {
        protected FieldResolverBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanResolve(IEmitContext context, TemplateField templateField, Field field);

        public abstract string Resolve(IEmitContext context, TemplateField templateField, Field field);
    }
}
