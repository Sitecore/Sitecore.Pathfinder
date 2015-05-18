namespace Sitecore.Pathfinder.Building.Deploying.Install
{
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Web;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;

  [Export(typeof(ITask))]
  public class Install : RequestTaskBase
  {
    public Install() : base("install")
    {
    }

    public override void Run(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        context.Trace.TraceInformation(Texts.Text3011);
        context.IsAborted = true;
        return;
      }

      context.Trace.TraceInformation(Texts.Text1008);

      var packageId = Path.GetFileNameWithoutExtension(context.Configuration.Get("nuget:filename"));
      if (string.IsNullOrEmpty(packageId))
      {
        return;
      }

      var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
      var installUrl = context.Configuration.GetString(Constants.Configuration.InstallUrl).TrimStart('/');
      var url = hostName + "/" + installUrl + HttpUtility.UrlEncode(packageId);

      if (!this.Request(context, url))
      {
        return;
      }

      foreach (var projectItem in context.Project.Items)
      {
        projectItem.Document.SourceFile.IsModified = false;
      }
    }
  }
}
