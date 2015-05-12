using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.TreeNodes
{
  public interface ITextSpan
  {
    int LineNumber { get; }
    int LinePosition { get; }
    ISourceFile SourceFile { get; }
  }
}