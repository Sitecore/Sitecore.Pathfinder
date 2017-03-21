using System;

namespace Sitecore.Pathfinder.Diagnostics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LocalizableAttribute: Attribute
    {
        public LocalizableAttribute(bool flag)
        {
        }
    }
}
