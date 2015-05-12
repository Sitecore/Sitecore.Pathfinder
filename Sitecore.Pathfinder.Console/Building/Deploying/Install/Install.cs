namespace Sitecore.Pathfinder.Building.Deploying.Install
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Net;
  using System.Web;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class Install : TaskBase
  {
    public Install() : base("install")
    {
    }

    public override void Run(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        throw new BuildException(Texts.Text3011);
      }

      context.Trace.TraceInformation(Texts.Text1008);

      var packageId = Path.GetFileNameWithoutExtension(context.Configuration.Get("nuget:filename"));
      if (string.IsNullOrEmpty(packageId))
      {
        return;
      }

      var hostName = context.Configuration.Get(Constants.HostName).TrimEnd('/');
      var installUrl = context.Configuration.Get(Constants.InstallUrl).TrimStart('/');
      var url = hostName + "/" + installUrl + HttpUtility.UrlEncode(packageId);

      try
      {
        this.InstallPackage(context, url);
        this.MarkProjectItemsAsNotModified(context);
      }
      catch (WebException ex)
      {
        var message = ex.Message;

        var stream = ex.Response.GetResponseStream();
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
    }

    public void InstallPackage([NotNull] IBuildContext context, [NotNull] string url)
    {
      var webClient = new WebClient();
      var output = webClient.DownloadString(url).Trim();
      if (string.IsNullOrEmpty(output))
      {
        return;
      }

      context.Trace.Writeline(output);
    }

    protected void MarkProjectItemsAsNotModified([NotNull] IBuildContext context)
    {
      foreach (var projectItem in context.Project.Items)
      {
        projectItem.SourceFile.IsModified = false;
      }
    }
  }
}
