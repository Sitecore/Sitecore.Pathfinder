// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Xml;

namespace Sitecore.Pathfinder.Building.Assembling
{
    public class WriteExports : BuildTaskBase
    {
        public WriteExports() : base("write-exports")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                return;
            }

            context.Trace.TraceInformation(Msg.D1015, Texts.Writing_package_exports___);

            var fileName = PathHelper.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.WriteExportsFileName));

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);

            var fieldToWrite = context.Configuration.GetCommaSeparatedStringList(Constants.Configuration.WriteExportsFieldsToWrite).Select(f => f.ToLowerInvariant()).ToList();

            using (var writer = new StreamWriter(fileName))
            {
                using (var output = new XmlTextWriter(writer))
                {
                    output.Formatting = Formatting.Indented;

                    output.WriteStartElement("Exports");

                    foreach (var template in context.Project.Templates)
                    {
                        template.WriteAsExportXml(output);
                    }

                    foreach (var item in context.Project.Items)
                    {
                        item.WriteAsExportXml(output, fieldToWrite);
                    }

                    output.WriteEndElement();
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Writes export declarations");

            helpWriter.Remarks.Write("Writes export declarations so other packages can use this package.");
        }
    }
}
