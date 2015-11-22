// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Checking
{
    [InheritedExport]
    public interface IChecker
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string Categories { get; }

        void Check([NotNull] ICheckerContext context);
    }
}
