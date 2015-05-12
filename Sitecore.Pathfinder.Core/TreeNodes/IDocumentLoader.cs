namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentLoader
  {
    bool CanLoad(IParseContext context, [NotNull] ISourceFile sourceFile);

    [NotNull]
    IDocument Load(IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}