namespace Sitecore.Pathfinder.Parsing.Items.PageParsers
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IItemParser))]
  public class PageParser : ItemParserBase
  {
    private const string FileExtension = ".page.xml";

    public PageParser() : base(Template)
    {
    }

    public override bool CanParse(IItemParseContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var root = this.LoadXmlFile(context);

      var baseTemplates = new List<string>();
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

        baseTemplates.Add(componentPath);
      }

      var templateModel = new TemplateModel(context.FileName);
      context.ParseContext.Project.Models.Add(templateModel);

      var n = context.ItemPath.LastIndexOf('/');
      templateModel.Name = context.ItemPath.Mid(n + 1);
      templateModel.ItemIdOrPath = context.ItemPath;
      templateModel.DatabaseName = context.DatabaseName;
      templateModel.BaseTemplates = string.Join("|", baseTemplates);
    }
  }
}
