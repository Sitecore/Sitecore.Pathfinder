// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Rocks.Server.Pipelines.GetFieldValue;
using Sitecore.SecurityModel;
using Sitecore.Text;

namespace Sitecore.Pathfinder.WebApi
{
    public class LayoutBuilder2
    {
        [NotNull]
        public string BuildLayout([NotNull] Item item)
        {
            var layout = GetFieldValuePipeline.Run().WithParameters(item.Fields[FieldIDs.LayoutField]).Value ?? string.Empty;
            return BuildLayout(item, layout);
        }

        [NotNull]
        public string BuildLayout([NotNull] Item item, [NotNull] string layoutDefinition)
        {
            if (string.IsNullOrEmpty(layoutDefinition))
            {
                return BuildEmptyLayout(item.Database);
            }

            var root = layoutDefinition.ToXElement();
            if (root == null)
            {
                return BuildEmptyLayout(item.Database);
            }

            var deviceElement = root.Elements().FirstOrDefault();
            if (deviceElement == null)
            {
                return BuildEmptyLayout(item.Database);
            }

            var renderingElement = deviceElement.Elements().FirstOrDefault();
            if (renderingElement == null)
            {
                return BuildEmptyLayout(item.Database);
            }

            return BuildLayout(item, root);
        }

        [NotNull]
        protected virtual string BuildEmptyLayout([NotNull] Database database)
        {
            var writer = new StringWriter();
            var output = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };

            output.WriteStartElement("Layout");
            output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");

            var deviceItem = database.GetItem(ItemIDs.DevicesRoot);
            if (deviceItem != null)
            {
                foreach (Item item in deviceItem.Children)
                {
                    output.WriteStartElement("Device");
                    output.WriteAttributeString("Name", item.Name);
                    output.WriteEndElement();
                }
            }

            output.WriteEndElement();

            return writer.ToString();
        }


        [NotNull]
        protected virtual string BuildLayout([NotNull] Item item, [NotNull] XElement root)
        {
            using (new SecurityDisabler())
            {
                var renderingItems = item.Database.GetItemsByTemplate(ServerConstants.Renderings.ViewRenderingId, TemplateIDs.XSLRendering, TemplateIDs.Sublayout, ServerConstants.Renderings.WebcontrolRendering, ServerConstants.Renderings.UrlRendering, ServerConstants.Renderings.MethodRendering).GroupBy(i => i.Name).Select(i => i.First()).OrderBy(i => i.Name).ToList();

                var writer = new StringWriter();
                var output = new XmlTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                output.WriteStartElement("Layout");
                output.WriteAttributeString("xmlns", "http://www.sitecore.net/pathfinder/item");

                foreach (var deviceElement in root.Elements())
                {
                    WriteDevice(output, renderingItems, item.Database, deviceElement);
                }

                output.WriteEndElement();

                return writer.ToString();
            }
        }

        protected virtual void WriteDevice([NotNull] XmlTextWriter output, [NotNull][ItemNotNull] List<Item> renderingItems, [NotNull] Database database, [NotNull] XElement deviceElement)
        {
            var deviceItem = database.GetItem(deviceElement.GetAttributeValue("id"));
            if (deviceItem == null)
            {
                return;
            }

            output.WriteStartElement("Device");
            output.WriteAttributeString("Name", deviceItem.Name);

            var layoutItem = database.GetItem(deviceElement.GetAttributeValue("l"));
            if (layoutItem != null)
            {
                output.WriteAttributeString("Layout", layoutItem.Paths.Path);
            }

            var renderings = new List<Rendering>();
            foreach (var renderingElement in deviceElement.Elements())
            {
                var item = database.GetItem(renderingElement.GetAttributeValue("id"));
                if (item != null)
                {
                    renderings.Add(new Rendering(item, renderingElement));
                }
            }

            // resolve parents
            foreach (var rendering in renderings)
            {
                rendering.ParentRendering = renderings.FirstOrDefault(r => r.Placeholders.Contains(rendering.Placeholder, StringComparer.OrdinalIgnoreCase));
            }

            foreach (var rendering in renderings.Where(r => r.ParentRendering == null))
            {
                rendering.Write(output, renderingItems, database, renderings);
            }

            output.WriteEndElement();
        }

        private class Rendering
        {
            public Rendering([NotNull] Item item, [NotNull] XElement renderingElement)
            {
                Item = item;
                Parameters = new UrlString(renderingElement.GetAttributeValue("par"));
                RenderingElement = renderingElement;
                Id = Parameters.Parameters["Id"];
                Placeholder = renderingElement.GetAttributeValue("ph");

                var placeHolders = item["Place Holders"];
                foreach (var s in placeHolders.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
                {
                    var placeholderName = s.Replace("$Id", Id).Trim();
                    Placeholders.Add(placeholderName);
                }

                var parametersTemplateItem = item.Database.GetItem(item["Parameters Template"]);
                if (parametersTemplateItem == null)
                {
                    return;
                }

                var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, item.Database);
                if (template == null)
                {
                    return;
                }

                foreach (var field in template.GetFields(true))
                {
                    if (field.Template.BaseIDs.Length != 0)
                    {
                        Fields[field.Name.ToLowerInvariant()] = field;
                    }
                }
            }

            [Diagnostics.NotNull]
            private UrlString Parameters { get; }

            [NotNull]
            private Dictionary<string, TemplateField> Fields { get; } = new Dictionary<string, TemplateField>();

            [NotNull]
            private string Id { get; }

            [Diagnostics.NotNull]
            private Item Item { get; }

            [CanBeNull]
            public Rendering ParentRendering { get; set; }

            [NotNull]
            public string Placeholder { get; }

            [NotNull]
            [ItemNotNull]
            public List<string> Placeholders { get; } = new List<string>();

            [NotNull]
            private XElement RenderingElement { get; }

            [Diagnostics.NotNull]
            private static readonly Regex SafeName = new Regex("^[a-zA-Z0-9_\\-\\.]+$", RegexOptions.Compiled);

            public void Write([NotNull] XmlTextWriter output, [NotNull][ItemNotNull] List<Item> renderingItems, [NotNull] Database database, [NotNull][ItemNotNull] List<Rendering> renderings)
            {
                var name = Item.Name;

                var renderingDefinitions = renderingItems.Where(i => i.Name == Item.Name).ToList();
                if (renderingDefinitions.Count > 1)
                {
                    name = GetUniqueRenderingName(renderingDefinitions, this);
                }

                if (SafeName.IsMatch(name))
                {
                    output.WriteStartElement(name);
                }
                else
                {
                    output.WriteStartElement("Rendering");
                    output.WriteAttributeString("RenderingName", name);
                }

                // write placeholder, if it is not the default placeholder
                if (!string.IsNullOrEmpty(Placeholder))
                {
                    if (ParentRendering == null || !string.Equals(ParentRendering.Placeholders.FirstOrDefault(), Placeholder, StringComparison.OrdinalIgnoreCase))
                    {
                        output.WriteAttributeString("Placeholder", Placeholder);
                    }
                }

                // write parameters
                foreach (var key in Parameters.Parameters.Keys.OfType<string>().Where(k => !string.IsNullOrEmpty(k)).OrderBy(k => k))
                {
                    var value = Parameters.Parameters[key];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    value = HttpUtility.UrlDecode(value) ?? string.Empty;

                    TemplateField field;
                    if (Fields.TryGetValue(key.ToLowerInvariant(), out field))
                    {
                        switch (field.Type.ToLowerInvariant())
                        {
                            case "checkbox":
                                if (!value.StartsWith("{Binding"))
                                {
                                    value = MainUtil.GetBool(value, false) ? "True" : "False";
                                }

                                break;

                            case "droptree":
                                if (ID.IsID(value))
                                {
                                    var i = database.GetItem(value);
                                    if (i != null)
                                    {
                                        value = i.Paths.Path;
                                    }
                                }

                                break;
                        }

                        var source = new UrlString(field.Source);
                        var defaultValue = source.Parameters["defaultvalue"] ?? string.Empty;

                        if (string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    // todo: Hacky, hacky, hacky
                    if ((key == "IsEnabled" || key == "IsVisible") && value == "True")
                    {
                        continue;
                    }

                    output.WriteAttributeString(key, value);
                }

                // write data source
                var dataSource = RenderingElement.GetAttributeValue("ds");
                if (ID.IsID(dataSource))
                {
                    var dataSourceItem = database.GetItem(dataSource);
                    if (dataSourceItem != null)
                    {
                        dataSource = dataSourceItem.Paths.Path;
                    }
                }

                output.WriteAttributeStringIf("DataSource", dataSource);

                output.WriteAttributeStringIf("Cacheable", RenderingElement.GetAttributeValue("cac") == @"1");
                output.WriteAttributeStringIf("VaryByData", RenderingElement.GetAttributeValue("vbd") == @"1");
                output.WriteAttributeStringIf("VaryByDevice", RenderingElement.GetAttributeValue("vbdev") == @"1");
                output.WriteAttributeStringIf("VaryByLogin", RenderingElement.GetAttributeValue("vbl") == @"1");
                output.WriteAttributeStringIf("VaryByParameters", RenderingElement.GetAttributeValue("vbp") == @"1");
                output.WriteAttributeStringIf("VaryByQueryString", RenderingElement.GetAttributeValue("vbqs") == @"1");
                output.WriteAttributeStringIf("VaryByUser", RenderingElement.GetAttributeValue("vbu") == @"1");

                // write child renderings
                foreach (var rendering in renderings)
                {
                    if (rendering.ParentRendering == this)
                    {
                        rendering.Write(output, renderingItems, database, renderings);
                    }
                }

                output.WriteEndElement();
            }

            [NotNull]
            private string GetUniqueRenderingName([NotNull][ItemNotNull] List<Item> renderings, [NotNull] Rendering rendering)
            {
                var paths = renderings.Where(r => r.Name == rendering.Item.Name && r.ID != rendering.Item.ID).Select(r => r.Paths.Path).ToList();
                var parts = rendering.Item.Paths.Path.Split(Constants.Slash, StringSplitOptions.RemoveEmptyEntries);

                var result = string.Empty;

                for (var i = parts.Length - 1; i >= 0; i--)
                {
                    result = "/" + parts[i] + result;

                    var r = result;
                    if (!paths.Any(p => p.EndsWith(r, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        break;
                    }
                }

                result = result.Mid(1).Replace("/", ".");

                return result;
            }
        }
    }
    /*
    <r>
        <d id="{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}" l="{99C9A84D-AA93-4B2C-ADE1-D349B804590D}">
            <r id="{DAFAFFB8-74AF-4141-A96A-70B16834CEC6}" ph="Page.Code" uid="{947991F1-FDB0-4BBE-9A64-44817380652F}" />
            <r id="{E6FC07D8-2303-4D10-8380-573CCBC82B89}" par="Id=Container" ph="Page.Body" uid="{BD7F3EE7-DB8F-4B34-B9E8-359BF01231C7}" />
            <r id="{09459429-2686-4C46-8E0E-E7055C6FF833}" par="Id=Row&amp;Large=-%2c-&amp;ExtraSmall=-%2c-&amp;Medium=4%2c8&amp;Small=-%2c-" ph="Container.Content" uid="{902220F5-CA70-47DB-A98D-398DFF650941}" />
            <r id="{262EDE8D-941C-4A6D-A365-D2E7A377C3A6}" par="Id=StaticJsonDataSource1&amp;JsonFieldName=Data&amp;JsonDataSource=%7b6FD56988-5AC6-47B1-B78F-BFBDE9829733%7d" ph="Row.Column1" uid="{CB4AC476-6A23-4D93-90C2-DEA4325E62C6}" />
            <r id="{6E77A326-43B3-453C-AFC7-D7E542A14B1D}" par="Id=Bootstrap3Table1&amp;Data=%7bBinding+StaticJsonDataSource1.Data%7d" ph="Row.Column1" uid="{DCC9FDED-6F60-4D21-A3DB-AD07B103A22C}" />
            <r id="{999A4B35-A6E4-42ED-95CB-88BD8A3DEE08}" par="Id=Bootstrap3Heading1&amp;Text=Striped" ph="Row.Column1" uid="{C0A1EC5E-B7CD-4D77-A6BB-BF38C799448E}" />
            <r id="{6E77A326-43B3-453C-AFC7-D7E542A14B1D}" par="Id=Bootstrap3Table2&amp;Data=%7bBinding+StaticJsonDataSource1.Data%7d&amp;IsStriped=1" ph="Row.Column1" uid="{DCC9FDED-6F60-4D21-A3DB-AD07B103A22C}" />
            <r id="{999A4B35-A6E4-42ED-95CB-88BD8A3DEE08}" par="Id=Bootstrap3Heading1&amp;Text=Bordered" ph="Row.Column1" uid="{C0A1EC5E-B7CD-4D77-A6BB-BF38C799448E}" />
            <r id="{6E77A326-43B3-453C-AFC7-D7E542A14B1D}" par="Id=Bootstrap3Table3&amp;IsBordered=1&amp;Data=%7bBinding+StaticJsonDataSource1.Data%7d" ph="Row.Column1" uid="{DCC9FDED-6F60-4D21-A3DB-AD07B103A22C}" />
            <r id="{999A4B35-A6E4-42ED-95CB-88BD8A3DEE08}" par="Id=Bootstrap3Heading1&amp;Text=Hover" ph="Row.Column1" uid="{C0A1EC5E-B7CD-4D77-A6BB-BF38C799448E}" />
            <r id="{6E77A326-43B3-453C-AFC7-D7E542A14B1D}" par="Id=Bootstrap3Table4&amp;Hover=1&amp;Data=%7bBinding+StaticJsonDataSource1.Data%7d" ph="Row.Column1" uid="{DCC9FDED-6F60-4D21-A3DB-AD07B103A22C}" />
            <r id="{999A4B35-A6E4-42ED-95CB-88BD8A3DEE08}" par="Id=Bootstrap3Heading1&amp;Text=Condensed" ph="Row.Column1" uid="{C0A1EC5E-B7CD-4D77-A6BB-BF38C799448E}" />
            <r id="{6E77A326-43B3-453C-AFC7-D7E542A14B1D}" par="Id=Bootstrap3Table5&amp;IsCondensed=1&amp;Data=%7bBinding+StaticJsonDataSource1.Data%7d" ph="Row.Column1" uid="{DCC9FDED-6F60-4D21-A3DB-AD07B103A22C}" />
            <r id="{57DDF236-BAA2-4422-A4C1-75D27AED1184}" par="Id=Bootstrap3Button1&amp;Text=Button" ph="Row.Column1" uid="{2E789D2A-AE94-410E-8933-B96E3F668E41}" />
        </d>
    </r>
    */
}
