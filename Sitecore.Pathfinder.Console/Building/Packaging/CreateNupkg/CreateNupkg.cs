namespace Sitecore.Pathfinder.Building.Packaging.CreateNupkg
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using NuGet;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class CreateNupkg : TaskBase
  {
    public CreateNupkg() : base("nuget-pack")
    {
    }

    public override void Run(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        context.Trace.TraceInformation("Package contains errors and will not be deployed");
        context.IsAborted = true;
        return;
      }

      context.Trace.TraceInformation("Creating Nupkg file...");

      var packageFileName = context.Configuration.Get("nuget:filename");
      var nuspecFileName = PathHelper.Combine(context.SolutionDirectory, packageFileName);
      if (string.IsNullOrEmpty(nuspecFileName))
      {
        return;
      }

      var nupkgFileName = Path.ChangeExtension(nuspecFileName, ".nupkg");

      if (context.FileSystem.FileExists(nupkgFileName))
      {
        context.FileSystem.DeleteFile(nupkgFileName);
      }

      if (!context.FileSystem.FileExists(nuspecFileName))
      {
        return;
      }

      this.BuildNupkgFile(context, nuspecFileName, nupkgFileName);

      context.OutputFiles.Add(nupkgFileName);

      context.Trace.TraceInformation("NuGet file size", new FileInfo(nupkgFileName).Length.ToString("#,##0"));
    }

    private void BuildNupkgFile([NotNull] IBuildContext context, [NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
    {
      try
      {
        using (var nuspec = new FileStream(nuspecFileName, FileMode.Open, FileAccess.Read))
        {
          var packageBuilder = new PackageBuilder(nuspec, Path.GetDirectoryName(nupkgFileName));

          using (var nupkg = new FileStream(nupkgFileName, FileMode.Create))
          {
            packageBuilder.Save(nupkg);
          }
        }
      }
      catch (Exception ex)
      {
        context.Trace.TraceError("Failed to create the Nupkg file", ex.Message);
      }
    }
  }
}
