namespace Sitecore.Pathfinder.Resources
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Newtonsoft.Json;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.JsonTextWriterExtensions;
  using Sitecore.SecurityModel;
  using Sitecore.Text;

  public class JsonSchemaGenerator
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

    [Sitecore.NotNull]
    public string Generate([Sitecore.NotNull] string databaseName)
    {
      var database = Factory.GetDatabase(databaseName);
      if (database == null)
      {
        throw new Exception("Database not found");
      }

      using (new SecurityDisabler())
      {
        var renderingItems = database.SelectItems("//*[" + RenderingIdsFastQuery + "]").GroupBy(r => r.Name).Select(group => group.First()).OrderBy(r => r.Name).ToList();
        renderingItems.RemoveAll(r => StandardValuesManager.IsStandardValuesHolder(r) || r.Name == "$name");

        var writer = new StringWriter();
        var output = new JsonTextWriter(writer)
        {
          Formatting = Formatting.Indented
        };

        this.WriteSchema(output, renderingItems);

        return writer.ToString();
      }
    }

    private void WriteAttributeString([NotNull] JsonTextWriter output, [NotNull] string name, [NotNull] string type, [NotNull] string description)
    {
      output.WriteStartObject(name);
      output.WritePropertyString("type", type);

      if (!string.IsNullOrEmpty(description))
      {
        output.WritePropertyString("description", description);
      }

      output.WriteEndObject();
    }

    private bool WriteDropListAttribute([NotNull] JsonTextWriter output, [NotNull] Database database, [NotNull] TemplateField field, [NotNull] UrlString urlString, [NotNull] string bindmode)
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

      // todo: support SPEAK bindings
      output.WriteStartObject(field.Name);

      var description = field.GetToolTip(LanguageManager.DefaultLanguage);
      if (!string.IsNullOrEmpty(description))
      {
        output.WritePropertyString("description", description);
      }

      output.WriteStartArray("enum");

      foreach (Item child in item.Children)
      {
        output.WriteValue(child.Name);
      }

      output.WriteEndArray();
      output.WriteEndObject();

      return true;
    }

    private void WriteLayout([Sitecore.NotNull] JsonTextWriter output)
    {
      output.WriteStartObject("Layout");
      output.WritePropertyString("type", "object");
      output.WritePropertyString("additionalProperties", false);

      output.WriteStartObject("properties");

      output.WriteStartObject("Devices");
      output.WritePropertyString("type", "array");

      output.WriteStartObject("items");
      output.WritePropertyString("additionalProperties", false);

      output.WriteStartObject("properties");
      output.WriteObjectString("Name", "type", "string");
      output.WriteObjectString("Layout", "type", "string");
      output.WriteObjectString("Renderings", "$ref", "#/definitions/Renderings");
      output.WriteEndObject(); // properties

      output.WriteEndObject(); // items
      output.WriteEndObject(); // Devices

      output.WriteEndObject(); // properties

      output.WriteEndObject(); // Layout
    }

    private void WriteRenderingParameters([NotNull] JsonTextWriter output, [NotNull] Item rendering)
    {
      var parametersTemplateItemId = rendering["Parameters Template"];
      if (string.IsNullOrEmpty(parametersTemplateItemId))
      {
        return;
      }

      var parametersTemplateItem = rendering.Database.GetItem(parametersTemplateItemId);
      if (parametersTemplateItem == null)
      {
        return;
      }

      var fieldNames = new List<string>();
      fieldNames.Add("Placeholder");
      fieldNames.Add("DataSource");

      var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, rendering.Database);
      foreach (var field in template.GetFields(true).OrderBy(f => f.Name))
      {
        if (field.Template.BaseIDs.Length == 0)
        {
          continue;
        }

        var fieldName = field.Name;
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
            this.WriteAttributeString(output, fieldName, "string", field.GetToolTip(LanguageManager.DefaultLanguage));
          }
        }
        else
        {
          // todo: support SPEAK bindings
          var description = field.GetToolTip(LanguageManager.DefaultLanguage);
          var type = "string";
          switch (field.Type.ToLowerInvariant())
          {
            case "checkbox":
              type = "boolean";
              break;

            case "integer":
              type = bindmode == "server" || bindmode == "read" ? "integer" : "string";
              break;
          }

          this.WriteAttributeString(output, fieldName, type, description);
        }
      }
    }

    private void WriteRenderings([NotNull] JsonTextWriter output, [NotNull] IEnumerable<Item> renderingItems)
    {
      foreach (var rendering in renderingItems)
      {
        if (StandardValuesManager.IsStandardValuesHolder(rendering))
        {
          continue;
        }

        if (rendering.Name == "$name")
        {
          continue;
        }

        output.WriteStartObject(rendering.Name);
        output.WritePropertyString("type", "object");
        output.WritePropertyString("additionalProperties", false);
        output.WritePropertyString("additionalItems", false);

        output.WriteStartObject("properties");

        output.WriteStartObject(rendering.Name);
        output.WritePropertyString("type", "object");
        if (!string.IsNullOrEmpty(rendering.Help.ToolTip))
        {
          output.WritePropertyString("description", rendering.Help.ToolTip);
        }

        output.WritePropertyString("additionalProperties", false);
        output.WritePropertyString("additionalItems", false);

        output.WriteStartObject("properties");

        this.WriteRenderingParameters(output, rendering);
        this.WriteStandardAttributes(output);
        output.WriteObjectString("Renderings", "$ref", "#/definitions/Renderings");

        output.WriteEndObject(); // properties

        output.WriteEndObject();

        output.WriteEndObject(); // properties

        output.WriteEndObject();
      }
    }

    private void WriteRenderingsDefinition([Sitecore.NotNull] JsonTextWriter output, [Sitecore.NotNull] IEnumerable<Item> renderingItems)
    {
      output.WriteStartObject("Renderings");
      output.WritePropertyString("type", "array");

      output.WriteStartObject("items");

      output.WriteStartArray("oneOf");

      foreach (var rendering in renderingItems)
      {
        output.WriteStartObject();
        output.WritePropertyString("$ref", "#/definitions/" + rendering.Name);
        output.WriteEndObject();
      }

      output.WriteEndArray();

      output.WriteEndObject(); // items

      output.WriteEndObject(); // Renderings
    }

    private void WriteSchema([Sitecore.NotNull] JsonTextWriter output, [Sitecore.NotNull] IEnumerable<Item> renderingItems)
    {
      output.WriteStartObject();

      output.WritePropertyString("$schema", "http://json-schema.org/draft-04/schema#");
      output.WritePropertyString("$type", "object");

      output.WritePropertyString("additionalProperties", false);

      output.WriteStartObject("properties");
      output.WriteObjectString("$schema", "type", "string");
      output.WriteObjectString("Layout", "$ref", "#/definitions/Layout");
      output.WriteEndObject();

      output.WriteStartObject("definitions");
      this.WriteRenderingsDefinition(output, renderingItems);
      this.WriteRenderings(output, renderingItems);
      this.WriteLayout(output);
      output.WriteEndObject();

      output.WriteEndObject();
    }

    private void WriteStandardAttributes([Sitecore.NotNull] JsonTextWriter output)
    {
      this.WriteAttributeString(output, "Placeholder", "string", "The place holder where this rendering will be rendered.");
      this.WriteAttributeString(output, "DataSource", "string", "The data source.");
      this.WriteAttributeString(output, "Cacheable", "boolean", "Indicates if the output will be cached.");
      this.WriteAttributeString(output, "VaryByData", "boolean", "Indicates if the cached output will vary by the data source.");
      this.WriteAttributeString(output, "VaryByDevice", "boolean", "Indicates if the cached output will vary by the current device.");
      this.WriteAttributeString(output, "VaryByLogin", "boolean", "Indicates if the cached output will vary by the login.");
      this.WriteAttributeString(output, "VaryByParameters", "boolean", "Indicates if the cached output will vary by the parameters.");
      this.WriteAttributeString(output, "VaryByQueryString", "boolean", "Indicates if the cached output will vary by the query string.");
      this.WriteAttributeString(output, "VaryByUser", "boolean", "Indicates if the cached output will vary by the user.");
    }
  }
}
