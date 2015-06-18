// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Reflection;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Testing
{
    public interface ITestRunner
    {
        [CanBeNull]
        Assembly CompileAssembly([NotNull] ICollection<string> references, [NotNull] IEnumerable<string> fileNames);

        void RunTests([NotNull] Assembly testAssembly);
    }
}
