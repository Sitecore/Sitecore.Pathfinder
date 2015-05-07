namespace Sitecore.Building.Compiling.ItemFiles.ItemFileCompilers.Templates
{
  using System;
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Building.Compiling.ItemFiles.Data;
  using Sitecore.Data.Items;
  using Sitecore.Extensions.XElementExtensions;

  [Export(typeof(IItemFileCompiler))]
  public class TemplateCompiler : ItemFileCompilerBase
  {
    private const string FileExtension = ".template.xml";

    public override bool CanCompile(IItemFileCompilerContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override CompilerResult Compile(IItemFileCompilerContext context)
    {
      var text = context.BuildContext.FileSystem.ReadAllText(context.FileName);

      XDocument doc;
      try
      {
        doc = XDocument.Parse(text, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
      }
      catch
      {
        context.BuildContext.Trace.TraceError(ErrorCodes.Code1000, "Template file is not valid", context.FileName);
        return CompilerResult.Failed;
      }

      var root = doc.Root;
      if (root == null)
      {
        context.BuildContext.Trace.TraceError(ErrorCodes.Code1000, "Template file is not valid", context.FileName);
        return CompilerResult.Failed;
      }

      var template = new TemplateBuilder(context, root);
      return template.Build(context);
    }

    private class TemplateBuilder : TemplateBuilderBase
    {
      public TemplateBuilder([NotNull] IItemFileCompilerContext context, [NotNull] XElement element)
      {
        this.Name = element.GetAttributeValue("name");
        this.TemplateIdOrPath = element.GetAttributeValue("id");
        this.BaseTemplates = element.GetAttributeValue("basetemplates");
        this.Icon = element.GetAttributeValue("icon");

        if (string.IsNullOrEmpty(this.BaseTemplates))
        {
          this.BaseTemplates = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";
        }

        this.ResolveItem(context);

        foreach (var child in element.Elements())
        {
          var section = new TemplateSectionBuilder(context, child, this.Item);
          this.Sections.Add(section);
        }
      }
    }

    private class TemplateFieldBuilder : TemplateFieldBuilderBase
    {
      public TemplateFieldBuilder([NotNull] IItemFileCompilerContext context, [NotNull] XElement element, [CanBeNull] Item sectionItem)
      {
        this.Name = element.GetAttributeValue("name");
        this.FieldId = element.GetAttributeValue("id");
        this.Type = element.GetAttributeValue("type");
        this.Shared = element.GetAttributeValue("shared") == "1";
        this.Unversioned = element.GetAttributeValue("unversioned") == "1";
        this.Source = element.GetAttributeValue("source");

        this.ResolveItem(context, sectionItem);
      }
    }

    private class TemplateSectionBuilder : TemplateSectionBuilderBase
    {
      public TemplateSectionBuilder([NotNull] IItemFileCompilerContext context, [NotNull] XElement element, [CanBeNull] Item templateItem)
      {
        this.Name = element.GetAttributeValue("name");
        this.SectionId = element.GetAttributeValue("id");

        this.ResolveItem(context, templateItem);

        foreach (var child in element.Elements())
        {
          var field = new TemplateFieldBuilder(context, child, this.Item);
          this.Fields.Add(field);
        }
      }
    }
  }
}
