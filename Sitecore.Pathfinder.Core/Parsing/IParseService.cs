// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing
{
    public interface IParseService
    {
        void Parse([NotNull] IProject project, [NotNull] ISourceFile sourceFile);
    }
}
