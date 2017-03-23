// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Pathfinder.Diagnostics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LocalizableAttribute : Attribute
    {
        public LocalizableAttribute(bool flag)
        {
        }
    }
}
