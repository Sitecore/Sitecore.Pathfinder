namespace Sitecore.Pathfinder.TextDocuments.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(ITextDocumentLexer))]
  public class XmlTextDocumentLexer : ITextDocumentLexer
  {
    public bool CanLex(IParseContext context, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public ITextDocument Lex(IParseContext context, ISourceFile sourceFile)
    {
      return new XmlTextDocument(context, sourceFile);
    }
  }
}
