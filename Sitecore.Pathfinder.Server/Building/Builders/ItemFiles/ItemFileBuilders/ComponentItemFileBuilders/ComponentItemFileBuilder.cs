namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.ComponentItemFileBuilders
{
  using System;
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Builders.Templates;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IItemFileBuilder))]
  public class ComponentItemFileBuilder : ItemFileBuilderBase
  {
    private const string FileExtension = ".component.xml";

    public ComponentItemFileBuilder() : base(Template)
    {
    }

    public override void Build(IItemFileBuildContext context)
    {
      var root = this.LoadXmlFile(context);

      var templateItem = this.CreatePrivateTemplate(context, root);
      if (templateItem == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      this.CreatePublicTemplate(context, templateItem);
    }

    public override bool CanBuild(IItemFileBuildContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public void Parse([NotNull] IItemFileBuildContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement element)
    {
      templateModel.Name = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        var n = context.ItemPath.LastIndexOf('/');
        templateModel.Name = n >= 0 ? context.ItemPath.Mid(n + 1) : context.ItemPath;
      }

      templateModel.DatabaseName = context.DatabaseName;
      templateModel.ItemIdOrPath = context.ItemPath;
      templateModel.BaseTemplates = element.GetAttributeValue("BaseTemplates") ?? string.Empty;
      templateModel.Icon = element.GetAttributeValue("Icon") ?? string.Empty;

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, templateModel, sectionElement);
      }
    }

    [CanBeNull]
    protected Item CreatePrivateTemplate([NotNull] IItemFileBuildContext context, [NotNull] XElement root)
    {
      var templateModel = new TemplateModel();
      context.BuildContext.Objects.Add(templateModel);

      // todo: remove duplicated code from TemplateParser
      this.Parse(context, templateModel, root);

      templateModel.Name = "__" + templateModel.Name;
      var n = templateModel.ItemIdOrPath.LastIndexOf('/');
      templateModel.ItemIdOrPath = templateModel.ItemIdOrPath.Left(n + 1) + templateModel.Name;

      var templateBuilder = new TemplateBuilder(templateModel);
      return templateBuilder.Build(context);
    }

    protected void CreatePublicTemplate([NotNull] IItemFileBuildContext context, [NotNull] Item privateTemplate)
    {
      var templateModel = new TemplateModel();
      context.BuildContext.Objects.Add(templateModel);

      // cut off '__' prefix
      templateModel.Name = privateTemplate.Name.Mid(2);
      templateModel.DatabaseName = privateTemplate.Database.Name;
      templateModel.ItemIdOrPath = privateTemplate.Parent.Paths.Path + "/" + templateModel.Name;
      templateModel.BaseTemplates = privateTemplate.ID.ToString();

      // create the public template
      var publicTemplate = privateTemplate.Parent.Add(templateModel.Name, new TemplateID(TemplateIDs.Template));
      using (new EditContext(publicTemplate))
      {
        publicTemplate[FieldIDs.BaseTemplate] = privateTemplate.ID.ToString();
      }

      // create standard values
      var standardValuesItem = publicTemplate.Add("__Standard Values", new TemplateID(publicTemplate.TemplateID));
      if (standardValuesItem == null)
      {
        throw new BuildException(Texts.Text2023, context.FileName);
      }

      using (new EditContext(publicTemplate))
      {
        publicTemplate[FieldIDs.StandardValueHolderId] = standardValuesItem.ID.ToString();
      }
    }

    private void ParseField([NotNull] IItemFileBuildContext context, [NotNull] TemplateSectionModel sectionModel, [NotNull] XElement fieldElement)
    {
      var fieldModel = new TemplateFieldModel();
      sectionModel.Fields.Add(fieldModel);

      fieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, context.FileName, fieldElement);
      }

      fieldModel.Shared = fieldElement.GetAttributeValue("Sharing") == "Shared";
      fieldModel.Unversioned = fieldElement.GetAttributeValue("Sharing") == "Unversioned";
      fieldModel.Source = fieldElement.GetAttributeValue("Source") ?? string.Empty;

      fieldModel.Type = fieldElement.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(fieldModel.Type))
      {
        fieldModel.Type = "Single-Line Text";
      }
    }

    private void ParseSection([NotNull] IItemFileBuildContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement sectionElement)
    {
      var sectionModel = new TemplateSectionModel();
      templateModel.Sections.Add(sectionModel);

      sectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        throw new BuildException(Texts.Text2007, context.FileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, sectionModel, fieldElement);
      }
    }
  }
}
