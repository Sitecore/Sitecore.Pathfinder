namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders
{
  using System.Collections.Generic;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Models.Items;
  using Sitecore.Pathfinder.Models.Templates;

  public class XmlItemParserContext
  {
    public XmlItemParserContext([NotNull] IItemFileBuildContext buildContext, [NotNull] XmlItemParser elementParser)
    {
      this.BuildContext = buildContext;
      this.ElementParser = elementParser;

      this.Items = new List<ItemModel>();
      this.Templates = new List<TemplateModel>();
      this.ItemPath = buildContext.ItemPath;
      this.DatabaseName = buildContext.DatabaseName;
    }

    [NotNull]
    public IItemFileBuildContext BuildContext { get; }

    [NotNull]
    public string DatabaseName { get; set; }

    [NotNull]
    public XmlItemParser ElementParser { get; }

    [NotNull]
    public string ItemName
    {
      get
      {
        var n = this.ItemPath.LastIndexOf('/');
        return n >= 0 ? this.ItemPath.Mid(n + 1) : this.ItemPath;
      }
    }

    [NotNull]
    public string ItemPath { get; set; }

    [NotNull]
    public ICollection<ItemModel> Items { get; private set; }

    [NotNull]
    public string ParentItemPath
    {
      get
      {
        var n = this.ItemPath.LastIndexOf('/');
        return this.ItemPath.Left(n);
      }
    }

    [NotNull]
    public ICollection<TemplateModel> Templates { get; private set; }
  }
}
