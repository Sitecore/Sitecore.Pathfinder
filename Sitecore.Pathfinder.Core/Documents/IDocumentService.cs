namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public interface IDocumentService
  {
    [NotNull]
    IDocument LoadDocument([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile);
  }
}