// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building
{
    public abstract class RequestTaskBase : TaskBase
    {
        protected RequestTaskBase([NotNull] string taskName) : base(taskName)
        {
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
                    context.Trace.Writeline(output);
                }

                return true;
            }
            catch (WebException ex)
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

                context.Trace.TraceError(Texts.The_server_returned_an_error, message);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.The_server_returned_an_error, ex.Message);
            }

            return false;
        }
    }
}
