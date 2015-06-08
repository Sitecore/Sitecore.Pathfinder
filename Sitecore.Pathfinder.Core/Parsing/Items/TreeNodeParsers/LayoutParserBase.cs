namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class LayoutParserBase : TextNodeParserBase
  {
    protected LayoutParserBase(double priority) : base(priority)
    {
    }

    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode);
      item.ItemName = itemName;
      item.DatabaseName = context.ParseContext.DatabaseName;
      item.ItemIdOrPath = itemIdOrPath;

      item.Fields.Add(context.ParseContext.Factory.Field(item, "__Renderings", string.Empty, 0, textNode, textNode));

      context.ParseContext.Project.AddOrMerge(item);
    }

    protected virtual void ParseDeviceReferences([NotNull] ItemParseContext context, [NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceTextNode)
    {
      var deviceNameTextNode = deviceTextNode.GetAttribute("Name") ?? deviceTextNode;
      references.Add(context.ParseContext.Factory.DeviceReference(projectItem, deviceNameTextNode, deviceTextNode.GetAttributeValue("Name")));

      var layoutTextNode = deviceTextNode.GetAttribute("Layout");
      if (layoutTextNode != null)
      {
        references.Add(context.ParseContext.Factory.LayoutReference(projectItem, layoutTextNode, deviceTextNode.GetAttributeValue("Layout")));
      }

      foreach (var renderingTextNode in deviceTextNode.ChildNodes)
      {
        this.ParseRenderingReferences(context, references, projectItem, renderingTextNode);
      }
    }

    protected override IEnumerable<IReference> ParseReferences(ItemParseContext context, IProjectItem projectItem, ITextNode source, string text)
    {
      var result = base.ParseReferences(context, projectItem, source, text).ToList();

      var layoutTextNode = source.ChildNodes.FirstOrDefault();
      if (layoutTextNode == null)
      {
        return result;
      }

      foreach (var deviceTextNode in layoutTextNode.ChildNodes)
      {
        this.ParseDeviceReferences(context, result, projectItem, deviceTextNode);
      }

      return result;
    }

    protected virtual void ParseRenderingReferences([NotNull] ItemParseContext context, [NotNull] ICollection<IReference> references, [NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode)
    {
      references.Add(context.ParseContext.Factory.LayoutRenderingReference(projectItem, renderingTextNode, renderingTextNode.Name));

      foreach (var childTextNode in renderingTextNode.ChildNodes)
      {
        this.ParseRenderingReferences(context, references, projectItem, childTextNode);
      }
    }
  }
}
