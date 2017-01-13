// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class PublishDatabase : WebBuildTaskBase
    {
        public PublishDatabase() : base("publish-database")
        {
        }

        [NotNull, Option("database", Alias = "d", HelpText = "Name of database to publish", PositionalArg = 1)]
        public string DatabaseName { get; set; } = string.Empty;

        [NotNull, Option("mode", Alias = "m", DefaultValue = "i", HelpText = "Publishing mode (r, i, s or b)")]
        public string Mode { get; set; } = string.Empty;

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1016, Texts.Publishing___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            var databaseName = DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                var project = context.LoadProject();
                databaseName = project.Options.DatabaseName;
            }

            if (string.Equals(databaseName, "core", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.TraceInformation(Msg.D1013, Texts.Database_is__core___Skipping_);
                return;
            }

            context.Trace.TraceInformation(Msg.D1014, Texts.Database, databaseName);

            var queryStringParameters = new Dictionary<string, string>
            {
                ["m"] = Mode,
                ["db"] = databaseName
            };

            var webRequest = GetWebRequest(context).WithQueryString(queryStringParameters).WithUrl(context.Configuration.GetString(Constants.Configuration.PublishDatabases.PublishUrl));
            Post(context, webRequest);
        }
    }
}
