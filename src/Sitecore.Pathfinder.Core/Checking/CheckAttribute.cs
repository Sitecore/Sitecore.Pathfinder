// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Checking
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class CheckAttribute : Attribute
    {
    }
}
