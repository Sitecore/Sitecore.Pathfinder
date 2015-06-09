// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    public interface ISnapshot
    {
        [NotNull]
        ISourceFile SourceFile { get; }
    }
}
