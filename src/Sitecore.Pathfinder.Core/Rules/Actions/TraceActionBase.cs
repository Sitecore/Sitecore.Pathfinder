// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public abstract class TraceActionBase : ActionBase
    {
        protected TraceActionBase([NotNull] ITraceService trace, [NotNull] string name) : base(name)
        {
            Trace = trace;
        }

        [NotNull]
        protected ITraceService Trace { get; }

        public override void Execute(IRuleContext context, IDictionary<string, object> parameters)
        {
            var text = GetParameterValue(parameters, "text", context.Object);
            if (string.IsNullOrEmpty(text))
            {
                text = GetParameterValue(parameters, "value", context.Object) ?? string.Empty;
            }

            var details = GetParameterValue(parameters, "details", context.Object);

            int msg;
            int.TryParse(GetParameterValue(parameters, "msg", context.Object), out msg);

            ITextNode textNode = null;
            ISnapshot snapshot = null;

            var itemBase = context.Object as DatabaseProjectItem;
            if (itemBase != null)
            {
                textNode = itemBase.SourceTextNodes.FirstOrDefault();
            }

            var projectItem = context.Object as IProjectItem;
            if (projectItem != null)
            {
                snapshot = projectItem.Snapshots.FirstOrDefault();
            }

            TraceLine(msg, text, textNode, snapshot, details ?? string.Empty);
        }

        protected abstract void TraceLine(int msg, [NotNull] string text, [CanBeNull] ITextNode textNode, [CanBeNull] ISnapshot snapshot, [NotNull] string details);
    }
}
