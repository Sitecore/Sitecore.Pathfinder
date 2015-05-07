namespace Sitecore.Pathfinder.Building.Packaging.CreateNupkg
{
  using System;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using NuGet;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class CreateNupkg : TaskBase
  {
    public CreateNupkg() : base("create-nupkg")
    {
    }

    public override void Execute(IBuildContext context)
    {
      if (!context.IsDeployable)
      {
        return;
      }

      context.Trace.TraceInformation(ConsoleTexts.Text1005);

      var packageFileName = context.Configuration.Get("packaging:nuspec:filename");
      var nuspecFileName = PathHelper.Combine(context.OutputDirectory, packageFileName);
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

      if (!context.SourceFiles.Any())
      {
        return;
      }

      this.BuildNupkgFile(nuspecFileName, nupkgFileName);

      context.OutputFiles.Add(nupkgFileName);
    }

    private void BuildNupkgFile([NotNull] string nuspecFileName, [NotNull] string nupkgFileName)
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
        Trace.TraceError("Failed to create the nupkg file: {0}", ex.Message);
      }
    }
  }
}
