// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Layouts;

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

            var rendering = field.Item.Project.Items.OfType<Rendering>().FirstOrDefault(i => string.Compare(i.FilePath, value, StringComparison.OrdinalIgnoreCase) == 0);
            if (rendering == null)
            {
                context.Trace.TraceError(Texts.Rendering_reference_not_found, value);
            }

            var layoutItem = field.Item.Project.Items.OfType<Item>().FirstOrDefault(i => i.ItemIdOrPath == "/sitecore/layout/Layouts/MvcLayout");
            if (layoutItem == null)
            {
                context.Trace.TraceError(Texts.Layout_reference_not_found, "/sitecore/layout/Layouts/MvcLayout");
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

            var deviceItems = field.Item.Project.Items.OfType<Item>().Where(i => string.Equals(i.TemplateIdOrPath, "/sitecore/templates/System/Layout/Device", StringComparison.OrdinalIgnoreCase) || string.Equals(i.TemplateIdOrPath, "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}", StringComparison.OrdinalIgnoreCase));
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
