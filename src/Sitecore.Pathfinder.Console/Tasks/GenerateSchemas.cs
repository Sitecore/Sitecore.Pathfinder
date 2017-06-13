// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class GenerateSchemas : BuildTaskBase
    {
        public const string Namespace = "http://www.w3.org/2001/XMLSchema";

        public const string Xs = "xs";

        [ImportingConstructor]
        public GenerateSchemas([NotNull] IFileSystem fileSystem) : base("generate-schemas")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1009, "Generating schemas...");

            var project = context.LoadProject();

            WriteJsonSchema(project);
            // WriteXmlSchema(project);
        }

        [NotNull]
        protected virtual string GetTemplateName([NotNull] Template template)
        {
            return template.ItemName.EscapeXmlElementName();
        }

        [NotNull]
        protected virtual Dictionary<string, Template> GetTemplates([NotNull] IProject project, [NotNull] Database database)
        {
            var templates = new Dictionary<string, Template>();
            foreach (var template in project.ProjectItems.OfType<Template>().Where(i => i.Database == database))
            {
                if (template.ItemName == "Template")
                {
                    continue;
                }

                var itemName = template.ItemName;
                var count = 0;

                while (templates.ContainsKey(itemName))
                {
                    count++;
                    itemName = template.ItemName + count;
                }

                templates[itemName] = template;
            }

            return templates;
        }

        protected virtual void WriteAttributeString([NotNull] XmlWriter output, [NotNull] string name, [NotNull] string type, [NotNull] string help)
        {
            output.WriteStartElement(Xs, "attribute", Namespace);
            output.WriteAttributeString("name", name);
            output.WriteAttributeString("type", type);

            if (!string.IsNullOrEmpty(help))
            {
                output.WriteStartElement(Xs, "annotation", Namespace);
                output.WriteStartElement(Xs, "documentation", Namespace);
                output.WriteValue(help);
                output.WriteEndElement();
                output.WriteEndElement();
            }

            output.WriteEndElement();
        }

        protected virtual void WriteJsonField([NotNull] JsonTextWriter output, [NotNull] TemplateField field)
        {
            output.WriteStartObject(field.FieldName);

            // todo: support for enums (drop downs, lists etc)
            switch (field.Type.ToLowerInvariant())
            {
                case "checkbox":
                    output.WritePropertyString("type", "boolean");
                    break;
                case "integer":
                    output.WritePropertyString("type", "integer");
                    break;
                default:
                    output.WritePropertyString("type", "string");
                    break;
            }

            if (!string.IsNullOrEmpty(field.ShortHelp))
            {
                output.WritePropertyString("description", field.ShortHelp);
            }

            output.WriteEndObject();
        }

        protected virtual void WriteJsonSchema([NotNull] IProject project)
        {
            WriteJsonSchema(project, "master");
            WriteJsonSchema(project, "core");
        }

        protected virtual void WriteJsonSchema([NotNull] IProject project, [NotNull] string databaseName)
        {
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), databaseName + ".schema.json");
            var database = project.GetDatabase(databaseName);

            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    var output = new JsonTextWriter(writer)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4
                    };

                    WriteJsonSchema(output, project, database);
                }
            }
        }

        protected virtual void WriteJsonSchema([NotNull] JsonTextWriter output, [NotNull] IProject project, [NotNull] Database database)
        {
            var pairs = GetTemplates(project, database);
            var languages = database.Languages.ToArray();

            output.WriteStartObject();

            output.WritePropertyString("$schema", "http://json-schema.org/draft-04/schema#");
            output.WritePropertyString("type", "object");

            output.WritePropertyString("additionalProperties", false);

            output.WriteStartObject("properties");
            output.WriteObjectString("$schema", "type", "string");

            foreach (var pair in pairs.OrderBy(t => t.Key))
            {
                output.WriteStartObject(pair.Key);
                output.WritePropertyString("$ref", "#/definitions/" + pair.Key);
                output.WriteEndObject();
            }

            output.WriteStartObject("Template");
            output.WritePropertyString("$ref", "#/definitions/Template");
            output.WriteEndObject();

            output.WriteEndObject();

            output.WriteStartObject("definitions");

            foreach (var pair1 in pairs.OrderBy(t => t.Key))
            {
                WriteJsonTemplate(output, languages, pairs, pair1);
            }

            WriteJsonTemplate(output);

            output.WriteEndObject();

            output.WriteEndObject();
        }

        protected virtual void WriteJsonTemplate([NotNull] JsonTextWriter output, [ItemNotNull, NotNull] IEnumerable<Language> languages, [NotNull] Dictionary<string, Template> templates, KeyValuePair<string, Template> pair)
        {
            output.WriteStartObject(pair.Key + "Fields");
            output.WritePropertyString("type", "object");
            output.WritePropertyString("additionalProperties", false);
            output.WriteStartObject("properties");

            output.WriteSchemaPropertyObject("Id", "type", "string");
            output.WriteSchemaPropertyObject("Name", "type", "string");
            output.WriteSchemaPropertyObject("Database", "type", "string");
            output.WriteSchemaPropertyObject("TemplateName", "type", "string");
            output.WriteSchemaPropertyObject("Database", "type", "string");
            output.WriteSchemaPropertyObject("ItemPath", "type", "string");

            var allFields = pair.Value.GetAllFields().OrderBy(f => f.FieldName).ToArray();

            if (allFields.Any(f => !f.Shared))
            {
                output.WriteStartObject("Fields");
                output.WritePropertyString("type", "object");
                output.WritePropertyString("additionalProperties", false);
                output.WriteStartObject("properties");

                foreach (var field in allFields.Where(f => f.Shared))
                {
                    WriteJsonField(output, field);
                }

                foreach (var language in languages)
                {
                    output.WriteStartObject(language.LanguageName);
                    output.WritePropertyString("type", "object");
                    output.WritePropertyString("additionalProperties", false);
                    output.WriteStartObject("properties");

                    foreach (var field in allFields.Where(f => !f.Shared && f.Unversioned))
                    {
                        WriteJsonField(output, field);
                    }

                    if (allFields.Any(f => !f.Shared && !f.Unversioned))
                    {
                        output.WriteStartObject(".version.number");
                        output.WritePropertyString("type", "object");
                        output.WritePropertyString("pattern", "\\\\d+");
                        output.WritePropertyString("additionalProperties", false);
                        output.WriteStartObject("properties");

                        foreach (var field in allFields.Where(f => !f.Shared && !f.Unversioned))
                        {
                            WriteJsonField(output, field);
                        }

                        output.WriteEndObject();
                        output.WriteEndObject();
                    }

                    output.WriteEndObject();
                    output.WriteEndObject();
                }

                output.WriteEndObject();
                output.WriteEndObject();
            }

            output.WriteStartObject("Items");
            output.WritePropertyString("type", "object");
            output.WritePropertyString("additionalProperties", false);
            output.WriteStartObject("properties");

            foreach (var t in templates.OrderBy(t => t.Key))
            {
                // avoid field and template clashes
                if (allFields.Any(f => f.FieldName == t.Key))
                {
                    continue;
                }

                output.WriteStartObject(t.Key);
                output.WritePropertyString("$ref", "#/definitions/" + t.Key);
                output.WriteEndObject();
            }

            output.WriteEndObject();
            output.WriteEndObject();

            output.WriteEndObject();
            output.WriteEndObject();

            output.WriteStartObject(pair.Key);
            output.WriteStartArray("oneOf");
            output.WriteStartObject();
            output.WritePropertyString("$ref", "#/definitions/" + pair.Key + "Fields");
            output.WriteEndObject();
            output.WriteStartObject();
            output.WritePropertyString("type", "array");
            output.WriteStartObject("items");
            output.WritePropertyString("$ref", "#/definitions/" + pair.Key + "Fields");
            output.WriteEndObject();
            output.WriteEndObject();
            output.WriteEndArray();
            output.WriteEndObject();
        }

        protected virtual void WriteJsonTemplate([NotNull] JsonTextWriter output)
        {
            output.WriteStartObject("Template");
            output.WritePropertyString("type", "array");
            output.WriteStartObject("items");

            output.WritePropertyString("type", "object");
            output.WritePropertyString("additionalProperties", false);
            output.WriteStartObject("properties");
            output.WriteSchemaPropertyObject("Id", "type", "string");
            output.WriteSchemaPropertyObject("Name", "type", "string");
            output.WriteSchemaPropertyObject("ItemPath", "type", "string");
            output.WriteSchemaPropertyObject("Database", "type", "string");
            output.WriteSchemaPropertyObject("BaseTemplates", "type", "string");
            output.WriteSchemaPropertyObject("Icon", "type", "string");
            output.WriteSchemaPropertyObject("ShortHelp", "type", "string");
            output.WriteSchemaPropertyObject("LongHelp", "type", "string");

            output.WriteStartObject("Section");
            output.WritePropertyString("type", "array");
            output.WriteStartObject("items");
            output.WritePropertyString("type", "object");
            output.WritePropertyString("additionalProperties", false);
            output.WriteStartObject("properties");
            output.WriteSchemaPropertyObject("Id", "type", "string");
            output.WriteSchemaPropertyObject("Name", "type", "string");
            output.WriteSchemaPropertyObject("Icon", "type", "string");

            output.WriteStartObject("Field");
            output.WritePropertyString("type", "array");
            output.WriteStartObject("items");
            output.WritePropertyString("type", "object");
            output.WritePropertyString("additionalProperties", false);
            output.WriteStartObject("properties");
            output.WriteSchemaPropertyObject("Name", "type", "string");
            output.WriteSchemaPropertyObject("Type", "type", "string");
            output.WriteSchemaPropertyObject("Sharing", "type", "string");
            output.WriteSchemaPropertyObject("Source", "type", "string");
            output.WriteSchemaPropertyObject("Sortorder", "type", "integer");
            output.WriteSchemaPropertyObject("ShortHelp", "type", "string");
            output.WriteSchemaPropertyObject("LongHelp", "type", "string");

            output.WriteEndObject(); // properties
            output.WriteArrayString("required", "Name");
            output.WriteEndObject(); // items
            output.WriteEndObject(); // Field
            output.WriteEndObject(); // properties
            output.WriteArrayString("required", "Name", "Field");

            output.WriteEndObject(); // items
            output.WriteEndObject(); // Section
            output.WriteEndObject(); // properties
            output.WriteArrayString("required", "Name");
            output.WriteEndObject(); // items
            output.WriteEndObject(); // Template
        }

        protected virtual void WriteXmlBoolSimpleType([NotNull] XmlWriter output)
        {
            output.WriteStartElement(Xs, "simpleType", Namespace);
            output.WriteAttributeString("name", "bool");

            output.WriteStartElement(Xs, "restriction", Namespace);
            output.WriteAttributeString("base", "xs:string");

            output.WriteStartElement(Xs, "enumeration", Namespace);
            output.WriteAttributeString("value", "False");
            output.WriteEndElement();

            output.WriteStartElement(Xs, "enumeration", Namespace);
            output.WriteAttributeString("value", "True");
            output.WriteEndElement();

            output.WriteEndElement(); // restriction
            output.WriteEndElement(); // simpletype
        }

        protected virtual void WriteXmlFields([NotNull] XmlWriter output)
        {
            output.WriteStartElement(Xs, "group", Namespace);
            output.WriteAttributeString("name", "Fields");
            output.WriteStartElement(Xs, "sequence", Namespace);

            output.WriteStartElement(Xs, "element", Namespace);
            output.WriteAttributeString("name", "Fields.Unversioned");
            output.WriteAttributeString("minOccurs", "0");
            output.WriteAttributeString("maxOccurs", "unbounded");

            output.WriteStartElement(Xs, "complexType", Namespace);

            output.WriteStartElement(Xs, "attribute", Namespace);
            output.WriteAttributeString("name", "Language");
            output.WriteAttributeString("type", "xs:string");
            output.WriteEndElement();

            output.WriteStartElement(Xs, "anyAttribute", Namespace);
            output.WriteAttributeString("processContents", "lax");
            output.WriteEndElement();

            output.WriteEndElement(); // complexType
            output.WriteEndElement(); // element

            output.WriteStartElement(Xs, "element", Namespace);
            output.WriteAttributeString("name", "Fields.Versioned");
            output.WriteAttributeString("minOccurs", "0");
            output.WriteAttributeString("maxOccurs", "unbounded");

            output.WriteStartElement(Xs, "complexType", Namespace);

            output.WriteStartElement(Xs, "attribute", Namespace);
            output.WriteAttributeString("name", "Language");
            output.WriteAttributeString("type", "xs:string");
            output.WriteEndElement();

            output.WriteStartElement(Xs, "attribute", Namespace);
            output.WriteAttributeString("name", "Version");
            output.WriteAttributeString("type", "xs:integer");
            output.WriteEndElement();

            output.WriteStartElement(Xs, "anyAttribute", Namespace);
            output.WriteAttributeString("processContents", "lax");
            output.WriteEndElement();

            output.WriteEndElement(); // complexType
            output.WriteEndElement(); // element

            output.WriteStartElement(Xs, "element", Namespace);
            output.WriteAttributeString("name", "Fields.Layout");
            output.WriteAttributeString("minOccurs", "0");
            output.WriteAttributeString("maxOccurs", "1");

            output.WriteStartElement(Xs, "complexType", Namespace);

            output.WriteStartElement(Xs, "choice", Namespace);

            output.WriteStartElement(Xs, "any", Namespace);
            output.WriteAttributeString("minOccurs", "0");
            output.WriteAttributeString("maxOccurs", "1");
            output.WriteAttributeString("processContents", "lax");
            output.WriteAttributeString("namespace", "http://www.sitecore.net/pathfinder/layouts/master");
            output.WriteEndElement();

            output.WriteStartElement(Xs, "any", Namespace);
            output.WriteAttributeString("minOccurs", "0");
            output.WriteAttributeString("maxOccurs", "1");
            output.WriteAttributeString("processContents", "lax");
            output.WriteAttributeString("namespace", "http://www.sitecore.net/pathfinder/layouts/core");
            output.WriteEndElement();

            output.WriteEndElement(); // choice
            output.WriteEndElement(); // complexType
            output.WriteEndElement(); // element

            output.WriteEndElement(); // sequence

            output.WriteEndElement(); // group
        }

        protected virtual void WriteXmlSchema([NotNull] IProject project)
        {
            WriteXmlSchema(project, "master");
            WriteXmlSchema(project, "core");
        }

        protected virtual void WriteXmlSchema([NotNull] IProject project, [NotNull] string databaseName)
        {
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), databaseName + ".xsd");
            var database = project.GetDatabase(databaseName);

            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (var output = XmlWriter.Create(stream, settings))
                {
                    var templates = project.ProjectItems.OfType<Template>().Where(i => i.Database == database).ToArray();

                    WriteXmlSchema(output, database, "http://www.sitecore.net/pathfinder/content/" + databaseName, templates);
                }
            }
        }

        protected virtual void WriteXmlSchema([NotNull] XmlWriter output, [NotNull] Database database, [NotNull] string nameSpace, [NotNull, ItemNotNull] IEnumerable<Template> templates)
        {
            output.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
            output.WriteStartElement(Xs, "schema", Namespace);

            output.WriteAttributeString("targetNamespace", nameSpace);
            output.WriteAttributeString("xmlns", nameSpace);
            output.WriteAttributeString("xmlns", "vs", "http://www.w3.org/2000/xmlns/", "http://schemas.microsoft.com/Visual-Studio-Intellisense");
            output.WriteAttributeString("vs", "friendlyname", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "Sitecore Pathfinder Template Schema");
            output.WriteAttributeString("vs", "ishtmlschema", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "false");
            output.WriteAttributeString("vs", "iscasesensitive", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "true");
            output.WriteAttributeString("vs", "requireattributequotes", "http://schemas.microsoft.com/Visual-Studio-Intellisense", "true");
            output.WriteAttributeString("elementFormDefault", "qualified");

            WriteXmlBoolSimpleType(output);
            WriteXmlStandardAttributes(output);
            WriteXmlFields(output);

            WriteXmlTemplatesGroup(output, templates);
            WriteXmlTemplates(output, database, templates);

            output.WriteEndElement();
        }

        protected virtual void WriteXmlStandardAttributes([NotNull] XmlWriter output)
        {
            output.WriteStartElement(Xs, "attributeGroup", Namespace);
            output.WriteAttributeString("name", "stdattr");

            WriteAttributeString(output, "Name", "xs:string", "The name of the item.");
            WriteAttributeString(output, "Id", "xs:string", "The ID of the item.");
            WriteAttributeString(output, "__Icon", "xs:string", "The icon that represents this template.");

            output.WriteEndElement();
        }

        protected virtual void WriteXmlTemplateFields([NotNull] XmlWriter output, [NotNull] Database database, [NotNull] Template template)
        {
            var fieldNames = new List<string>();
            fieldNames.Add("Name");
            fieldNames.Add("Id");
            fieldNames.Add("ParentItemPath");
            fieldNames.Add("__Icon");

            foreach (var field in template.GetAllFields().OrderBy(f => f.FieldName))
            {
                var fieldName = field.FieldName.GetSafeCodeIdentifier();
                if (fieldNames.Contains(fieldName))
                {
                    continue;
                }

                var writeHelp = true;

                fieldNames.Add(fieldName);

                output.WriteStartElement(Xs, "attribute", Namespace);
                output.WriteAttributeString("name", fieldName);

                // todo: support for dropdown lists
                switch (field.Type.ToLowerInvariant())
                {
                    case "checkbox":
                        output.WriteAttributeString("type", "bool");
                        break;
                    case "integer":
                        output.WriteAttributeString("type", "xs:int");
                        break;
                    case "valuelookup":
                    case "droplist":
                        output.WriteAttributeString("type", "xs:string");
                        writeHelp = false;
                        break;
                    case "lookup":
                    case "droplink":
                        output.WriteAttributeString("type", "xs:string");
                        writeHelp = false;
                        break;
                    default:
                        output.WriteAttributeString("type", "xs:string");
                        break;
                }

                if (writeHelp)
                {
                    var help = field.ShortHelp;
                    if (!string.IsNullOrEmpty(help))
                    {
                        output.WriteStartElement(Xs, "annotation", Namespace);
                        output.WriteStartElement(Xs, "documentation", Namespace);
                        output.WriteValue(help);
                        output.WriteEndElement();
                        output.WriteEndElement();
                    }
                }

                output.WriteEndElement();
            }
        }

        protected virtual void WriteXmlTemplates([NotNull] XmlWriter output, [NotNull] Database database, [NotNull, ItemNotNull] IEnumerable<Template> templates)
        {
            foreach (var template in templates)
            {
                output.WriteStartElement(Xs, "element", Namespace);
                output.WriteAttributeString("name", GetTemplateName(template));

                output.WriteStartElement(Xs, "complexType", Namespace);

                output.WriteStartElement(Xs, "sequence", Namespace);

                output.WriteStartElement(Xs, "group", Namespace);
                output.WriteAttributeString("ref", "Fields");
                output.WriteAttributeString("minOccurs", "0");
                output.WriteAttributeString("maxOccurs", "1");
                output.WriteEndElement();

                output.WriteStartElement(Xs, "group", Namespace);
                output.WriteAttributeString("ref", "Templates");
                output.WriteAttributeString("minOccurs", "0");
                output.WriteAttributeString("maxOccurs", "unbounded");
                output.WriteEndElement();

                output.WriteEndElement(); // sequence

                WriteXmlTemplateFields(output, database, template);

                output.WriteStartElement(Xs, "attributeGroup", Namespace);
                output.WriteAttributeString("ref", "stdattr");
                output.WriteEndElement();

                output.WriteStartElement(Xs, "anyAttribute", Namespace);
                output.WriteAttributeString("processContents", "lax");
                output.WriteEndElement();

                output.WriteEndElement(); // complexType
                output.WriteEndElement();
            }
        }

        protected virtual void WriteXmlTemplatesGroup([NotNull] XmlWriter output, [NotNull, ItemNotNull] IEnumerable<Template> templates)
        {
            output.WriteStartElement(Xs, "group", Namespace);
            output.WriteAttributeString("name", "Templates");

            output.WriteStartElement(Xs, "choice", Namespace);

            foreach (var template in templates)
            {
                output.WriteStartElement(Xs, "element", Namespace);
                output.WriteAttributeString("ref", GetTemplateName(template));
                output.WriteEndElement();
            }

            output.WriteEndElement(); // choice
            output.WriteEndElement(); // group
        }
    }
}
