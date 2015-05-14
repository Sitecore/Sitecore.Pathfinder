namespace Sitecore.Pathfinder.TextDocuments
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;

  [Export(typeof(ITextTokenService))]
  public class TextTokenService : ITextTokenService
  {
    private readonly Dictionary<string, string> globalTokens = new Dictionary<string, string>();

    public string this[string tokenName]
    {
      get
      {
        string value;
        return this.globalTokens.TryGetValue(tokenName, out value) ? value : string.Empty;
      }

      set
      {
        this.globalTokens[tokenName] = value;
      }
    }

    public string Replace(string text, Dictionary<string, string> contextTokens)
    {
      foreach (var token in this.globalTokens)
      {
        var tokenName = "$" + token.Key;
        text = text.Replace(tokenName, token.Value);
      }

      foreach (var token in contextTokens)
      {
        var tokenName = "$" + token.Key;
        text = text.Replace(tokenName, token.Value);
      }

      return text;
    }
  }
}
