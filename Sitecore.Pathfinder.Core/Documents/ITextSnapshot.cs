// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Documents
{
    public interface ITextSnapshot : ISnapshot
    {
        [NotNull]
        ITextNode Root { get; }

        void ValidateSchema([NotNull] IParseContext context);
    }
}
