namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface ITokenService
  {
    [NotNull]
    string this[[NotNull] string tokenName] { get; set; }

    [NotNull]
    string Replace([NotNull] string text, [NotNull] Dictionary<string, string> contextTokens);
  }
}
