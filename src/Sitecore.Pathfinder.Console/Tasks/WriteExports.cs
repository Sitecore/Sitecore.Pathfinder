// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class WriteExports : BuildTaskBase
    {
        [ImportingConstructor]
        public WriteExports([NotNull] IFileSystemService fileSystem) : base("write-exports")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1015, Texts.Writing_package_exports___);

            var fieldToWrite = context.Configuration.GetStringList(Constants.Configuration.WriteExports.FieldsToWrite).Select(f => f.ToLowerInvariant()).ToList();

            var fileName = PathHelper.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.WriteExports.FileName));
            FileSystem.CreateDirectoryFromFileName(fileName);

            var project = context.LoadProject();

            using (var writer = FileSystem.OpenStreamWriter(fileName))
            {
                using (var output = new XmlTextWriter(writer))
                {
                    output.Formatting = Formatting.Indented;

                    output.WriteStartElement("Exports");

                    foreach (var template in project.Templates)
                    {
                        template.WriteAsExportXml(output);
                    }

                    foreach (var item in project.Items)
                    {
                        item.WriteAsExportXml(output, fieldToWrite);
                    }

                    output.WriteEndElement();
                }
            }
        }
    }
}
