// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.HtmlControls.Data;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.WebApi.SynchronizeWebsites
{
    public class XsdContentSchemaSynchronizer : ISynchronizer
    {
        public const string Namespace = "http://www.w3.org/2001/XMLSchema";

        public const string Xs = "xs";

        [Diagnostics.NotNull]
        protected static readonly ID InsertOptionsFieldId = new ID(Constants.Fields.InsertOptionsFieldId);

        public bool CanSynchronize(IConfiguration configuration, string fileName)
        {
            return fileName.EndsWith(".content.xsd", StringComparison.OrdinalIgnoreCase);
        }

        public void Synchronize(IConfiguration configuration, ZipWriter zip, string fileName, string configKey)
        {
            var databaseName = configuration.GetString(configKey + "database");
            var schemaNamespace = configuration.GetString(configKey + "namespace");

            Synchronize(zip, fileName, databaseName, schemaNamespace);
        }

        protected virtual void Synchronize([Diagnostics.NotNull] ZipWriter zip, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string databaseName, [Diagnostics.NotNull] string schemaNamespace)
        {
            var schema = Synchronize(databaseName, schemaNamespace);
            zip.AddEntry(fileName, Encoding.UTF8.GetBytes(schema));
        }

        [NotNull]
        protected virtual string Synchronize([NotNull] string databaseName, [Diagnostics.NotNull] string schemaNamespace)
        {
            var database = Factory.GetDatabase(databaseName);
            if (database == null)
            {
                throw new Exception("Database not found");
            }

            using (new SecurityDisabler())
            {
                var templates = TemplateManager.GetTemplates(database).Values.Where(t => t.Name != "__Standard Values").GroupBy(r => r.Name).Select(group => @group.First()).OrderBy(r => r.Name).ToList();

                var writer = new StringWriter();
                var output = new XmlTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                WriteSchema(output, database, schemaNamespace, templates);

                return writer.ToString();
            }
        }

        [Diagnostics.NotNull]
        protected virtual string GetTemplateName([Diagnostics.NotNull] Template template)
        {
            return template.Name.EscapeXmlElementName();
        }

        protected virtual void WriteAttributeString([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] string name, [Diagnostics.NotNull] string type, [Diagnostics.NotNull] string help)
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

        protected virtual void WriteBoolSimpleType([Diagnostics.NotNull] XmlTextWriter output)
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

        protected virtual void WriteDropListProperty([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] TemplateField field, bool useId)
        {
            database = LookupSources.GetDatabase(field.Source) ?? database;
            var items = LookupSources.GetItems(database.GetRootItem(), field.Source);

            var help = field.GetToolTip(LanguageManager.DefaultLanguage);
            if (!string.IsNullOrEmpty(help))
            {
                output.WriteStartElement(Xs, "annotation", Namespace);
                output.WriteStartElement(Xs, "documentation", Namespace);
                output.WriteValue(help);
                output.WriteEndElement();
                output.WriteEndElement();
            }

            output.WriteStartElement(Xs, "simpleType", Namespace);

            output.WriteStartElement(Xs, "restriction", Namespace);
            output.WriteAttributeString("base", "xs:string");

            foreach (var child in items)
            {
                output.WriteStartElement(Xs, "enumeration", Namespace);
                output.WriteAttributeString("value", useId ? child.ID.ToString() : child.Name);
                output.WriteEndElement();
            }

            output.WriteEndElement(); // restriction

            output.WriteEndElement(); // simpleType
        }

        protected virtual void WriteItemPath([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
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

            output.WriteStartElement(Xs, "attribute", Namespace);
            output.WriteAttributeString("name", "ParentItemPath");

            output.WriteStartElement(Xs, "annotation", Namespace);
            output.WriteStartElement(Xs, "documentation", Namespace);
            output.WriteValue("The path of the parent item.");
            output.WriteEndElement();
            output.WriteEndElement();

            output.WriteStartElement(Xs, "simpleType", Namespace);

            output.WriteStartElement(Xs, "restriction", Namespace);
            output.WriteAttributeString("base", "xs:string");

            foreach (var child in links)
            {
                output.WriteStartElement(Xs, "enumeration", Namespace);
                output.WriteAttributeString("value", child);
                output.WriteEndElement();
            }

            output.WriteEndElement(); // restriction

            output.WriteEndElement(); // simpleType

            output.WriteEndElement();
        }

        protected virtual void WriteSchema([NotNull] XmlTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] string nameSpace, [Diagnostics.NotNull][ItemNotNull] IEnumerable<Template> templates)
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

            WriteBoolSimpleType(output);
            WriteStandardAttributes(output);
            WriteFields(output);

            WriteTemplatesGroup(output, templates);
            WriteTemplates(output, database, templates);

            output.WriteEndElement();
        }

        protected virtual void WriteStandardAttributes([Diagnostics.NotNull] XmlTextWriter output)
        {
            output.WriteStartElement(Xs, "attributeGroup", Namespace);
            output.WriteAttributeString("name", "stdattr");

            WriteAttributeString(output, "Name", "xs:string", "The name of the item.");
            WriteAttributeString(output, "Id", "xs:string", "The ID of the item.");
            WriteAttributeString(output, "__Icon", "xs:string", "The icon that represents this template.");

            output.WriteEndElement();
        }

        protected virtual void WriteTemplateFields([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull] Template template)
        {
            var fieldNames = new List<string>();
            fieldNames.Add("Name");
            fieldNames.Add("Id");
            fieldNames.Add("ParentItemPath");
            fieldNames.Add("__Icon");

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

                var writeHelp = true;

                fieldNames.Add(fieldName);

                output.WriteStartElement(Xs, "attribute", Namespace);
                output.WriteAttributeString("name", fieldName);

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
                        WriteDropListProperty(output, database, field, false);
                        writeHelp = false;
                        break;
                    case "lookup":
                    case "droplink":
                        WriteDropListProperty(output, database, field, true);
                        writeHelp = false;
                        break;
                    default:
                        output.WriteAttributeString("type", "xs:string");
                        break;
                }

                if (writeHelp)
                {
                    var help = field.GetToolTip(LanguageManager.DefaultLanguage);
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

        protected virtual void WriteTemplates([Diagnostics.NotNull] XmlTextWriter output, [Diagnostics.NotNull] Database database, [Diagnostics.NotNull][ItemNotNull] IEnumerable<Template> templates)
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

                WriteTemplateFields(output, database, template);
                WriteItemPath(output, database, template);

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

        protected virtual void WriteFields([Diagnostics.NotNull] XmlTextWriter output)
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

        protected virtual void WriteTemplatesGroup([NotNull] XmlTextWriter output, [NotNull][ItemNotNull] IEnumerable<Template> templates)
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
