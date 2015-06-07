namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Items;

  public abstract class ContentParserBase : TextNodeParserBase
  {
    protected ContentParserBase(double priority) : base(priority)
    {
    }

    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Item-Name", context.ParseContext.ItemName);
      var parentItemPath = textNode.GetAttributeValue("Parent-Item-Path", context.ParentItemPath);
      var itemIdOrPath = parentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var item = new Item(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName, 
        DatabaseName = context.ParseContext.DatabaseName, 
        ItemIdOrPath = itemIdOrPath, 
        TemplateIdOrPath = textNode.Name
      };

      item.ContentItemHack = true;

      item.References.AddRange(this.ParseReferences(item, textNode, item.TemplateIdOrPath));

      this.ParseAttributes(context, item, textNode);

      context.ParseContext.Project.AddOrMerge(item);
    }

    protected abstract void ParseAttributes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

    protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
    {
      var fieldName = fieldTextNode.Name;

      if (fieldName == "Item-Name")
      {
        return;
      }

      if (fieldName == "Parent-Item-Path")
      {
        return;
      }

      var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      // todo: support for language, version and value.hing
      field = new Field(item, fieldName, string.Empty, 0, fieldTextNode, fieldTextNode, string.Empty);
      item.Fields.Add(field);

      item.References.AddRange(this.ParseReferences(item, fieldTextNode, field.Value));
    }
  }
}
