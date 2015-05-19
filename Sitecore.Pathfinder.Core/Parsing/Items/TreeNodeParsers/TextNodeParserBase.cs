namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.References;
  using Sitecore.Pathfinder.Text;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class TextNodeParserBase : ITextNodeParser
  {
    public abstract bool CanParse(ItemParseContext context, ITextNode textNode);

    public abstract void Parse(ItemParseContext context, ITextNode textNode);

    [NotNull]
    protected virtual IEnumerable<IReference> ParseReferences([NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
    {
      var reference = this.ParseReference(projectItem, source, text);
      if (reference != null)
      {
        yield return reference;
        yield break;
      }

      if (text.IndexOf('|') >= 0)
      {
        var parts = text.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
          reference = this.ParseReference(projectItem, source, part);
          if (reference != null)
          {
            yield return reference;
          }
        }

        yield break;
      }

      if (text.IndexOf('&') < 0 && text.IndexOf('=') < 0)
      {
        yield break;
      }

      var urlString = new UrlString(text);

      foreach (string key in urlString.Parameters)
      {
        if (string.IsNullOrEmpty(key))
        {
          continue;
        }

        var value = urlString.Parameters[key];

        reference = this.ParseReference(projectItem, source, value);
        if (reference != null)
        {
          yield return reference;
        }
      }
    }

    [CanBeNull]
    protected virtual IReference ParseReference([NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
    {
      if (text.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
      {
        return new Reference(projectItem, source, text);
      }

      Guid guid;
      if (Guid.TryParse(text, out guid))
      {
        return new Reference(projectItem, source, guid.ToString("B").ToUpperInvariant());
      }

      return null;
    }
  }
}
