namespace Sitecore.Pathfinder.Projects.Locations
{
  using System.Xml;
  using Sitecore.Pathfinder.Diagnostics;

  public class XElementLocation : Location
  {
    public XElementLocation([NotNull] ISourceFile sourceFile, IXmlLineInfo lineInfo) : base(sourceFile)
    {
      this.LineInfo = lineInfo;
    }

    [NotNull]
    public IXmlLineInfo LineInfo { get; }
  }
}