namespace Sitecore.Pathfinder.Building.Deploying.Publishing
{
  using System.ComponentModel.Composition;
  using System.Web;
  using Sitecore.Pathfinder.Extensions.ConfigurationExtensions;

  [Export(typeof(ITask))]
  public class Publish : RequestTaskBase
  {
    public Publish() : base("publish")
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

      context.Trace.TraceInformation(Texts.Text1009);

      var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
      var publishUrl = context.Configuration.GetString(Constants.Configuration.PublishUrl).TrimStart('/');
      var url = hostName + "/" + publishUrl + HttpUtility.UrlEncode(context.Configuration.Get(Constants.Configuration.Database));

      this.Request(context, url);
    }
  }
}
