// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Testing
{
    public class ValidateWebsite : RequestBuildTaskBase
    {
        public ValidateWebsite() : base("validate-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Validating_website___);
            context.Trace.TraceInformation(Texts.Go_get_a_cup_of_coffee_or_read_some_emails___this_may_take_a_while___);

            var queryStringParameters = new Dictionary<string, string>();

            queryStringParameters["t"] = context.Configuration.GetString(Constants.Configuration.ValidateWebsite.Timeout);
            queryStringParameters["c"] = context.Configuration.GetString(Constants.Configuration.ValidateWebsite.Site);
            queryStringParameters["d"] = context.Project.Options.DatabaseName + "^" + context.Configuration.GetString(Constants.Configuration.ValidateWebsite.Languages);
            queryStringParameters["v"] = context.Configuration.GetString(Constants.Configuration.ValidateWebsite.InactiveValidations);
            queryStringParameters["i"] = context.Configuration.GetString(Constants.Configuration.ValidateWebsite.RootItemPath);
            queryStringParameters["s"] = context.Configuration.GetString(Constants.Configuration.ValidateWebsite.ProcessSiteValidation);

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.ValidateWebsite.Url), queryStringParameters);
            var targetFileName = Path.GetTempFileName();

            if (!DownloadFile(context, url, targetFileName))
            {
                return;
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(targetFileName);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.An_error_occured, ex.Message);
                return;
            }

            var root = doc.Root;
            if (root == null)
            {
                context.Trace.TraceError(Texts.An_error_occured);
                return;
            }

            foreach (var element in root.Elements())
            {
                var name = element.GetAttributeValue("name");
                var severity = element.GetAttributeValue("severity");
                var catagory = element.GetAttributeValue("category");
                var item = element.GetAttributeValue("item");
                var itemPath = element.GetAttributeValue("itempath");
                var title = element.GetElementValue("title");
                var problem = element.GetElementValue("problem");
                var solution = element.GetElementValue("solution");

                if (severity == "error")
                {
                    context.Trace.TraceError(problem + @" " + solution, itemPath);
                }
                else
                {
                    context.Trace.TraceWarning(problem + @" " + solution, itemPath);
                }
            }

            context.FileSystem.DeleteFile(targetFileName);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Runs the Sitecore Rocks SitecoreCop on the website.");
            helpWriter.Remarks.Write("This may take a while.");
        }
    }
}
