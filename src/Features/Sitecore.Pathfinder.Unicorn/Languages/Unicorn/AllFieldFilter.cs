using System;
using Rainbow.Filtering;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    internal class AllFieldFilter : IFieldFilter
    {
        public bool Includes(Guid fieldId)
        {
            return true;
        }
    }
}