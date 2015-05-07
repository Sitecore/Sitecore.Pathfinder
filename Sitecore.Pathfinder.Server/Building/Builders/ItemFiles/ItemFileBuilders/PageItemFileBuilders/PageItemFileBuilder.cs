namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.PageItemFileBuilders
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IItemFileBuilder))]
  public class PageItemFileBuilder : ItemFileBuilderBase
  {
    private const string FileExtension = ".page.xml";

    public PageItemFileBuilder() : base(Template)
    {
    }

    public override void Build(IItemFileBuildContext context)
    {
      var root = this.LoadXmlFile(context);
      var database = Factory.GetDatabase(context.DatabaseName);

      var baseTemplates = new List<Item>();
      foreach (var element in root.Elements())
      {
        if (element.Name.LocalName != "Component")
        {
          throw new BuildException(Texts.Text2032, context.FileName, element);
        }

        var componentPath = element.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          throw new BuildException(Texts.Text2033, context.FileName, element);
        }

        var componentItem = database.GetItem(componentPath);
        if (componentItem == null)
        {
          throw new BuildException(Texts.Text2034, context.FileName, element);
        }

        baseTemplates.Add(componentItem);
      }

      var templateModel = new TemplateModel();
      context.BuildContext.Objects.Add(templateModel);

      var n = context.ItemPath.LastIndexOf('/');
      templateModel.Name = context.ItemPath.Mid(n + 1);
      templateModel.ItemIdOrPath = context.ItemPath;
      templateModel.DatabaseName = context.DatabaseName;
      templateModel.BaseTemplates = string.Join("|", baseTemplates.Select(i => i.ID.ToString()));

      var templateBuilder = new TemplateBuilder(templateModel);
      templateBuilder.Build(context);
      templateBuilder.CreateStandardValuesItem(context);
    }

    public override bool CanBuild(IItemFileBuildContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }
  }
}
