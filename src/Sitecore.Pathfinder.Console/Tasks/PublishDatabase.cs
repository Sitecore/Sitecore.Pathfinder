// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class PublishDatabase : WebBuildTaskBase
    {
        public PublishDatabase() : base("publish-database")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1016, Texts.Publishing___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            if (string.Equals(context.Project.Options.DatabaseName, "core", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.TraceInformation(Msg.D1013, Texts.Database_is__core___Skipping_);
                return;
            }

            context.Trace.TraceInformation(Msg.D1014, Texts.Database, context.Project.Options.DatabaseName);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = "i",
                ["db"] = context.Project.Options.DatabaseName
            };

            var webRequest = GetWebRequest(context).WithQueryString(queryStringParameters).WithUrl(context.Configuration.GetString(Constants.Configuration.PublishDatabases.PublishUrl));
            Post(context, webRequest);
        }
    }
}
