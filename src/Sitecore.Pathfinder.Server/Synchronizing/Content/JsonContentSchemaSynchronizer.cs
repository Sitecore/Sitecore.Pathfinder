// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Rocks.Server.Extensibility.Composition;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing.Content
{
    [Export(typeof(ISynchronizer))]
    public class JsonContentSchemaSynchronizer : ISynchronizer
    {
        protected static readonly ID InsertOptionsFieldId = new ID(Constants.Fields.InsertOptionsFieldId);

        public bool CanSynchronize(Microsoft.Framework.ConfigurationModel.Configuration configuration, string fileName)
        {
            return fileName.EndsWith(".content.schema.json", StringComparison.OrdinalIgnoreCase);
        }

        public void Synchronize(Microsoft.Framework.ConfigurationModel.Configuration configuration, ZipWriter zip, string fileName, string configKey)
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
                var templates = TemplateManager.GetTemplates(database).Values.Where(t => t.Name != "__Standard Values").GroupBy(r => r.Name).Select(group => group.First()).OrderBy(r => r.Name).ToList();

                var writer = new StringWriter();
                var output = new JsonTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                WriteSchema(output, database, templates);

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

        protected virtual void WriteChildren([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
        {
            BranchItem[] insertOptions = null;

            var standardValueHolderId = template.StandardValueHolderId;
            if (!ID.IsNullOrEmpty(standardValueHolderId))
            {
                var standardValuesItem = database.GetItem(standardValueHolderId);
                if (standardValuesItem != null)
                {
                    insertOptions = standardValuesItem.Branches;
                }
            }

            output.WriteStartObject("Children");

            if (insertOptions != null && insertOptions.Length > 0)
            {
                output.WritePropertyString("type", "array");
                output.WritePropertyString("description", "Contains the child items.");
                output.WriteStartObject("items");

                output.WriteStartArray("oneOf");

                foreach (var insertOption in insertOptions)
                {
                    output.WriteStartObject();
                    output.WritePropertyString("$ref", "#/definitions/" + insertOption.Name);
                    output.WriteEndObject();
                }

                output.WriteEndArray();
                output.WriteEndObject();
            }
            else
            {
                output.WritePropertyString("$ref", "#/definitions/Items");
            }

            output.WriteEndObject();
        }

        protected virtual void WriteDropListProperty([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] TemplateField field, bool useId)
        {
            database = LookupSources.GetDatabase(field.Source) ?? database;
            var items = LookupSources.GetItems(database.GetRootItem(), field.Source);

            output.WriteStartObject(field.Name);

            var description = field.GetToolTip(LanguageManager.DefaultLanguage);
            if (!string.IsNullOrEmpty(description))
            {
                output.WritePropertyString("description", description);
            }

            output.WriteStartArray("enum");

            foreach (var child in items)
            {
                output.WriteValue(useId ? child.ID.ToString() : child.Name);
            }

            output.WriteEndArray();
            output.WriteEndObject();
        }

        protected virtual void WriteItemPath([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
        {
            var item = database.GetItem(template.ID);
            if (item == null)
            {
                return;
            }

            var itemLinks = Globals.LinkDatabase.GetReferrers(item).Where(l => l.SourceFieldID == InsertOptionsFieldId).ToList();
            if (!itemLinks.Any())
            {
                return;
            }

            var links = new List<string>();

            foreach (var sourceItem in itemLinks.Select(l => l.GetSourceItem()).Where(i => i != null))
            {
                if (!StandardValuesManager.IsStandardValuesHolder(sourceItem))
                {
                    links.Add(sourceItem.Paths.Path);
                    continue;
                }

                var templateItem = database.GetItem(sourceItem.TemplateID);
                if (templateItem == null)
                {
                    continue;
                }

                var templateLinks = Globals.LinkDatabase.GetReferrers(templateItem).Where(l => l.SourceFieldID == InsertOptionsFieldId).ToList();
                foreach (var templateSourceItem in templateLinks.Select(l => l.GetSourceItem()).Where(i => i != null && i.ID != sourceItem.ID && !StandardValuesManager.IsStandardValuesHolder(i)))
                {
                    links.Add(templateSourceItem.Paths.Path);
                }
            }

            if (!links.Any())
            {
                return;
            }

            output.WriteStartObject("ParentItemPath");
            output.WritePropertyString("description", "The path of the parent item.");

            output.WriteStartArray("enum");

            foreach (var link in links.Distinct().OrderBy(l => l))
            {
                output.WriteValue(link);
            }

            output.WriteEndArray();
            output.WriteEndObject();
        }

        protected virtual void WriteItemsDefinition([NotNull] JsonTextWriter output, [NotNull] IEnumerable<Template> templates)
        {
            output.WriteStartObject("Items");
            output.WritePropertyString("type", "array");

            output.WriteStartObject("items");

            output.WriteStartArray("oneOf");

            foreach (var template in templates)
            {
                output.WriteStartObject();
                output.WritePropertyString("$ref", "#/definitions/" + template.Name);
                output.WriteEndObject();
            }

            output.WriteEndArray();

            output.WriteEndObject(); // items

            output.WriteEndObject(); // Items
        }

        protected virtual void WriteSchema([NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [NotNull] IEnumerable<Template> templates)
        {
            output.WriteStartObject();

            output.WritePropertyString("$schema", "http://json-schema.org/draft-04/schema#");
            output.WritePropertyString("type", "array");

            output.WritePropertyString("additionalProperties", false);

            output.WriteStartArray("oneOf");
            output.WriteStartObject();
            output.WritePropertyString("$ref", "#/definitions/Items");
            output.WriteEndObject();

            output.WriteEndArray();

            output.WriteStartObject("definitions");
            WriteItemsDefinition(output, templates);
            WriteTemplates(output, database, templates);
            output.WriteEndObject();

            output.WriteEndObject();
        }

        protected virtual void WriteStandardAttributes([NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
        {
            WriteAttributeString(output, "Name", "string", "The name of the item.");
            WriteAttributeString(output, "Id", "string", "The ID of the item.");

            WriteItemPath(output, database, template);

            WriteAttributeString(output, "__Icon", "string", "The icon that represents this template.");
        }

        protected virtual void WriteTemplateFields([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
        {
            var fieldNames = new List<string>();
            fieldNames.Add("Name");
            fieldNames.Add("Id");
            fieldNames.Add("ParentItemPath");
            fieldNames.Add("__Icon");
            fieldNames.Add("Children");

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

                output.WriteStartObject(fieldName);

                switch (field.Type.ToLowerInvariant())
                {
                    case "checkbox":
                        output.WritePropertyString("type", "boolean");
                        break;
                    case "integer":
                        output.WritePropertyString("type", "integer");
                        break;
                    case "valuelookup":
                    case "droplist":
                        WriteDropListProperty(output, database, field, false);
                        break;
                    case "lookup":
                    case "droplink":
                        WriteDropListProperty(output, database, field, true);
                        break;
                    default:
                        output.WritePropertyString("type", "string");
                        break;
                }

                var description = field.GetToolTip(LanguageManager.DefaultLanguage);
                if (!string.IsNullOrEmpty(description))
                {
                    output.WritePropertyString("description", description);
                }

                output.WriteEndObject();
            }
        }

        protected virtual void WriteTemplates([Diagnostics.NotNull] JsonTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] IEnumerable<Template> templates)
        {
            foreach (var template in templates)
            {
                output.WriteStartObject(template.Name);
                output.WritePropertyString("type", "object");
                output.WritePropertyString("additionalProperties", false);
                output.WritePropertyString("additionalItems", false);

                output.WriteStartObject("properties");

                output.WriteStartObject(template.Name);
                output.WritePropertyString("type", "object");

                output.WritePropertyString("additionalProperties", false);
                output.WritePropertyString("additionalItems", false);

                output.WriteStartObject("properties");

                WriteStandardAttributes(output, database, template);
                WriteTemplateFields(output, database, template);

                WriteChildren(output, database, template);

                output.WriteEndObject(); // properties

                output.WriteEndObject();

                output.WriteEndObject(); // properties

                output.WriteEndObject();
            }
        }
    }
}
