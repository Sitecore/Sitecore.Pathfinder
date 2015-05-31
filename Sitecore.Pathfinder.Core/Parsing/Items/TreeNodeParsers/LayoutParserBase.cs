namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class LayoutParserBase : TextNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = PathHelper.GetItemParentPath(context.ParentItemPath) + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var item = new Item(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName, 
        DatabaseName = context.ParseContext.DatabaseName, 
        ItemIdOrPath = itemIdOrPath, 
      };

      var value = this.GetValue(context, textNode);
      if (!string.IsNullOrEmpty(value))
      {
        item.Fields.Add(new Field("__Renderings", new Property(new TextNode(textNode.Snapshot, "__Renderings", value, null))));
      }

      context.ParseContext.Project.AddOrMerge(item);
    }

    [NotNull]
    protected virtual string GetValue([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
    {
      return string.Empty;
    }

    protected virtual void ParseDeviceReferences([NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
    {
      var deviceNameTextNode = deviceTextNode.GetAttribute("Name") ?? deviceTextNode;
      references.Add(new LayoutDeviceReference(projectItem, deviceNameTextNode, deviceTextNode.GetAttributeValue("Name")));

      var layoutTextNode = deviceTextNode.GetAttribute("Layout");
      if (layoutTextNode != null)
      {
        references.Add(new LayoutReference(projectItem, layoutTextNode, deviceTextNode.GetAttributeValue("Layout")));
      }

      foreach (var renderingTextNode in deviceTextNode.ChildNodes)
      {
        this.ParseRenderingReferences(references, projectItem, renderingTextNode);
      }
    }

    protected override IEnumerable<IReference> ParseReferences(IProjectItem projectItem, ITextNode source, string text)
    {
      var result = base.ParseReferences(projectItem, source, text).ToList();

      var layoutTextNode = source.ChildNodes.FirstOrDefault();
      if (layoutTextNode == null)
      {
        return result;
      }

      foreach (var deviceTextNode in layoutTextNode.ChildNodes)
      {
        this.ParseDeviceReferences(result, projectItem, deviceTextNode);
      }

      return result;
    }

    protected virtual void ParseRenderingReferences([NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode)
    {
      references.Add(new LayoutRenderingReference(projectItem, renderingTextNode, renderingTextNode.Name));

      foreach (var childTextNode in renderingTextNode.ChildNodes)
      {
        this.ParseRenderingReferences(references, projectItem, childTextNode);
      }
    }
  }
}
