namespace Sitecore.Pathfinder.Shell.Client.Applications.Pathfinder
{
  using System.Web;
  using Sitecore.Pathfinder.Resources;

  public class UpdateResources : IHttpHandler
  {
    public bool IsReusable => true;

    public void ProcessRequest(HttpContext context)
    {
      var resourceManager = new ResourceManager();

      var fileName = resourceManager.BuildResourceFile();

      context.Response.ContentType = "application/zip";
      context.Response.WriteFile(fileName);
    }
  }
}
