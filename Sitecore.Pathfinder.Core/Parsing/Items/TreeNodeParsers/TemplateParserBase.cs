namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class TemplateParserBase : TextNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemName);

      var template = new Template(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName,
        DatabaseName = context.ParseContext.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate),
        Icon = textNode.GetAttributeValue("Icon"),
        ShortHelp = textNode.GetAttributeValue("ShortHelp"),
        LongHelp = textNode.GetAttributeValue("LongHelp")
      };

      var sectionsTextNode = this.GetSectionsTextNode(textNode);
      if (sectionsTextNode != null)
      {
        foreach (var sectionTreeNode in sectionsTextNode.ChildNodes)
        {
          this.ParseSection(context, template, sectionTreeNode);
        }
      }

      context.ParseContext.Project.Items.Add(template);
    }

    [CanBeNull]
    protected abstract ITextNode GetFieldsTextNode([NotNull] ITextNode textNode);

    [CanBeNull]
    protected abstract ITextNode GetSectionsTextNode([NotNull] ITextNode textNode);

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] TemplateSection templateSection, [NotNull] ITextNode fieldTextNode)
    {
      var templateField = new TemplateField();
      templateSection.Fields.Add(templateField);

      templateField.Name = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, fieldTextNode);
      }

      templateField.Type = fieldTextNode.GetAttributeValue("Type", "Single-Line Text");
      templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTextNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode sectionTextNode)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionTextNode.GetAttributeValue("Name");
      templateSection.Icon = sectionTextNode.GetAttributeValue("Icon");

      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sectionTextNode);
      }

      var fieldsTextNode = this.GetFieldsTextNode(sectionTextNode);
      if (fieldsTextNode == null)
      {
        return;
      }

      foreach (var fieldTextNode in fieldsTextNode.ChildNodes)
      {
        this.ParseField(context, templateSection, fieldTextNode);
      }
    }
  }
}
