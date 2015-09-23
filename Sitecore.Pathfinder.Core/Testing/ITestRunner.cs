// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Testing
{
    public interface ITestRunner
    {
        [NotNull]
        string Name { get; }

        [CanBeNull]
        Assembly CompileAssembly([NotNull][ItemNotNull] ICollection<string> references, [NotNull][ItemNotNull] IEnumerable<string> fileNames);

        void RunTests([NotNull] Assembly testAssembly);
    }
}
