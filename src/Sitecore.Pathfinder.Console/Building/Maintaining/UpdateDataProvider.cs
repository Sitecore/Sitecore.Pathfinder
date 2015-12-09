namespace Sitecore.Pathfinder.Building.Maintaining
{
    public class UpdateDataProvider : RequestBuildTaskBase
    {
        public UpdateDataProvider() : base("update-mappings")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1009, "Updating the project/website mappings on the website...");

            var url = MakeWebApiUrl(context, "UpdateProjectWebsiteMappings");

            Request(context, url);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Updates the project/website mapping on the website.");
            helpWriter.Remarks.WriteLine("The 'project-website-mappings' settings enables the serializing data provider on the website to serialize changed items back to the projects.");
            helpWriter.Remarks.WriteLine("This task should be called when the 'project-website-mappings' settings have been changed (or you can just kill the 'w3wp.exe' process or restart IIS).");
        }
    }
}