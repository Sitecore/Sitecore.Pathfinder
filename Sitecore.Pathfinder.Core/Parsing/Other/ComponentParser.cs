namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Other;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IParser))]
  public class ComponentParser : ParserBase
  {
    private const string FileExtension = ".component.xml";

    public ComponentParser() : base(Items)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.TextDocument.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var textNode = context.TextDocument.Root;

      var privateTemplate = this.Parse(context, textNode);
      if (privateTemplate == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      var publicTemplate = this.CreatePublicTemplate(context, textNode, privateTemplate);

      var component = new Component(context.Project, textNode, privateTemplate, publicTemplate);
      context.Project.Items.Add(component);
    }

    [NotNull]
    protected Template CreatePublicTemplate([NotNull] IParseContext context, [NotNull] ITextNode textNode, [NotNull] Template privateTemplate)
    {
      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemName = privateTemplate.ItemName.Mid(2);
      var itemIdOrPath = parentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("PublicTemplate.Id", itemName);

      var publicTemplate = new Template(context.Project, projectUniqueId, privateTemplate.TextNode)
      {
        ItemName = itemName, 
        DatabaseName = privateTemplate.DatabaseName, 
        ItemIdOrPath = itemIdOrPath, 
        BaseTemplates = privateTemplate.ItemIdOrPath
      };

      context.Project.Items.Add(publicTemplate);

      return publicTemplate;
    }

    [NotNull]
    protected Template Parse([NotNull] IParseContext context, [NotNull] ITextNode textNode)
    {
      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemName = "__" + textNode.GetAttributeValue("Name", context.ItemName);
      var itemIdOrPath = parentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("PrivateTemplate.Id", itemName);

      var template = new Template(context.Project, projectUniqueId, textNode)
      {
        ItemName = itemName, 
        DatabaseName = context.DatabaseName, 
        ItemIdOrPath = itemIdOrPath, 
        BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate), 
        Icon = textNode.GetAttributeValue("Icon"), 
        ShortHelp = textNode.GetAttributeValue("ShortHelp"), 
        LongHelp = textNode.GetAttributeValue("LongHelp")
      };

      foreach (var sectionTreeNode in textNode.ChildNodes)
      {
        this.ParseSection(context, template, sectionTreeNode);
      }

      context.Project.Items.Add(template);

      return template;
    }

    protected void ParseField([NotNull] IParseContext context, [NotNull] TemplateSection section, [NotNull] ITextNode fieldTextNode)
    {
      var templateField = new TemplateField();
      section.Fields.Add(templateField);

      templateField.Name = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, fieldTextNode);
      }

      templateField.Type = fieldTextNode.GetAttributeValue("Type");
      templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTextNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");

      if (string.IsNullOrEmpty(templateField.Type))
      {
        templateField.Type = "Single-Line Text";
      }
    }

    protected void ParseSection([NotNull] IParseContext context, [NotNull] Template template, [NotNull] ITextNode sectionTextNode)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionTextNode.GetAttributeValue("Name");
      templateSection.Icon = sectionTextNode.GetAttributeValue("Icon");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sectionTextNode);
      }

      foreach (var fieldTreeNode in sectionTextNode.ChildNodes)
      {
        this.ParseField(context, templateSection, fieldTreeNode);
      }
    }
  }
}