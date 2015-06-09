// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents
{
    public interface ITextTokenService
    {
        [NotNull]
        string this[[NotNull] string tokenName] { get; set; }

        [NotNull]
        string Replace([NotNull] string text, [NotNull] Dictionary<string, string> contextTokens);
    }
}
