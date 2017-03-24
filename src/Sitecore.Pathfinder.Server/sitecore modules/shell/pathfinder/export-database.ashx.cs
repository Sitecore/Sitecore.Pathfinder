// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Shell
{
    public class ExportDatabase : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var databaseName = context.Request.QueryString["db"] ?? "master";

            using (new SecurityDisabler())
            {
                var database = Factory.GetDatabase(databaseName);

                using (var output = new XmlTextWriter(context.Response.Output))
                {
                    output.Formatting = Formatting.Indented;

                    output.WriteStartElement("Exports");

                    WriteItem(output, database, "/sitecore");
                    WriteItem(output, database, "/sitecore/client");
                    WriteItem(output, database, "/sitecore/client/Applications");
                    WriteItem(output, database, "/sitecore/content");
                    WriteItem(output, database, "/sitecore/content/Home");
                    WriteItem(output, database, "/sitecore/layout");
                    WriteItems(output, database, "/sitecore/layout/devices");
                    WriteItem(output, database, "/sitecore/media library");
                    WriteItem(output, database, "/sitecore/system");
                    WriteItems(output, database, "/sitecore/system/field types");
                    WriteItems(output, database, "/sitecore/system/languages");
                    WriteItems(output, database, "/sitecore/system/workflows");
                    WriteItems(output, database, "/sitecore/templates");

                    output.WriteEndElement();
                }
            }
        }

        private void WriteItem(XmlTextWriter output, Database database, string itemPath)
        {
            var item = database.GetItem(itemPath);
            if (item == null)
            {
                return;
            }

            WriteItem(output, item);
        }

        private void WriteItem(XmlTextWriter output, Item item)
        {
            if (item.TemplateID == TemplateIDs.Template)
            {
                output.WriteStartElement("Template");
                output.WriteAttributeString("Id", item.ID.ToString());
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("Name", item.Name);
                output.WriteAttributeString("Path", item.Paths.Path);
                output.WriteAttributeString("BaseTemplates", string.Join("|", item[FieldIDs.BaseTemplate]));

                foreach (Item templateSection in item.Children.OfType<Item>().Where(c => c.TemplateID == TemplateIDs.TemplateSection))
                {
                    output.WriteStartElement("Section");
                    output.WriteAttributeString("Id", templateSection.ID.ToString());
                    output.WriteAttributeString("Name", templateSection.Name);

                    foreach (Item field in templateSection.Children.OfType<Item>().Where(c => c.TemplateID == TemplateIDs.TemplateField))
                    {
                        output.WriteStartElement("Field");
                        output.WriteAttributeString("Id", field.ID.ToString());
                        output.WriteAttributeString("Name", field.Name);
                        output.WriteAttributeString("Type", field["Type"]);
                        output.WriteAttributeString("Sharing", field["Shared"] == "1" ? "Shared" : field["Unversioned"] == "1" ? "Unversioned" : "Versioned");
                        output.WriteEndElement();
                    }

                    output.WriteEndElement();
                }

                output.WriteEndElement();
            }
            else
            {
                output.WriteStartElement("Item");
                output.WriteAttributeString("Id", item.ID.ToString());
                output.WriteAttributeString("Database", item.Database.Name);
                output.WriteAttributeString("Name", item.Name);
                output.WriteAttributeString("Path", item.Paths.Path);
                output.WriteAttributeString("Template", item.Template.InnerItem.Paths.Path);
                output.WriteEndElement();
            }
        }

        private void WriteItems(XmlTextWriter output, Database database, string itemPath)
        {
            var item = database.GetItem(itemPath);
            if (item == null)
            {
                return;
            }

            WriteItems(output, item);
        }

        private void WriteItems(XmlTextWriter output, Item item)
        {
            WriteItem(output, item);

            foreach (Item child in item.GetChildren())
            {
                if (child.TemplateID != TemplateIDs.TemplateSection)
                {
                    WriteItems(output, child);
                }
            }
        }
    }
}
