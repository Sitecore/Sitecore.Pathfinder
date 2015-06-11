// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Web;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying.PublishDatabase
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

            var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
            var publishUrl = context.Configuration.GetString(Constants.Configuration.PublishUrl).TrimStart('/');
            var url = hostName + "/" + publishUrl + HttpUtility.UrlEncode(context.Project.Options.DatabaseName);

            Request(context, url);
        }
    }
}
