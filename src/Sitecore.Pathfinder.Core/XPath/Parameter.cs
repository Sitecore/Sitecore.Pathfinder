// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class Parameter : ElementBase
    {
        public Parameter([NotNull] string name) : base(name)
        {
        }

        public override object Evaluate(Query query, object context)
        {
            object result;
            if (query.Parameters.TryGetValue(Name, out result))
            {
                return result;
            }

            return null;
        }
    }
}
