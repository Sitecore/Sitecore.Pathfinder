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
      if (context.Project.HasErrors)
      {
        context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
        context.IsAborted = true;
        return;
      }

      context.Trace.TraceInformation(Texts.Publishing___);

      var hostName = context.Configuration.GetString(Constants.Configuration.HostName).TrimEnd('/');
      var publishUrl = context.Configuration.GetString(Constants.Configuration.PublishUrl).TrimStart('/');
      var url = hostName + "/" + publishUrl + HttpUtility.UrlEncode(context.Configuration.Get(Constants.Configuration.Database));

      this.Request(context, url);
    }
  }
}
