using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface ITemplatesRuleContext : IRuleContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<Template> Templates { get; }
    }
}