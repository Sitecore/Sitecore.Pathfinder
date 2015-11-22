// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutHtmlFileFieldCompiler : FieldCompilerBase
    {
        public LayoutHtmlFileFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return field.ValueHint.Contains("HtmlTemplate");
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var htmlTemplate = field.Value.Trim();
            if (string.IsNullOrEmpty(htmlTemplate))
            {
                return string.Empty;
            }

            var value = htmlTemplate;

            var rendering = field.Item.Project.ProjectItems.OfType<Rendering>().FirstOrDefault(i => string.Equals(i.FilePath, value, StringComparison.OrdinalIgnoreCase));
            if (rendering == null)
            {
                context.Trace.TraceError(Msg.C1047, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), value);
            }

            var layoutItem = field.Item.Project.ProjectItems.OfType<Item>().FirstOrDefault(i => i.ItemIdOrPath == "/sitecore/layout/Layouts/MvcLayout");
            if (layoutItem == null)
            {
                context.Trace.TraceError(Msg.C1048, Texts.Layout_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), "/sitecore/layout/Layouts/MvcLayout");
            }

            if (rendering == null || layoutItem == null)
            {
                return string.Empty;
            }

            var renderingItemUri = rendering.RenderingItemUri;
            if (renderingItemUri == ProjectItemUri.Empty)
            {
                return string.Empty;
            }

            var renderingId = renderingItemUri.Guid.Format();

            var writer = new StringWriter();
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartElement("r");

            var deviceItems = field.Item.Project.ProjectItems.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase));
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

            return writer.ToString();
        }
    }
}
