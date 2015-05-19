namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.ComponentModel.Composition;
  using System.Linq;
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

    public ComponentParser() : base(Constants.Parsers.Items)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.Document.SourceFile.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = context.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException(Texts.Text3031, context.Document);
      }

      var textNode = textDocument.Root;

      var privateTemplate = this.Parse(context, textNode);
      if (privateTemplate == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      var publicTemplate = this.CreatePublicTemplate(context, textNode, privateTemplate);

      var component = new Component(context.Project, context.Document, privateTemplate, publicTemplate);
      context.Project.AddOrMerge(component);
    }

    [NotNull]
    protected Template CreatePublicTemplate([NotNull] IParseContext context, [NotNull] ITextNode textNode, [NotNull] Template privateTemplate)
    {
      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemName = privateTemplate.ItemName.Mid(2);
      var itemIdOrPath = parentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("PublicTemplate.Id", itemIdOrPath);

      var publicTemplate = new Template(context.Project, projectUniqueId, privateTemplate.TextNode)
      {
        ItemName = itemName,
        DatabaseName = privateTemplate.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        BaseTemplates = privateTemplate.ItemIdOrPath,
      };

      return context.Project.AddOrMerge(publicTemplate);
    }

    [NotNull]
    protected Template Parse([NotNull] IParseContext context, [NotNull] ITextNode textNode)
    {
      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemName = "__" + textNode.GetAttributeValue("Name", context.ItemName);
      var itemIdOrPath = parentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("PrivateTemplate.Id", itemIdOrPath);

      var privateTemplate = new Template(context.Project, projectUniqueId, textNode)
      {
        ItemName = itemName,
        DatabaseName = context.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate),
        Icon = textNode.GetAttributeValue("Icon"),
        ShortHelp = textNode.GetAttributeValue("ShortHelp"),
        LongHelp = textNode.GetAttributeValue("LongHelp"),
      };

      foreach (var sectionTreeNode in textNode.ChildNodes)
      {
        this.ParseSection(context, privateTemplate, sectionTreeNode);
      }

      return context.Project.AddOrMerge(privateTemplate);
    }

    protected void ParseField([NotNull] IParseContext context, [NotNull] TemplateSection section, [NotNull] ITextNode fieldTextNode)
    {
      var fieldName = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        throw new BuildException(Texts.Text2008, fieldTextNode);
      }

      var templateField = section.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateField == null)
      {
        templateField = new TemplateField();
        section.Fields.Add(templateField);
        templateField.Name = fieldName;
      }

      templateField.Type = fieldTextNode.GetAttributeValue("Type", "Single-Line Text");
      templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTextNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");
    }

    protected void ParseSection([NotNull] IParseContext context, [NotNull] Template template, [NotNull] ITextNode sectionTextNode)
    {
      var sectionName = sectionTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(sectionName))
      {
        throw new BuildException(Texts.Text2007, sectionTextNode);
      }

      var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.Name, sectionName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateSection == null)
      {
        templateSection = new TemplateSection();
        template.Sections.Add(templateSection);
        templateSection.Name = sectionName;
      }

      templateSection.Icon = sectionTextNode.GetAttributeValue("Icon");

      foreach (var fieldTreeNode in sectionTextNode.ChildNodes)
      {
        this.ParseField(context, templateSection, fieldTreeNode);
      }
    }
  }
}