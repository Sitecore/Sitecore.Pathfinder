// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.CodeGeneration
{
    public interface ITextTemplatingEngine
    {
        [NotNull]
        string Generate([NotNull] string template, [NotNull] object model);
    }
}
