namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers
{
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(IElementParser))]
  public class PathParser : ElementParserBase
  {
    public override bool CanParse(XmlItemParserContext context, XElement element)
    {
      return element.Name.LocalName == "Path";
    }

    public override void Parse(XmlItemParserContext context, XElement element)
    {
      var path = context.ItemPath;
      var databaseName = context.DatabaseName;

      context.ItemPath = element.GetAttributeValue("Path");
      if (string.IsNullOrEmpty(context.ItemPath))
      {
        throw new BuildException(Texts.Text2005, context.BuildContext.FileName, element, element.Attributes("Path"));
      }

      var newDatabaseName = element.GetAttributeValue("Database");
      if (!string.IsNullOrEmpty(newDatabaseName))
      {
        context.DatabaseName = newDatabaseName;
      }

      context.ElementParser.ParseElements(context, element);

      context.ItemPath = path;
      context.DatabaseName = databaseName;
    }
  }
}
