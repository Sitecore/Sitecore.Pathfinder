// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Actions
{
    public class AbortAction : ActionBase
    {
        protected AbortAction() : base("abort")
        {
        }

        public override void Execute(IRuleContext context, IDictionary<string, object> parameters)
        {
            context.IsAborted = true;
        }
    }
}
