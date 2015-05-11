namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ParserBase : IParser
  {
    protected const double BinFiles = 9999;

    protected const double ContentFiles = 9000;

    protected const double Items = 3000;

    protected const double Media = 2000;

    protected const double Renderings = 5000;

    protected const double System = 100;

    protected const double Templates = 1000;

    private static readonly Dictionary<string, XmlSchemaSet> Schemas = new Dictionary<string, XmlSchemaSet>();

    protected ParserBase(double sortorder)
    {
      this.Sortorder = sortorder;
    }

    public double Sortorder { get; }

    public abstract bool CanParse(IParseContext context);

    public abstract void Parse(IParseContext context);

    [CanBeNull]
    protected virtual XmlSchemaSet GetSchema([NotNull] IParseContext context, [NotNull] string schemaFileName, [NotNull] string schemaNamespace)
    {
      var fileName = Path.Combine(context.Configuration.Get(Constants.ToolsDirectory), "schemas\\" + schemaFileName);
      if (!context.Project.FileSystem.FileExists(fileName))
      {
        return null;
      }

      var schemas = new XmlSchemaSet();
      schemas.Add(schemaNamespace, fileName);

      return schemas;
    }

    protected virtual void ValidateXmlSchema([NotNull] IParseContext context, [NotNull] XElement root, [NotNull] string schemaNamespace, [NotNull] string schemaFileName)
    {
      var doc = root.Document;
      if (doc == null)
      {
        return;
      }

      XmlSchemaSet schema;
      if (!Schemas.TryGetValue(schemaNamespace, out schema))
      {
        schema = this.GetSchema(context, schemaFileName, schemaNamespace);
        Schemas[schemaNamespace] = schema;
      }

      if (schema == null)
      {
        return;
      }

      ValidationEventHandler validate = delegate(object sender, ValidationEventArgs args)
      {
        switch (args.Severity)
        {
          case XmlSeverityType.Error:
            context.Project.Trace.TraceError(Texts.Text3013, context.SourceFile.SourceFileName, args.Exception.LineNumber, args.Exception.LinePosition, args.Message);
            break;
          case XmlSeverityType.Warning:
            context.Project.Trace.TraceWarning(Texts.Text3014, context.SourceFile.SourceFileName, args.Exception.LineNumber, args.Exception.LinePosition, args.Message);
            break;
        }
      };

      try
      {
        doc.Validate(schema, validate);
      }
      catch (Exception ex)
      {
        context.Project.Trace.TraceError(Texts.Text3012, context.SourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }
  }
}
