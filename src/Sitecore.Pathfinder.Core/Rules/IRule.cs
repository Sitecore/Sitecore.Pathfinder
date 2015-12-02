// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    public interface IRule
    {
        [NotNull]
        string Filter { get; }

        bool EvaluateIf([NotNull] IRuleContext context);

        bool Execute([NotNull] IRuleContext context);

        void ExecuteElse([NotNull] IRuleContext context);

        void ExecuteThen([NotNull] IRuleContext context);
    }
}
