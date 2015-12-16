// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.HtmlFile.HtmlFiles
{
    public class HtmlFilePropertyCompiler : CompilerBase
    {
        public const string LayoutHtmlfile = "Layout.HtmlFile";

        // must come after RenderingCompiler, or renderings will not be found
        public HtmlFilePropertyCompiler() : base(9000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            return item != null && item.ContainsProperty(LayoutHtmlfile);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var value = item.GetValue<string>(LayoutHtmlfile)?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var property = item.GetProperty<string>(LayoutHtmlfile);

            var rendering = item.Project.ProjectItems.OfType<Rendering>().FirstOrDefault(i => string.Equals(i.FilePath, value, StringComparison.OrdinalIgnoreCase));
            if (rendering == null)
            {
                context.Trace.TraceError(Msg.C1047, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), value);
            }

            var layoutItem = item.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => i.ItemIdOrPath == "/sitecore/layout/Layouts/MvcLayout");
            if (layoutItem == null)
            {
                context.Trace.TraceError(Msg.C1048, Texts.Layout_reference_not_found, TraceHelper.GetTextNode(property), "/sitecore/layout/Layouts/MvcLayout");
            }

            if (rendering == null || layoutItem == null)
            {
                return;
            }

            var renderingItemUri = rendering.RenderingItemUri;
            if (renderingItemUri == ProjectItemUri.Empty)
            {
                RetryCompilation(projectItem);
                return;
            }

            var renderingId = renderingItemUri.Guid.Format();

            var writer = new StringWriter();
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartElement("r");

            var deviceItems = item.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase));
            foreach (var deviceItem in deviceItems)
            {
                if (!deviceItem.ItemIdOrPath.StartsWith("/sitecore/layout/Devices/"))
                {
                    continue;
                }

                output.WriteStartElement("d");
                output.WriteAttributeString("id", deviceItem.Uri.Guid.Format());
                output.WriteAttributeString("l", layoutItem.Uri.Guid.Format());

                output.WriteStartElement("r");
                output.WriteAttributeString("id", renderingId);
                output.WriteAttributeString("ph", "Page.Body");
                output.WriteEndElement();

                output.WriteEndElement();
            }

            output.WriteEndElement();

            var field = item.Fields["__Renderings"];
            if (field == null)
            {
                field = context.Factory.Field(item, TextNode.Empty, "__Renderings", writer.ToString());
                item.Fields.Add(field);
            }
            else
            {
                field.Value = writer.ToString();
            }
        }
    }
}
