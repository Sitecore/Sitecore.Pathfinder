// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Rules.Contexts;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public abstract class TraceBase : ActionBase
    {
        protected TraceBase([NotNull] ITraceService trace, [NotNull] string name) : base(name)
        {
            Trace = trace;
        }

        [NotNull]
        protected ITraceService Trace { get; }

        public override void Execute(IRuleContext context, IDictionary<string, string> parameters)
        {
            var text = parameters.GetString("text");

            int msg;
            int.TryParse(parameters.GetString("msg"), out msg);

            ITextNode textNode = null;
            ISnapshot snapshot = null;

            var itemsContext = context as IItemsRuleContext;
            var templatesContext = context as ITemplatesRuleContext;
            var itemBasesContext = context as IItemBasesRuleContext;
            var projectItemsContext = context as IProjectItemRuleContext;
            var fileContext = context as IFilesRuleContext;

            if (itemsContext != null && itemsContext.Items.Count() == 1)
            {
                textNode = itemsContext.Items.First().SourceTextNodes.FirstOrDefault();
            }
            else if (templatesContext != null && templatesContext.Templates.Count() == 1)
            {
                textNode = templatesContext.Templates.First().SourceTextNodes.FirstOrDefault();
            }
            else if (itemBasesContext != null && itemBasesContext.ItemBases.Count() == 1)
            {
                textNode = itemBasesContext.ItemBases.First().SourceTextNodes.FirstOrDefault();
            }
            else if (projectItemsContext != null && projectItemsContext.ProjectItems.Count() == 1)
            {
                snapshot = projectItemsContext.ProjectItems.First().Snapshots.FirstOrDefault();
            }
            else if (fileContext != null && fileContext.Files.Count() == 1)
            {
                snapshot = fileContext.Files.First().Snapshots.FirstOrDefault();
            }

            TraceLine(msg, text, textNode, snapshot);
        }

        protected abstract void TraceLine(int msg, [NotNull] string text, [CanBeNull] ITextNode textNode, [CanBeNull] ISnapshot snapshot);
    }
}
