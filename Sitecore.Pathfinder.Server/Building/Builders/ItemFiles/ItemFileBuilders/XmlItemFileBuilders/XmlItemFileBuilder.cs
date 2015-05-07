namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Builders.Items;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Data;

  [Export(typeof(IItemFileBuilder))]
  public class XmlItemFileBuilder : ItemFileBuilderBase
  {
    private const string FileExtension = ".item.xml";

    public XmlItemFileBuilder() : base(Items)
    {
    }

    public override void Build(IItemFileBuildContext context)
    {
      var root = this.LoadXmlFile(context);

      var parser = new XmlItemParser(context.BuildContext.CompositionService);
      var parserContext = new XmlItemParserContext(context, parser);
      parser.ParseElement(parserContext, root);

      // create templates
      foreach (var templateModel in parserContext.Templates)
      {
        var templateBuilder = new TemplateBuilder(templateModel);
        templateBuilder.Build(context);
      }

      // create items
      foreach (var itemModel in parserContext.Items)
      {
        var itemBuilder = new ItemBuilder(itemModel);
        itemBuilder.Build(context);
      }
    }

    public override bool CanBuild(IItemFileBuildContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }
  }
}
