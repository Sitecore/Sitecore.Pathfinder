namespace Sitecore.Pathfinder.Building.Deploying.Install
{
  using System;
  using System.ComponentModel.Composition;
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

    public override void Execute(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        throw new BuildException(ConsoleTexts.Text3011);
      }

      context.Trace.TraceInformation(ConsoleTexts.Text1008);

      var packageId = context.Configuration.Get("packaging:nuspec:id");
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
      catch (Exception ex)
      {
        context.Trace.TraceError(ConsoleTexts.Text3008, ex.Message);
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

      // use SourceMap to remap file names
      var solutionDirectory = context.SolutionDirectory;

      foreach (var pair in context.SourceMap)
      {
        var targetFileName = PathHelper.UnmapPath(solutionDirectory, pair.Key).TrimStart('\\');
        var sourceFile = PathHelper.UnmapPath(solutionDirectory, pair.Value).TrimStart('\\');

        output = output.Replace(targetFileName, sourceFile);
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
