namespace Sitecore.Pathfinder.Building.Initializing.BeforeBuilds
{
  using System;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class BeforeBuild : TaskBase
  {
    public BeforeBuild() : base("before-build")
    {
    }

    public override void Execute(IBuildContext context)
    {
      var projectDirectory = context.SolutionDirectory;
      if (!context.FileSystem.DirectoryExists(projectDirectory))
      {
        return;
      }

      var configFileName = PathHelper.Combine(projectDirectory, context.Configuration.Get(Constants.ConfigFileName));
      if (!context.FileSystem.FileExists(configFileName))
      {
        return;
      }

      var hostName = context.Configuration.Get(Constants.HostName);
      var wwwroot = context.Configuration.Get(Constants.Wwwroot);
      if (string.Compare(hostName, "http://sitecore.default", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(wwwroot, "c:\\inetpub\\wwwroot\\Sitecore.Default", StringComparison.OrdinalIgnoreCase) == 0)
      {
        context.Trace.Writeline(ConsoleTexts.Text4003, context.Configuration.Get(Constants.ConfigFileName));
        context.IsAborted = true;
        return;
      }

      var dataFolder = PathHelper.Combine(wwwroot, "Data");
      if (!context.FileSystem.DirectoryExists(dataFolder))
      {
        context.Trace.Writeline(ConsoleTexts.Text4004, context.Configuration.Get(Constants.ConfigFileName));
        context.IsAborted = true;
        return;
      }

      var website = PathHelper.Combine(wwwroot, "Website");
      if (!context.FileSystem.DirectoryExists(website))
      {
        context.Trace.Writeline(ConsoleTexts.Text4005, context.Configuration.Get(Constants.ConfigFileName));
        context.IsAborted = true;
        return;
      }

      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.ToolsPath), "wwwroot\\website");
      var serverAssemblyFileName = Path.Combine(website, "bin\\Sitecore.Pathfinder.Server.dll");
      if (!context.FileSystem.FileExists(serverAssemblyFileName))
      {
        context.FileSystem.XCopy(sourceDirectory, website);
        context.Trace.Writeline(ConsoleTexts.Text4006, context.Configuration.Get(Constants.ConfigFileName));
      }
      else
      {
        var localFileName = Path.Combine(context.Configuration.Get(Constants.ToolsPath), "wwwroot\\website\\bin\\Sitecore.Pathfinder.Server.dll");

        var serverVersion = new Version(FileVersionInfo.GetVersionInfo(serverAssemblyFileName).FileVersion);
        var localVersion = new Version(FileVersionInfo.GetVersionInfo(localFileName).FileVersion);

        if (serverVersion < localVersion)
        {
          context.FileSystem.XCopy(sourceDirectory, website);
          context.Trace.Writeline(ConsoleTexts.Text4007, context.Configuration.Get(Constants.ConfigFileName));
        }
      }
    }
  }
}
