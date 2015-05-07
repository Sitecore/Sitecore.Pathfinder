namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(IPreprocessor))]
  public class XmlItemFilePreprocessor : PreprocessorBase
  {
    private static XmlSchemaSet schemas;

    public XmlItemFilePreprocessor() : base("item-xml-file")
    {
    }

    public override void Execute(IPreprocessingContext context, string fileName)
    {
      var serializationDirectory = Path.Combine(context.BuildContext.OutputDirectory, context.SerializationDirectory);
      var destinationFileName = Path.Combine(serializationDirectory, Path.GetFileName(fileName) ?? string.Empty);

      if (PathHelper.CompareFiles(fileName, destinationFileName))
      {
        return;
      }

      this.Validate(context, fileName);

      this.Copy(context, fileName, destinationFileName);

      context.BuildContext.SourceFiles.Add(destinationFileName);
      context.BuildContext.SourceMap.Add(destinationFileName, fileName);
    }

    [NotNull]
    protected XmlSchemaSet GetSchemas([NotNull] IPreprocessingContext context)
    {
      if (schemas != null)
      {
        return schemas;
      }

      var schemaFileName = Path.Combine(context.BuildContext.Configuration.Get(Constants.ToolsPath), "schemas\\item.xsd");

      schemas = new XmlSchemaSet();
      schemas.Add("http://www.sitecore.net/pathfinder/item", schemaFileName);

      return schemas;
    }

    protected void Validate([NotNull] IPreprocessingContext context, [NotNull] string fileName)
    {
      try
      {
        var doc = XDocument.Load(fileName, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

        ValidationEventHandler validate = delegate(object sender, ValidationEventArgs args)
        {
          switch (args.Severity)
          {
            case XmlSeverityType.Error:
              context.BuildContext.Trace.TraceError(ConsoleTexts.Text3013, fileName, 0, 0, args.Message);
              context.BuildContext.IsDeployable = false;
              break;
            case XmlSeverityType.Warning:
              context.BuildContext.Trace.TraceWarning(ConsoleTexts.Text3014, fileName, 0, 0, args.Message);
              break;
          }
        };

        var s = this.GetSchemas(context);
        doc.Validate(s, validate);
      }
      catch (Exception ex)
      {
        context.BuildContext.Trace.TraceError(ConsoleTexts.Text3012, fileName, 0, 0, ex.Message);
        context.BuildContext.IsDeployable = false;
      }
    }
  }
}
