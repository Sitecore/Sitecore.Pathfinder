namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
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
      return context.Document.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.Document.Root;

      var baseTemplates = new List<string>();
      foreach (var treeNode in root.TreeNodes)
      {
        if (treeNode.Name != "Component")
        {
          throw new BuildException(Texts.Text2032, treeNode);
        }

        var componentPath = treeNode.GetAttributeValue("Component");
        if (string.IsNullOrEmpty(componentPath))
        {
          throw new BuildException(Texts.Text2033, treeNode);
        }

        baseTemplates.Add(componentPath);
      }

      var template = new Template(context.Project, root);
      context.Project.Items.Add(template);

      template.ProjectId = "{" + context.ItemPath + "}";
      template.ItemName = context.ItemName;
      template.DatabaseName = context.DatabaseName;
      template.ItemIdOrPath = context.ItemPath;
      template.BaseTemplates = string.Join("|", baseTemplates);

      var pageType = new PageType(context.Project, root);
      context.Project.Items.Add(pageType);
    }
  }
}
