namespace Sitecore.Pathfinder.TextDocuments.Json
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ITextDocumentLexer))]
  public class JsonTextDocumentLexer : ITextDocumentLexer
  {
    public bool CanLex(IParseContext context, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".json", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public ITextDocument Lex(IParseContext context, ISourceFile sourceFile)
    {
      return new JsonTextDocument(context, sourceFile);
    }
  }
}