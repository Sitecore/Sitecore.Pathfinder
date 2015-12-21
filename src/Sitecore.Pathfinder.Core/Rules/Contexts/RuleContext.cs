// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public class RuleContext : IRuleContext
    {
        public RuleContext([NotNull] ITraceService trace, [NotNull] object obj)
        {
            Trace = trace;
            Object = obj;
        }

        public bool IsAborted { get; set; }

        public object Object { get; }

        public ITraceService Trace { get; }
    }
}
