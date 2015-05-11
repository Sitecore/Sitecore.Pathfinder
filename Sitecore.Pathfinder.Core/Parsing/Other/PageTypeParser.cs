namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Projects.Other;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IParser))]
  public class PageTypeParser : ParserBase
  {
    private const string FileExtension = ".pagetype.xml";

    public PageTypeParser() : base(Templates)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.SourceFile.ReadAsXml(context);

      var baseTemplates = new List<string>();
      foreach (var element in root.Elements())
      {
        if (element.Name.LocalName != "Component")
        {
          throw new BuildException(Texts.Text2032, context.SourceFile.SourceFileName, element);
        }

        var componentPath = element.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          throw new BuildException(Texts.Text2033, context.SourceFile.SourceFileName, element);
        }

        baseTemplates.Add(componentPath);
      }

      var template = new Template(context.Project, context.SourceFile);
      context.Project.Items.Add(template);

      template.ItemName = context.ItemName;
      template.DatabaseName = context.DatabaseName;
      template.ItemIdOrPath = context.ItemPath;
      template.BaseTemplates = string.Join("|", baseTemplates);

      var pageType = new PageType(context.Project, context.SourceFile);
      context.Project.Items.Add(pageType);
    }
  }
}
