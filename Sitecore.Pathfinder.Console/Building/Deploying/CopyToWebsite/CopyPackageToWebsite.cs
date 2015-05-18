namespace Sitecore.Pathfinder.Building.Deploying.CopyToWebsite
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class CopyPackageToWebsite : TaskBase
  {
    public CopyPackageToWebsite() : base("copy-to-website")
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

      context.Trace.TraceInformation(Texts.Text1007);

      var destinationDirectory = context.Configuration.Get(Constants.Configuration.Wwwroot);
      destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.DataFolderName));
      destinationDirectory = PathHelper.Combine(destinationDirectory, Constants.Configuration.Pathfinder);
      destinationDirectory = PathHelper.Combine(destinationDirectory, context.Configuration.Get(Constants.Configuration.PackageDirectory));

      context.FileSystem.CreateDirectory(destinationDirectory);

      foreach (var fileName in context.OutputFiles)
      {
        var destinationFileName = PathHelper.Combine(destinationDirectory, Path.GetFileName(fileName) ?? string.Empty);

        context.FileSystem.Copy(fileName, destinationFileName);
      }
    }
  }
}
