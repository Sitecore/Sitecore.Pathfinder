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
      if (context.Project.HasErrors)
      {
        context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
        context.IsAborted = true;
        return;
      }

      context.Trace.TraceInformation(Texts.Copying_package_to_website___);

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
