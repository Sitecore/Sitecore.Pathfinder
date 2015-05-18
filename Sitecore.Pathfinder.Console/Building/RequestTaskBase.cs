namespace Sitecore.Pathfinder.Building
{
  using System;
  using System.IO;
  using System.Net;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class RequestTaskBase : TaskBase
  {
    protected RequestTaskBase([NotNull] string taskName) : base(taskName)
    {
    }

    protected bool Request([NotNull] IBuildContext context, [NotNull] string url)
    {
      var webClient = new WebClient();
      try
      {
        var output = webClient.DownloadString(url);

        if (!string.IsNullOrEmpty(output))
        {
          output = output.Trim();
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
          message = new StreamReader(stream).ReadToEnd();
        }

        context.Trace.TraceError(Texts.Text3008, message);
      }
      catch (Exception ex)
      {
        context.Trace.TraceError(Texts.Text3008, ex.Message);
      }

      return false;
    }
  }
}
