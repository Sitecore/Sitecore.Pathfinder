// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public class WebRequest
    {
        public WebRequest([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        public IConfiguration Configuration { get; }

        [NotNull, ItemNotNull]
        public NameValueCollection PostData { get; } = new NameValueCollection();

        [NotNull]
        public IDictionary<string, string> QueryStringParameters { get; } = new Dictionary<string, string>();

        [NotNull]
        public string Url { get; private set; } = string.Empty;

        [NotNull]
        public WebRequest AsTask([NotNull] string taskName)
        {
            Url = "sitecore/shell/client/Applications/Pathfinder/Task/" + taskName;
            return this;
        }

        [NotNull]
        public virtual string GetUrl()
        {
            var hostName = Configuration.GetString(Constants.Configuration.HostName);
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ConfigurationException(Texts.Host_not_found);
            }

            var result = hostName.TrimEnd('/') + "/" + Url.TrimStart('/');
            var parameters = string.Empty;

            foreach (var pair in QueryStringParameters)
            {
                if (string.IsNullOrEmpty(pair.Value))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(parameters))
                {
                    parameters += "&";
                }

                parameters += HttpUtility.UrlEncode(pair.Key);
                parameters += "=";
                parameters += HttpUtility.UrlEncode(pair.Value);
            }

            if (!string.IsNullOrEmpty(parameters))
            {
                result += "?" + parameters;
            }

            return result;
        }

        [NotNull]
        public virtual WebRequest WithConfiguration()
        {
            PostData["configuration"] = Configuration.ToJson();
            return this;
        }

        [NotNull]
        public virtual WebRequest WithProjectDirectory([NotNull] string projectDirectory)
        {
            QueryStringParameters["pd"] = projectDirectory;
            return this;
        }

        [NotNull]
        public WebRequest WithQueryString([NotNull] Dictionary<string, string> queryStringParameters)
        {
            QueryStringParameters.AddRange(queryStringParameters);
            return this;
        }

        [NotNull]
        public virtual WebRequest WithToolsDirectory([NotNull] string toolsDirectory)
        {
            QueryStringParameters["td"] = toolsDirectory;
            return this;
        }

        [NotNull]
        public WebRequest WithUrl([NotNull] string url)
        {
            Url = url;
            return this;
        }

        [NotNull]
        public virtual WebRequest WithCredentials()
        {
            QueryStringParameters["u"] = Configuration.GetString(Constants.Configuration.UserName);
            QueryStringParameters["p"] = Configuration.GetString(Constants.Configuration.Password);
            return this;
        }
    }
}
