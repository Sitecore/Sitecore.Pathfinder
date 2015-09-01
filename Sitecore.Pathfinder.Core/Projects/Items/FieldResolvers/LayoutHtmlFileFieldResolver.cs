// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Layouts;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LayoutHtmlFileFieldResolver : FieldResolverBase
    {
        public LayoutHtmlFileFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(Field field)
        {
            return field.ValueHint.Value.Contains("HtmlTemplate");
        }

        public override string Resolve(Field field)
        {
            var htmlTemplate = field.Value.Value.Trim();
            if (string.IsNullOrEmpty(htmlTemplate))
            {
                return string.Empty;
            }

            var value = htmlTemplate;

            var rendering = field.Item.Project.Items.OfType<Rendering>().FirstOrDefault(i => string.Compare(i.FilePath, value, StringComparison.OrdinalIgnoreCase) == 0);
            if (rendering == null)
            {
                field.WriteDiagnostic(Severity.Error, "Rendering reference not found", value);
            }

            var layoutItem = field.Item.Project.Items.OfType<ExternalReferenceItem>().FirstOrDefault(i => i.ItemIdOrPath == "/sitecore/layout/Layouts/MvcLayout");
            if (layoutItem == null)
            {
                field.WriteDiagnostic(Severity.Error, "Layout reference not found", "/sitecore/layout/Layouts/MvcLayout");
            }

            if (rendering == null || layoutItem == null)
            {
                return string.Empty;
            }

            var renderingId = rendering.Item.Guid.Format();

            var writer = new StringWriter();
            var output = new XmlTextWriter(writer);
            output.Formatting = Formatting.Indented;

            output.WriteStartElement("r");

            foreach (var deviceItem in field.Item.Project.Items.OfType<ExternalReferenceItem>())
            {
                if (!deviceItem.ItemIdOrPath.StartsWith("/sitecore/layout/Devices/"))
                {
                    continue;
                }

                output.WriteStartElement("d");
                output.WriteAttributeString("id", deviceItem.Guid.Format());
                output.WriteAttributeString("l", layoutItem.Guid.Format());

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
