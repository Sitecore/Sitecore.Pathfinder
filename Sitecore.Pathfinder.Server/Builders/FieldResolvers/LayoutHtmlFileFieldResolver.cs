// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data.Templates;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Layouts;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
    [Export(typeof(IFieldResolver))]
    public class LayoutHtmlFileFieldResolver : FieldResolverBase
    {
        public LayoutHtmlFileFieldResolver() : base(Constants.FieldResolvers.Normal)
        {
        }

        public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
        {
            return field.ValueHint.Value.Contains("HtmlTemplate");
        }

        public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
        {
            var htmlTemplate = field.Value.Value;
            if (string.IsNullOrEmpty(htmlTemplate))
            {
                return string.Empty;
            }

            var value = htmlTemplate;

            var rendering = context.Project.Items.OfType<Rendering>().FirstOrDefault(i => string.Compare(i.FilePath, value, StringComparison.OrdinalIgnoreCase) == 0);
            if (rendering == null)
            {
                context.Trace.TraceError(Texts.Rendering_not_found, value);
                return string.Empty;
            }

            var renderingId = rendering.Item.Guid.ToString("B").ToUpperInvariant();

            using (new SecurityDisabler())
            {
                var database = Factory.GetDatabase(field.Item.DatabaseName);
                var devices = database.GetItem(ItemIDs.DevicesRoot);
                if (devices == null)
                {
                    return string.Empty;
                }

                var writer = new StringWriter();
                var output = new XmlTextWriter(writer);
                output.Formatting = Formatting.Indented;

                output.WriteStartElement("r");

                foreach (Sitecore.Data.Items.Item device in devices.Children)
                {
                    output.WriteStartElement("d");
                    output.WriteAttributeString("id", device.ID.ToString());
                    output.WriteAttributeString("l", "{5E9D5374-E00A-4053-9127-EBC96A02C721}");

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
}
