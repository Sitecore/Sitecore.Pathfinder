using System;

namespace Sitecore.Patfhfinder.Diagnostics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LocalizableAttribute: Attribute
    {
        public LocalizableAttribute(bool flag)
        {
        }
    }
}
