// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Checking.Conventions
{
    public class ConventionRuleContext : IRuleContext
    {
        public ConventionRuleContext([NotNull] ITraceService trace, [NotNull] object obj)
        {
            Trace = trace;
            Object = obj;
        }

        public bool IsAborted { get; set; }

        public object Object { get; }

        public ITraceService Trace { get; }
    }
}
