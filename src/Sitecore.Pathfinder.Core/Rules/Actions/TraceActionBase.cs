// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
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

        public override void Execute(IRuleContext context, IDictionary<string, string> parameters)
        {
            var text = parameters.GetString("text");
            if (string.IsNullOrEmpty(text))
            {
                text = parameters.GetString("value");
            }

            int msg;
            int.TryParse(parameters.GetString("msg"), out msg);

            ITextNode textNode = null;
            ISnapshot snapshot = null;

            if (context.Objects.Count() == 1)
            {
                var itemBase = context.Objects.First() as DatabaseProjectItem;
                if (itemBase != null)
                {
                    textNode = itemBase.SourceTextNodes.FirstOrDefault();
                }

                var projectItem = context.Objects.First() as IProjectItem;
                if (projectItem != null)
                {
                    snapshot = projectItem.Snapshots.FirstOrDefault();
                }
            }

            TraceLine(msg, text, textNode, snapshot);
        }

        protected abstract void TraceLine(int msg, [NotNull] string text, [CanBeNull] ITextNode textNode, [CanBeNull] ISnapshot snapshot);
    }
}
