// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Building.Deploying
{
    public class WriteExports : TaskBase
    {
        public WriteExports() : base("write-exports")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                return;
            }

            context.Trace.TraceInformation("Writing package exports...");

            var fileName = PathHelper.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.WriteExportsFileName));

            var fieldToWrite = context.Configuration.GetString(Constants.Configuration.WriteExportsFieldsToWrite).Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()).ToList();

            using (var writer = new StreamWriter(fileName))
            {
                using (var output = new XmlTextWriter(writer))
                {
                    output.Formatting = Formatting.Indented;

                    output.WriteStartElement("Exports");

                    foreach (var template in context.Project.Items.OfType<Template>().Where(template => !template.IsExtern))
                    {
                        template.WriteAsExport(output);
                    }

                    foreach (var item in context.Project.Items.OfType<Item>().Where(item => !item.IsExtern))
                    {
                        item.WriteAsExport(output, fieldToWrite);
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
