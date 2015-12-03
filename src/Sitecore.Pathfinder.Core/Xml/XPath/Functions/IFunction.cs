// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public interface IFunction
    {
        [NotNull]
        string Name { get; }

        [CanBeNull]
        object Evaluate([NotNull] FunctionArgs args);
    }
}
