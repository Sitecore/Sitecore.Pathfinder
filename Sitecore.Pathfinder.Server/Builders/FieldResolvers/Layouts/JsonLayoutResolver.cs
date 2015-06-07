namespace Sitecore.Pathfinder.Builders.FieldResolvers.Layouts
{
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Documents;

  public class JsonLayoutResolver : LayoutResolverBase
  {
    protected override void WriteRendering(LayoutResolveContext context, XmlTextWriter output, IEnumerable<Item> renderingItems, Database database, ITextNode renderingTextNode, string placeholders)
    {
      renderingTextNode = renderingTextNode.ChildNodes[0];
      if (renderingTextNode != null)
      {
        base.WriteRendering(context, output, renderingItems, database, renderingTextNode, placeholders);
      }
    }
  }
}