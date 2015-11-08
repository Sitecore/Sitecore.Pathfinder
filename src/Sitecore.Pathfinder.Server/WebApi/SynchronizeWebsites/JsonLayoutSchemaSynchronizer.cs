// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.SecurityModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.WebApi.SynchronizeWebsites
{
    public class JsonLayoutSchemaSynchronizer : ISynchronizer
    {
        public bool CanSynchronize(IConfiguration configuration, string fileName)
        {
            return fileName.EndsWith(".layout.schema.json", StringComparison.OrdinalIgnoreCase);
        }

        public void Synchronize(IConfiguration configuration, ZipWriter zip, string fileName, string configKey)
        {
            var databaseName = configuration.Get(configKey + "database");

            Synchronize(zip, fileName, databaseName);
        }

        protected virtual void Synchronize([NotNull] ZipWriter zip, [Diagnostics.NotNull] string fileName, [NotNull] string databaseName)
        {
            var schema = Synchronize(databaseName);
            zip.AddEntry(fileName, Encoding.UTF8.GetBytes(schema));
        }

        [NotNull]
        protected virtual string Synchronize([NotNull] string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);
            if (database == null)
            {
                throw new Exception("Database not found");
            }

            using (new SecurityDisabler())
            {
                var renderingItems = database.GetItemsByTemplate(ServerConstants.Renderings.ViewRenderingId, TemplateIDs.XSLRendering, TemplateIDs.Sublayout, ServerConstants.Renderings.WebcontrolRendering, ServerConstants.Renderings.UrlRendering, ServerConstants.Renderings.MethodRendering).GroupBy(i => i.Name).Select(i => i.First()).OrderBy(i => i.Name).ToList();
                var deviceNames = database.GetItem(ItemIDs.DevicesRoot)?.Children.Select(i => i.Name).ToList() ?? new List<string>();

                var writer = new StringWriter();
                var output = new JsonTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                WriteSchema(output, deviceNames, renderingItems);

                return writer.ToString();
            }
        }

        protected virtual void WriteAttributeString([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] string name, [Diagnostics.NotNull] string type, [Diagnostics.NotNull] string description)
        {
            output.WriteStartObject(name);
            output.WritePropertyString("type", type);

            if (!string.IsNullOrEmpty(description))
            {
                output.WritePropertyString("description", description);
            }

            output.WriteEndObject();
        }

        protected virtual bool WriteDropListAttribute([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] TemplateField field, [Diagnostics.NotNull] Sitecore.Text.UrlString urlString, [Diagnostics.NotNull] string bindmode)
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

        protected virtual void WriteLayout([NotNull] JsonTextWriter output, [ItemNotNull] [Diagnostics.NotNull] List<string> deviceNames)
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

            output.WriteStartObject("Name");
            output.WritePropertyString("description", "The name of the device.");
            output.WriteStartArray("enum");

            foreach (var deviceName in deviceNames)
            {
                output.WriteValue(deviceName);
            }

            output.WriteEndArray();
            output.WriteEndObject();


            output.WriteObjectString("Layout", "type", "string");
            output.WriteObjectString("Renderings", "$ref", "#/definitions/Renderings");
            output.WriteEndObject(); // properties

            output.WriteEndObject(); // items
            output.WriteEndObject(); // Devices

            output.WriteEndObject(); // properties

            output.WriteEndObject(); // Layout
        }

        protected virtual void WriteRenderingParameters([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Item rendering)
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

                var urlString = new Sitecore.Text.UrlString(field.Source);
                var bindmode = (urlString["bindmode"] ?? string.Empty).ToLowerInvariant();

                if (string.Compare(field.Type, "droplist", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (!WriteDropListAttribute(output, rendering.Database, field, urlString, bindmode))
                    {
                        WriteAttributeString(output, fieldName, "string", field.GetToolTip(LanguageManager.DefaultLanguage));
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

                    WriteAttributeString(output, fieldName, type, description);
                }
            }
        }

        protected virtual void WriteRenderings([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull][ItemNotNull] IEnumerable<Item> renderingItems)
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

                WriteRenderingParameters(output, rendering);
                WriteStandardAttributes(output);
                output.WriteObjectString("Renderings", "$ref", "#/definitions/Renderings");

                output.WriteEndObject(); // properties

                output.WriteEndObject();

                output.WriteEndObject(); // properties

                output.WriteEndObject();
            }
        }

        protected virtual void WriteRenderingsDefinition([NotNull] JsonTextWriter output, [NotNull][ItemNotNull] IEnumerable<Item> renderingItems)
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

        protected virtual void WriteSchema([NotNull] JsonTextWriter output, [ItemNotNull] [Diagnostics.NotNull] List<string> deviceNames, [NotNull][ItemNotNull] IEnumerable<Item> renderingItems)
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
            WriteRenderingsDefinition(output, renderingItems);
            WriteRenderings(output, renderingItems);
            WriteLayout(output, deviceNames);
            output.WriteEndObject();

            output.WriteEndObject();
        }

        protected virtual void WriteStandardAttributes([NotNull] JsonTextWriter output)
        {
            WriteAttributeString(output, "Placeholder", "string", "The place holder where this rendering will be rendered.");
            WriteAttributeString(output, "DataSource", "string", "The data source.");
            WriteAttributeString(output, "Cacheable", "boolean", "Indicates if the output will be cached.");
            WriteAttributeString(output, "VaryByData", "boolean", "Indicates if the cached output will vary by the data source.");
            WriteAttributeString(output, "VaryByDevice", "boolean", "Indicates if the cached output will vary by the current device.");
            WriteAttributeString(output, "VaryByLogin", "boolean", "Indicates if the cached output will vary by the login.");
            WriteAttributeString(output, "VaryByParameters", "boolean", "Indicates if the cached output will vary by the parameters.");
            WriteAttributeString(output, "VaryByQueryString", "boolean", "Indicates if the cached output will vary by the query string.");
            WriteAttributeString(output, "VaryByUser", "boolean", "Indicates if the cached output will vary by the user.");
        }
    }
}
