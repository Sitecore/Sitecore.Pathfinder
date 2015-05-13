namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLexer))]
  public class XmlDocumentLexer : IDocumentLexer
  {
    public bool CanLex(IParseContext context, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocument Lex(IParseContext context, ISourceFile sourceFile)
    {
      return new XmlDocument(context, sourceFile);
    }
  }
}
