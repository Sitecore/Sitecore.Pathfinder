namespace Sitecore.Pathfinder.Documents.Json
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLexer))]
  public class JsonDocumentLexer : IDocumentLexer
  {
    public bool CanLex(IParseContext context, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".json", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocument Lex(IParseContext context, ISourceFile sourceFile)
    {
      return new JsonDocument(context, sourceFile);
    }
  }
}