namespace Sitecore.Pathfinder.Diagnostics
{
  using System.ComponentModel;
  using Sitecore.Pathfinder.TextDocuments;

  public class RetryableBuildException : BuildException
  {
    public RetryableBuildException([Localizable(true)] [NotNull] string text) : base(text)
    {
    }

    public RetryableBuildException([Localizable(true)] [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "") : base(text, sourceFile, details)
    {
    }

    public RetryableBuildException([Localizable(true)] [NotNull] string text, [NotNull] IDocument document, [NotNull] string details = "") : base(text, document, details)
    {
    }

    public RetryableBuildException([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(text, textNode, details)
    {
    }
  }
}
