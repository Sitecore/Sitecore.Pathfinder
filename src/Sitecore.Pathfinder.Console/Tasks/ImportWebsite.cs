// � 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ImportWebsite : WebBuildTaskBase
    {
        public ImportWebsite() : base("import-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.G1012, Texts.Importing_website___);

            var webRequest = GetWebRequest(context).AsTask("ImportWebsite");

            Post(context, webRequest);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Imports items and files from the website.");
            helpWriter.Remarks.WriteLine("This task is usually use at the beginning of a project to import existing items and files.");
            helpWriter.Remarks.WriteLine("The task is uses the 'project-website-mappings' configuration to determine which items and files are imported.");
        }
    }
}
