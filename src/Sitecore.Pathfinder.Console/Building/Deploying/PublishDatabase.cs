// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying
{
    [Export(typeof(ITask))]
    public class PublishDatabase : RequestTaskBase
    {
        public PublishDatabase() : base("publish-database")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Texts.Publishing___);

            if (string.Compare(context.Project.Options.DatabaseName, "core", StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Trace.TraceInformation(Texts.Database_is__core___Skipping_);
                return;
            }

            context.Trace.TraceInformation(Texts.Database, context.Project.Options.DatabaseName);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = "i",
                ["db"] = context.Project.Options.DatabaseName
            };

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.PublishUrl), queryStringParameters);
            Request(context, url);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Publishes a Sitecore database (usually the master database).");
            helpWriter.Parameters.Write("database - name of the database (master, core)");
            helpWriter.Examples.WriteLine($"scc {TaskName}");
            helpWriter.Examples.WriteLine($"scc {TaskName} --database master");
        }
    }
}
