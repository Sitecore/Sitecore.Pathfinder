namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers
{
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;

  [Export(typeof(IElementParser))]
  public class PathParser : ElementParserBase
  {
    public override bool CanParse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      return element.Name.LocalName == "Path";
    }

    public override void Parse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      var path = context.ItemPath;
      var databaseName = context.DatabaseName;

      context.ItemPath = element.GetAttributeValue("Path");
      if (string.IsNullOrEmpty(context.ItemPath))
      {
        throw new BuildException(Texts.Text2005, context.FileName, element, element.Attributes("Path"));
      }

      var newDatabaseName = element.GetAttributeValue("Database");
      if (!string.IsNullOrEmpty(newDatabaseName))
      {
        context.DatabaseName = newDatabaseName;
      }

      parser.ParseElements(context, element);

      context.ItemPath = path;
      context.DatabaseName = databaseName;
    }
  }
}
