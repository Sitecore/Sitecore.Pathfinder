namespace Sitecore.Pathfinder.TextDocuments
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITextTokenService
  {
    [NotNull]
    string this[[NotNull] string tokenName] { get; set; }

    [NotNull]
    string Replace([NotNull] string text, [NotNull] Dictionary<string, string> contextTokens);
  }
}
