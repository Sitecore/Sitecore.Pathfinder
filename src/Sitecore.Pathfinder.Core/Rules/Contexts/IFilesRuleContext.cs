// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Rules.Contexts
{
    public interface IFilesRuleContext : IRuleContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<File> Files { get; }
    }
}
