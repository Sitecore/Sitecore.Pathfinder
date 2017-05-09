// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
    public class OptionValuesAttribute : Attribute
    {
        public OptionValuesAttribute([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; }
    }
}
