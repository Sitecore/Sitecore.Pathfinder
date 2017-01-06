// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public interface IOptionPicker
    {
        [NotNull]
        Dictionary<string, string> GetOptions([NotNull] string optionName, [NotNull] ITaskContext context);
    }
}
