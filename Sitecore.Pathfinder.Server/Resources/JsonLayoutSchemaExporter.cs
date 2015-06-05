namespace Sitecore.Pathfinder.Resources
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using System.Text;
  using Newtonsoft.Json;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.SecurityModel;
  using Sitecore.Text;
  using Sitecore.Zip;

  [Export(typeof(IResourceExporter))]
  public class JsonLayoutSchemaExporter : IResourceExporter
  {
    public void Export(ZipWriter zip)
    {
      this.Generate(zip, "master");
      this.Generate(zip, "core");
    }

    protected virtual void Generate([Sitecore.NotNull] ZipWriter zip, [Sitecore.NotNull] string databaseName)
    {
      var schema = this.Generate(databaseName);
      zip.AddEntry(".schemas\\" + databaseName + ".layout.schema.json", Encoding.UTF8.GetBytes(schema));
    }

    [Sitecore.NotNull]
    protected virtual string Generate([Sitecore.NotNull] string databaseName)
    {
      var database = Factory.GetDatabase(databaseName);
      if (database == null)
      {
        throw new Exception("Database not found");
      }

      using (new SecurityDisabler())
      {
        var renderingItems = database.SelectItems("//*[" + Constants.RenderingIdsFastQuery + "]").GroupBy(r => r.Name).Select(group => group.First()).OrderBy(r => r.Name).ToList();
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

    protected virtual void WriteAttributeString([NotNull] JsonTextWriter output, [NotNull] string name, [NotNull] string type, [NotNull] string description)
    {
      output.WriteStartObject(name);
      output.WritePropertyString("type", type);

      if (!string.IsNullOrEmpty(description))
      {
        output.WritePropertyString("description", description);
      }

      output.WriteEndObject();
    }

    protected virtual bool WriteDropListAttribute([NotNull] JsonTextWriter output, [NotNull] Database database, [NotNull] TemplateField field, [NotNull] UrlString urlString, [NotNull] string bindmode)
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

    protected virtual void WriteLayout([Sitecore.NotNull] JsonTextWriter output)
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

    protected virtual void WriteRenderingParameters([NotNull] JsonTextWriter output, [NotNull] Item rendering)
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

    protected virtual void WriteRenderings([NotNull] JsonTextWriter output, [NotNull] IEnumerable<Item> renderingItems)
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

    protected virtual void WriteRenderingsDefinition([Sitecore.NotNull] JsonTextWriter output, [Sitecore.NotNull] IEnumerable<Item> renderingItems)
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

    protected virtual void WriteSchema([Sitecore.NotNull] JsonTextWriter output, [Sitecore.NotNull] IEnumerable<Item> renderingItems)
    {
      output.WriteStartObject();

      output.WritePropertyString("$schema", "http://json-schema.org/draft-04/schema#");
      output.WritePropertyString("type", "object");

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

    protected virtual void WriteStandardAttributes([Sitecore.NotNull] JsonTextWriter output)
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
