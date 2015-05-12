namespace Sitecore.Pathfinder.TreeNodes.Xml
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IDocumentLoader))]
  public class XmlDocumentLoader : IDocumentLoader
  {
    public bool CanLoad(IParseContext context, ISourceFile sourceFile)
    {
      return string.Compare(Path.GetExtension(sourceFile.SourceFileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public IDocument Load(IParseContext context, ISourceFile sourceFile)
    {
      return new XmlDocument(context, sourceFile);
    }
  }
}
