namespace Sitecore.Pathfinder.Diagnostics
{
  using Sitecore.Pathfinder.TextDocuments;

  public class RetryableBuildException : BuildException
  {
    public RetryableBuildException(int text) : base(text)
    {
    }

    public RetryableBuildException(int text, [NotNull] ISourceFile sourceFile, [NotNull] params object[] args) : base(text, sourceFile, args)
    {
    }

    public RetryableBuildException(int text, [NotNull] IDocument document, [NotNull] params object[] args) : base(text, document, args)
    {
    }

    public RetryableBuildException(int text, [NotNull] ITextNode textNode, [NotNull] params object[] args) : base(text, textNode, args)
    {
    }
  }
}
