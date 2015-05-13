namespace Sitecore.Pathfinder.Documents.Xml
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class XmlDocument : Document
  {
    protected static readonly Dictionary<string, XmlSchemaSet> Schemas = new Dictionary<string, XmlSchemaSet>();

    private ITreeNode root;

    private XElement rootElement;

    public XmlDocument([NotNull] IParseContext parseContext, [NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.ParseContext = parseContext;
    }

    public override ITreeNode Root => this.root ?? (this.root = this.Parse(null, this.RootElement));

    [NotNull]
    protected IParseContext ParseContext { get; }

    [NotNull]
    protected XElement RootElement => this.rootElement ?? (this.rootElement = this.SourceFile.ReadAsXml(this.ParseContext));

    public override void BeginEdit()
    {
      this.IsEditing = true;
    }

    public override void EndEdit()
    {
      if (!this.IsEditing)
      {
        throw new InvalidOperationException("Document is not in edit mode");
      }

      if (this.root == null)
      {
        return;
      }

      this.IsEditing = false;
      this.rootElement.Save(this.SourceFile.SourceFileName, SaveOptions.DisableFormatting);
    }

    public override void ValidateSchema(IParseContext context, string schemaNamespace, string schemaFileName)
    {
      var doc = this.RootElement.Document;
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
            context.Project.Trace.TraceError(Texts.Text3013, context.Document.SourceFile.SourceFileName, args.Exception.LineNumber, args.Exception.LinePosition, args.Message);
            break;
          case XmlSeverityType.Warning:
            context.Project.Trace.TraceWarning(Texts.Text3014, context.Document.SourceFile.SourceFileName, args.Exception.LineNumber, args.Exception.LinePosition, args.Message);
            break;
        }
      };

      try
      {
        doc.Validate(schema, validate);
      }
      catch (Exception ex)
      {
        context.Project.Trace.TraceError(Texts.Text3012, context.Document.SourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }

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

    [NotNull]
    private ITreeNode Parse([CanBeNull] ITreeNode parent, [NotNull] XElement element)
    {
      var treeNode = new XmlTreeNode(this, element, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var attribute in element.Attributes())
      {
        if (attribute.Name.LocalName == "xmlns")
        {
          continue;
        }

        var attributeTreeNode = new XmlTreeNode(this, attribute);
        treeNode.Attributes.Add(attributeTreeNode);
      }

      foreach (var child in element.Elements())
      {
        this.Parse(treeNode, child);
      }

      return treeNode;
    }
  }
}
