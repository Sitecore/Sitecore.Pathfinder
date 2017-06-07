using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration
{
    [AttributeUsage(AttributeTargets.Constructor), MeansImplicitUse]
    public class FactoryConstructorAttribute : Attribute
    {
        public FactoryConstructorAttribute()
        {
        }

        public FactoryConstructorAttribute([NotNull] Type returnType)
        {
            ReturnType = returnType;
        }

        [CanBeNull]
        public Type ReturnType { get; }
    }
}
