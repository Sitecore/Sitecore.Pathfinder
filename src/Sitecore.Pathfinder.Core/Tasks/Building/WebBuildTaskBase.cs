// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public abstract class WebBuildTaskBase : BuildTaskBase
    {
        protected WebBuildTaskBase([NotNull] string taskName) : base(taskName)
        {
        }

        protected virtual bool DownloadFile([NotNull] ITaskContext context, [NotNull] WebRequest webRequest, [NotNull] string targetFileName)
        {
            var webClient = GetWebClient(context.Configuration);
            try
            {
                var url = webRequest.GetUrl();

                // todo: stream this or we might run out of memory
                var responseBytes = webClient.UploadValues(url, "POST", webRequest.PostData);

                context.FileSystem.WriteAllBytes(targetFileName, responseBytes);

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

        [NotNull]
        protected virtual WebClient GetWebClient([NotNull] IConfiguration configuration)
        {
            return new WebClientWithTimeout(configuration);
        }

        [NotNull]
        protected virtual WebRequest GetWebRequest([NotNull] IConfiguration configuration)
        {
            return new WebRequest(configuration);
        }

        [NotNull]
        protected virtual WebRequest GetWebRequest([NotNull] IBuildContext context)
        {
            return GetWebRequest(context.Configuration).WithCredentials().WithProjectDirectory(context.Project.ProjectDirectory).WithToolsDirectory(context.ToolsDirectory).WithConfiguration();
        }

        protected virtual void HandleWebException([NotNull] ITaskContext context, [NotNull] WebException ex)
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

        protected virtual bool Post([NotNull] ITaskContext context, [NotNull] WebRequest webRequest)
        {
            var webClient = GetWebClient(context.Configuration);
            try
            {
                var url = webRequest.GetUrl();

                var responsebytes = webClient.UploadValues(url, "POST", webRequest.PostData);
                var output = Encoding.UTF8.GetString(responsebytes);

                if (!string.IsNullOrEmpty(output))
                {
                    output = HttpUtility.HtmlDecode(output).Trim();
                }

                if (!string.IsNullOrEmpty(output))
                {
                    // todo: consider parsing diagnostics
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

        protected class WebClientWithTimeout : WebClient
        {
            public WebClientWithTimeout([NotNull] IConfiguration configuration)
            {
                Configuration = configuration;
            }

            [NotNull]
            protected IConfiguration Configuration { get; }

            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                var webRequest = base.GetWebRequest(uri);

                if (webRequest != null)
                {
                    webRequest.Timeout = Configuration.GetInt(Constants.Configuration.System.WebRequests.Timeout, 300) * 1000;
                }

                return webRequest;
            }
        }
    }
}
