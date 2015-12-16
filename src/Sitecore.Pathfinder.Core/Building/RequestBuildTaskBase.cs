// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building
{
    public abstract class RequestBuildTaskBase : BuildTaskBase
    {
        /// <summary>Createa new task.</summary>
        /// <param name="taskName">The name of the task. This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</param>
        protected RequestBuildTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        protected virtual bool DownloadFile([NotNull] IBuildContext context, [NotNull] string url, [NotNull] string targetFileName)
        {
            var webClient = new WebClient();

            try
            {
                webClient.DownloadFile(url, targetFileName);
                return true;
            }
            catch (WebException ex)
            {
                HandleWebException(context, ex);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.M1000, Texts.The_server_returned_an_error, ex.Message);
            }

            return false;
        }

        protected virtual void HandleWebException([NotNull] IBuildContext context, [NotNull] WebException ex)
        {
            var message = ex.Message;

            var stream = ex.Response?.GetResponseStream();
            if (stream != null)
            {
                message = HttpUtility.HtmlDecode(new StreamReader(stream).ReadToEnd()) ?? string.Empty;
            }

            var bodyStart = message.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            if (bodyStart >= 0)
            {
                bodyStart = message.IndexOf(">", bodyStart, StringComparison.OrdinalIgnoreCase);
                var bodyEnd = message.IndexOf("</body", StringComparison.OrdinalIgnoreCase);

                message = message.Mid(bodyStart + 1, bodyEnd - bodyStart - 1).Trim();

                message = Regex.Replace(message, "<[^>]*>", string.Empty);
                message = Regex.Replace(message, @"[\r\n]+", "\r\n");
                message = Regex.Replace(message, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                message = Regex.Replace(message, @"^\s+", "    ", RegexOptions.Multiline);
            }

            context.Trace.TraceError(Msg.M1001, Texts.The_server_returned_an_error, message);
        }

        [NotNull]
        protected virtual string MakeUrl([NotNull] IBuildContext context, [NotNull] string path, [NotNull] Dictionary<string, string> queryStringParameters)
        {
            var hostName = context.Configuration.GetString(Constants.Configuration.HostName);
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ConfigurationException(Texts.Host_not_found);
            }

            var result = hostName.TrimEnd('/') + "/" + path.TrimStart('/');
            var parameters = string.Empty;

            foreach (var pair in queryStringParameters)
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

            if (queryStringParameters.ContainsKey("u"))
            {
                throw new InvalidOperationException("Querystring parameter 'u' has already been specified");
            }

            if (queryStringParameters.ContainsKey("p"))
            {
                throw new InvalidOperationException("Querystring parameter 'p' has already been specified");
            }

            if (!string.IsNullOrEmpty(parameters))
            {
                parameters += "&";
            }

            parameters += "u=";
            parameters += HttpUtility.UrlEncode(context.Configuration.GetString(Constants.Configuration.UserName));
            parameters += "&p=";
            parameters += HttpUtility.UrlEncode(context.Configuration.GetString(Constants.Configuration.Password));

            if (!string.IsNullOrEmpty(parameters))
            {
                result += "?" + parameters;
            }

            return result;
        }

        [NotNull]
        protected virtual string MakeWebApiUrl([NotNull] IBuildContext context, [NotNull] string route)
        {
            return MakeWebApiUrl(context, route, new Dictionary<string, string>());
        }

        [NotNull]
        protected virtual string MakeWebApiUrl([NotNull] IBuildContext context, [NotNull] string route, [NotNull] Dictionary<string, string> queryStringParameters)
        {
            queryStringParameters["td"] = context.ToolsDirectory;
            queryStringParameters["pd"] = context.ProjectDirectory;

            return MakeUrl(context, "sitecore/shell/client/Applications/Pathfinder/WebApi/" + route, queryStringParameters);
        }

        protected virtual bool Request([NotNull] IBuildContext context, [NotNull] string url)
        {
            var webClient = new WebClient();
            try
            {
                var output = webClient.DownloadString(url);

                if (!string.IsNullOrEmpty(output))
                {
                    output = HttpUtility.HtmlDecode(output).Trim();
                }

                if (!string.IsNullOrEmpty(output))
                {
                    context.Trace.WriteLine(output);
                }

                return true;
            }
            catch (WebException ex)
            {
                HandleWebException(context, ex);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Msg.M1002, Texts.The_server_returned_an_error, ex.Message);
            }

            return false;
        }
    }
}
