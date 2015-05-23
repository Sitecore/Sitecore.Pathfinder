namespace Sitecore.Pathfinder.Diagnostics
{
  using System.ComponentModel;
  using Sitecore.Pathfinder.TextDocuments;

  public class RetryableEmitException : EmitException
  {
    public RetryableEmitException([Localizable(true)] [NotNull] string text) : base(text)
    {
    }

    public RetryableEmitException([Localizable(true)] [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "") : base(text, sourceFile, details)
    {
    }

    public RetryableEmitException([Localizable(true)] [NotNull] string text, [NotNull] IDocumentSnapshot documentSnapshot, [NotNull] string details = "") : base(text, documentSnapshot, details)
    {
    }

    public RetryableEmitException([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(text, textNode, details)
    {
    }
  }
}
