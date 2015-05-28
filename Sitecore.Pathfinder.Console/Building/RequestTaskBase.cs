namespace Sitecore.Pathfinder.Building
{
  using System;
  using System.IO;
  using System.Net;
  using System.Web;
  using Sitecore.Pathfinder.Diagnostics;

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
