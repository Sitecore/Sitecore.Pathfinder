namespace Sitecore.Pathfinder.Diagnostics
{
  using System.Xml.Linq;

  public class RetryableBuildException : BuildException
  {
    public RetryableBuildException(int text) : base(text)
    {
    }

    public RetryableBuildException(int text, [NotNull] string fileName, int line = 0, int column = 0, [NotNull] params object[] args) : base(text, fileName, line, column, args)
    {
    }

    public RetryableBuildException(int text, [NotNull] string fileName, [NotNull] XElement element, [CanBeNull] XAttribute attribute, [NotNull] params object[] args) : base(text, fileName, element, attribute, args)
    {
    }

    public RetryableBuildException(int text, [NotNull] string fileName, [CanBeNull] XElement element, [NotNull] params object[] args) : base(text, fileName, element, args)
    {
    }
  }
}
