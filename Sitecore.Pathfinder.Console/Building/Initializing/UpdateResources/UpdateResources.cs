namespace Sitecore.Pathfinder.Building.Initializing.UpdateResources
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.IO.Compression;
  using System.Net;
  using System.Web;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;

  [Export(typeof(ITask))]
  public class UpdateResources : TaskBase
  {
    public UpdateResources() : base("update-resources")
    {
    }

    public override void Run(IBuildContext context)
    {
      context.Trace.TraceInformation(Texts.Downloading_resources___);

      var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
      var installUrl = context.Configuration.GetString(Constants.Configuration.UpdateResourcesUrl).TrimStart('/');
      var url = hostName + "/" + installUrl;

      var targetFileName = Path.GetTempFileName();

      var webClient = new WebClient();
      try
      {
        webClient.DownloadFile(url, targetFileName);

        context.Trace.TraceInformation(Texts.Updating_resources___);
        using (var zip = ZipFile.OpenRead(targetFileName))
        {
          foreach (var entry in zip.Entries)
          {
            context.Trace.TraceInformation(entry.FullName);
            entry.ExtractToFile(Path.Combine(context.SolutionDirectory, entry.FullName), true);
          }
        }
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
    }
  }
}
