﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISnapshotLoader
    {
        double Priority { get; }

        bool CanLoad([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISnapshot Load([NotNull] ISourceFile sourceFile, [NotNull] IDictionary<string, string> tokens);
    }
}
