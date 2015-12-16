// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Building.Commands
{
    public class ImportWebsite : RequestBuildTaskBase
    {
        public ImportWebsite() : base("import-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1012, Texts.Importing_website___);

            var url = MakeWebApiUrl(context, "ImportWebsite");

            Request(context, url);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Imports items and files from the website.");
            helpWriter.Remarks.WriteLine("This task is usually use at the beginning of a project to import existing items and files.");
            helpWriter.Remarks.WriteLine("The task is uses the 'project-website-mappings' configuration to determine which items and files are imported.");
        }
    }
}
