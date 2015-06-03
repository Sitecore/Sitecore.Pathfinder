namespace Sitecore.Pathfinder.Resources
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Xml;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.SecurityModel;
  using Sitecore.Text;

  public class XsdSchemaGenerator
  {
    public const string Namespace = "http://www.w3.org/2001/XMLSchema";

    public const string RenderingIdsFastQuery = "@@templateid='{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}' or " + "@@templateid='{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}' or " + "@@templateid='{86776923-ECA5-4310-8DC0-AE65FE88D078}' or " + "@@templateid='{39587D7D-F06D-4CB4-A25E-AA7D847EDDD0}' or " + "@@templateid='{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}' or " + "@@templateid='{83E993C5-C0FC-4472-86A9-2F6CFED694E4}' or " + "@@templateid='{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}' or " + "@@templateid='{F1F1D639-4F54-40C2-8BE0-81266B392CEB}'";

    public static readonly List<string> RenderingIds = new List<string>
    {
      "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}", 
      "{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}", 
      "{86776923-ECA5-4310-8DC0-AE65FE88D078}", 
      "{39587D7D-F06D-4CB4-A25E-AA7D847EDDD0}", 
      "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}",                                        
      "{83E993C5-C0FC-4472-86A9-2F6CFED694E4}", 
      "{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}", 
      "{F1F1D639-4F54-40C2-8BE0-81266B392CEB}"
    };

    [NotNull]
    public string Generate([NotNull] string schemaNamespace, [NotNull] string databaseName)
    {
      var database = Factory.GetDatabase(databaseName);
      if (database == null)
      {
        throw new Exception("Database not found");
      }

      using (new SecurityDisabler())
      {
        var renderingItems = database.SelectItems("//*[" + RenderingIdsFastQuery + "]").OrderBy(r => r.Name).ToList();

        var writer = new StringWriter();
        var output = new XmlTextWriter(writer)
        {
          Formatting = Formatting.Indented
        };

        this.WriteSchema(output, schemaNamespace, renderingItems);

        return writer.ToString();
      }
    }

    [NotNull]
    private string GetUniqueRenderingName([NotNull] IEnumerable<Item> renderings, [NotNull] Item rendering)
    {
      var paths = renderings.Where(r => r.Name == rendering.Name && r != rendering).Select(r => r.Paths.Path).ToList();
      var parts = rendering.Paths.Path.Split('/');

      var result = string.Empty;

      for (var i = parts.Length - 1; i >= 0; i--)
      {
        result = "/" + parts[i] + result;

        var path = result;
        if (!paths.Any(p => p.EndsWith(path, StringComparison.InvariantCultureIgnoreCase)))
        {
          break;
        }
      }

      result = result.Mid(1).Replace("/", ".");
      return result;
    }

    private void WriteAttributeString([NotNull] XmlTextWriter output, [NotNull] string name, [NotNull] string type, [NotNull] string help)
    {
      output.WriteStartElement("xs", "attribute", Namespace);
      output.WriteAttributeString("name", name);
      output.WriteAttributeString("type", type);

      if (!string.IsNullOrEmpty(help))
      {
        output.WriteStartElement("xs", "annotation", Namespace);
        output.WriteStartElement("xs", "documentation", Namespace);
        output.WriteValue(help);
        output.WriteEndElement();
        output.WriteEndElement();
      }

      output.WriteEndElement();
    }

    private void WriteBindingSimpleType([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "simpleType", Namespace);

      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:string");

      output.WriteStartElement("xs", "pattern", Namespace);
      output.WriteAttributeString("value", "\\{Binding.*\\}");
      output.WriteEndElement();

      output.WriteEndElement(); // restriction
      output.WriteEndElement(); // simpletype

      output.WriteStartElement("xs", "simpleType", Namespace);

      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:string");

      output.WriteStartElement("xs", "pattern", Namespace);
      output.WriteAttributeString("value", "\\{\\@.*\\}");
      output.WriteEndElement();

      output.WriteEndElement(); // restriction
      output.WriteEndElement(); // simpletype
    }

    private void WriteBoolSimpleType([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "simpleType", Namespace);
      output.WriteAttributeString("name", "bool");

      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:string");

      output.WriteStartElement("xs", "enumeration", Namespace);
      output.WriteAttributeString("value", "False");
      output.WriteEndElement();

      output.WriteStartElement("xs", "enumeration", Namespace);
      output.WriteAttributeString("value", "True");
      output.WriteEndElement();

      output.WriteEndElement(); // restriction
      output.WriteEndElement(); // simpletype

      output.WriteStartElement("xs", "simpleType", Namespace);
      output.WriteAttributeString("name", "bindablebool");

      output.WriteStartElement("xs", "union", Namespace);

      output.WriteStartElement("xs", "simpleType", Namespace);

      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:string");

      output.WriteStartElement("xs", "enumeration", Namespace);
      output.WriteAttributeString("value", "False");
      output.WriteEndElement();

      output.WriteStartElement("xs", "enumeration", Namespace);
      output.WriteAttributeString("value", "True");
      output.WriteEndElement();

      output.WriteEndElement(); // restriction
      output.WriteEndElement(); // simpletype

      this.WriteBindingSimpleType(output);

      output.WriteEndElement(); // union
      output.WriteEndElement(); // simpletype
    }

    private void WriteDevices([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "element", Namespace);
      output.WriteAttributeString("name", "Device");

      output.WriteStartElement("xs", "complexType", Namespace);

      this.WriterRenderingSequence(output);

      this.WriteAttributeString(output, "Name", "xs:string", "The name of the device");
      this.WriteAttributeString(output, "Layout", "xs:string", "The path to the layout.");

      output.WriteEndElement();

      output.WriteStartElement("xs", "unique", Namespace);
      output.WriteAttributeString("name", "IdKey");

      output.WriteStartElement("xs", "selector", Namespace);
      output.WriteAttributeString("xpath", ".//*");
      output.WriteEndElement();

      output.WriteStartElement("xs", "field", Namespace);
      output.WriteAttributeString("xpath", "@Id");
      output.WriteEndElement();

      output.WriteEndElement();

      output.WriteEndElement();
    }

    private bool WriteDropListAttribute([NotNull] XmlTextWriter output, [NotNull] Database database, [NotNull] TemplateField field, [NotNull] UrlString urlString, [NotNull] string bindmode)
    {
      var itemId = ID.IsID(field.Source) ? field.Source : urlString["datasource"];

      if (string.IsNullOrEmpty(itemId))
      {
        return false;
      }

      var item = database.GetItem(itemId);
      if (item == null)
      {
        return false;
      }

      var fieldName = field.Name.GetSafeCodeIdentifier();

      output.WriteStartElement("xs", "attribute", Namespace);
      output.WriteAttributeString("name", fieldName);

      var help = field.GetToolTip(LanguageManager.DefaultLanguage);
      if (!string.IsNullOrEmpty(help))
      {
        output.WriteStartElement("xs", "annotation", Namespace);
        output.WriteStartElement("xs", "documentation", Namespace);
        output.WriteValue(help);
        output.WriteEndElement();
        output.WriteEndElement();
      }

      output.WriteStartElement("xs", "simpleType", Namespace);

      if (bindmode == "server" || bindmode == "read")
      {
        this.WriteDropListAttributeEnumeration(output, item);
      }
      else
      {
        output.WriteStartElement("xs", "union", Namespace);

        output.WriteStartElement("xs", "simpleType", Namespace);
        this.WriteDropListAttributeEnumeration(output, item);
        output.WriteEndElement();

        this.WriteBindingSimpleType(output);

        output.WriteEndElement();
      }

      output.WriteEndElement();

      output.WriteEndElement();

      return true;
    }

    private void WriteDropListAttributeEnumeration([NotNull] XmlTextWriter output, [NotNull] Item item)
    {
      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:string");

      foreach (Item child in item.Children)
      {
        output.WriteStartElement("xs", "enumeration", Namespace);
        output.WriteAttributeString("value", child.Name);
        output.WriteEndElement();
      }

      output.WriteEndElement();
    }

    private void WriteGenericRendering([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "element", Namespace);

      output.WriteAttributeString("name", "Rendering");

      output.WriteStartElement("xs", "complexType", Namespace);

      this.WriterRenderingSequence(output);

      this.WriteAttributeString(output, "RenderingName", "xs:string", "The full name of the rendering.");

      output.WriteStartElement("xs", "attributeGroup", Namespace);
      output.WriteAttributeString("ref", "stdattr");
      output.WriteEndElement();

      output.WriteStartElement("xs", "anyAttribute", Namespace);
      output.WriteAttributeString("processContents", "lax");
      output.WriteEndElement();

      output.WriteEndElement();
      output.WriteEndElement();
    }

    private void WriteIntSimpleType([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "simpleType", Namespace);
      output.WriteAttributeString("name", "bindableint");

      output.WriteStartElement("xs", "union", Namespace);

      output.WriteStartElement("xs", "simpleType", Namespace);

      output.WriteStartElement("xs", "restriction", Namespace);
      output.WriteAttributeString("base", "xs:int");

      output.WriteEndElement(); // restriction
      output.WriteEndElement(); // simpletype

      this.WriteBindingSimpleType(output);

      output.WriteEndElement(); // union
      output.WriteEndElement(); // simpletype
    }

    private void WriteLayout([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "element", Namespace);
      output.WriteAttributeString("name", "Layout");

      output.WriteStartElement("xs", "complexType", Namespace);

      output.WriteStartElement("xs", "sequence", Namespace);
      output.WriteAttributeString("minOccurs", "1");
      output.WriteAttributeString("maxOccurs", "unbounded");

      this.WriteDevices(output);

      output.WriteEndElement();

      this.WriteAttributeString(output, "ItemId", "xs:string", "The ID of the item where this layout is saved.");

      output.WriteEndElement();

      output.WriteEndElement();
    }

    private void WriteRendering([NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems, [NotNull] Item rendering)
    {
      var name = rendering.Name;
      if (renderingItems.Any(r => r.Name == name && r != rendering))
      {
        name = this.GetUniqueRenderingName(renderingItems, rendering);
      }

      var isValidName = Regex.IsMatch(name, "^[a-zA-Z0-9_\\-\\.]+$");
      if (!isValidName)
      {
        return;
      }

      output.WriteStartElement("xs", "element", Namespace);
      output.WriteAttributeString("name", name);

      var help = rendering.Help.ToolTip;
      if (!string.IsNullOrEmpty(help))
      {
        output.WriteStartElement("xs", "annotation", Namespace);
        output.WriteStartElement("xs", "documentation", Namespace);
        output.WriteValue(help);
        output.WriteEndElement();
        output.WriteEndElement();
      }

      output.WriteStartElement("xs", "complexType", Namespace);

      this.WriterRenderingSequence(output);

      var hasRenderingParameters = this.WriteRenderingParameters(output, rendering);

      output.WriteStartElement("xs", "attributeGroup", Namespace);
      output.WriteAttributeString("ref", "stdattr");
      output.WriteEndElement();

      if (!hasRenderingParameters)
      {
        output.WriteStartElement("xs", "anyAttribute", Namespace);
        output.WriteAttributeString("processContents", "lax");
        output.WriteEndElement();
      }

      output.WriteEndElement();

      output.WriteEndElement();
    }

    private bool WriteRenderingParameters([NotNull] XmlTextWriter output, [NotNull] Item rendering)
    {
      var parametersTemplateItemId = rendering["Parameters Template"];
      if (string.IsNullOrEmpty(parametersTemplateItemId))
      {
        return false;
      }

      var parametersTemplateItem = rendering.Database.GetItem(parametersTemplateItemId);
      if (parametersTemplateItem == null)
      {
        return false;
      }

      var fieldNames = new List<string>();

      var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, rendering.Database);
      foreach (var field in template.GetFields(true).OrderBy(f => f.Name))
      {
        if (field.Template.BaseIDs.Length == 0)
        {
          continue;
        }

        var fieldName = field.Name.GetSafeCodeIdentifier();
        if (fieldNames.Contains(fieldName))
        {
          continue;
        }

        fieldNames.Add(fieldName);

        var urlString = new UrlString(field.Source);
        var bindmode = (urlString["bindmode"] ?? string.Empty).ToLowerInvariant();

        if (string.Compare(field.Type, "droplist", StringComparison.InvariantCultureIgnoreCase) == 0)
        {
          if (!this.WriteDropListAttribute(output, rendering.Database, field, urlString, bindmode))
          {
            this.WriteAttributeString(output, fieldName, "xs:string", field.GetToolTip(LanguageManager.DefaultLanguage));
          }
        }
        else
        {
          var type = "xs:string";
          switch (field.Type.ToLowerInvariant())
          {
            case "checkbox":
              type = "bindablebool";
              break;

            case "integer":
              type = bindmode == "server" || bindmode == "read" ? "xs:int" : "bindableint";
              break;
          }

          this.WriteAttributeString(output, fieldName, type, field.GetToolTip(LanguageManager.DefaultLanguage));
        }
      }

      return true;
    }

    private void WriteRenderings([NotNull] XmlTextWriter output, [NotNull] IEnumerable<Item> renderingItems)
    {
      output.WriteStartElement("xs", "group", Namespace);
      output.WriteAttributeString("name", "rendering");

      output.WriteStartElement("xs", "choice", Namespace);

      foreach (var rendering in renderingItems)
      {
        this.WriteRendering(output, renderingItems, rendering);
      }

      this.WriteGenericRendering(output);

      output.WriteEndElement();
      output.WriteEndElement();
    }

    private void WriterRenderingSequence([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "sequence", Namespace);
      output.WriteAttributeString("minOccurs", "0");
      output.WriteAttributeString("maxOccurs", "unbounded");

      output.WriteStartElement("xs", "group", Namespace);
      output.WriteAttributeString("ref", "rendering");
      output.WriteEndElement();

      output.WriteEndElement();
    }

    private void WriteSchema([NotNull] XmlTextWriter output, [NotNull] string nameSpace, [NotNull] IEnumerable<Item> renderingItems)
    {                                                                          
      output.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
      output.WriteStartElement("xs", "schema", Namespace);

      output.WriteAttributeString("targetNamespace", nameSpace);
      output.WriteAttributeString("xmlns", nameSpace);
      output.WriteAttributeString("xmlns", "vs", "http://www.w3.org/2000/xmlns/", "http://schemas.microsoft.com/Visual-Studio-Intellisense");
      output.WriteAttributeString("vs", "friendlyname", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "Sitecore SPEAK Intellisense");
      output.WriteAttributeString("vs", "ishtmlschema", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "false");
      output.WriteAttributeString("vs", "iscasesensitive", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "true");
      output.WriteAttributeString("vs", "requireattributequotes", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "true");
      output.WriteAttributeString("elementFormDefault", "qualified");

      this.WriteBoolSimpleType(output);
      this.WriteIntSimpleType(output);

      this.WriteStandardAttributes(output);

      this.WriteRenderings(output, renderingItems);

      this.WriteLayout(output);

      output.WriteEndElement();
    }

    private void WriteStandardAttributes([NotNull] XmlTextWriter output)
    {
      output.WriteStartElement("xs", "attributeGroup", Namespace);
      output.WriteAttributeString("name", "stdattr");

      this.WriteAttributeString(output, "Placeholder", "xs:string", "The place holder where this rendering will be rendered.");
      this.WriteAttributeString(output, "DataSource", "xs:string", "The data source.");
      this.WriteAttributeString(output, "Cacheable", "bool", "Indicates if the output will be cached.");
      this.WriteAttributeString(output, "VaryByData", "bool", "Indicates if the cached output will vary by the data source.");
      this.WriteAttributeString(output, "VaryByDevice", "bool", "Indicates if the cached output will vary by the current device.");
      this.WriteAttributeString(output, "VaryByLogin", "bool", "Indicates if the cached output will vary by the login.");
      this.WriteAttributeString(output, "VaryByParameters", "bool", "Indicates if the cached output will vary by the parameters.");
      this.WriteAttributeString(output, "VaryByQueryString", "bool", "Indicates if the cached output will vary by the query string.");
      this.WriteAttributeString(output, "VaryByUser", "bool", "Indicates if the cached output will vary by the user.");

      output.WriteEndElement();
    }
  }
}
