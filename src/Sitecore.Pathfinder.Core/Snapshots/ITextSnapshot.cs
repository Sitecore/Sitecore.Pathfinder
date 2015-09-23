// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ITextSnapshot : ISnapshot
    {
        [NotNull]
        string ParseError { get; }

        TextSpan ParseErrorTextSpan { get; }

        [NotNull]
        ITextNode Root { get; }

        void ValidateSchema([NotNull] IParseContext context);
    }
}
