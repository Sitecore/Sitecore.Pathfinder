namespace Sitecore.Pathfinder.Diagnostics
{
  using Sitecore.Pathfinder.TreeNodes;

  public class RetryableBuildException : BuildException
  {
    public RetryableBuildException(int text) : base(text)
    {
    }

    public RetryableBuildException(int text, [NotNull] string fileName, [NotNull] params object[] args) : base(text, fileName, args)
    {
    }

    public RetryableBuildException(int text, [NotNull] ITextSpan textSpan, [NotNull] params object[] args) : base(text, textSpan, args)
    {
    }
  }
}
