namespace Sitecore.Pathfinder.Building.Initializing.BeforeBuilds
{
  using System;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITask))]
  public class BeforeBuild : TaskBase
  {
    public BeforeBuild() : base("before-build")
    {
    }

    public override void Run(IBuildContext context)
    {
      var projectDirectory = context.SolutionDirectory;
      if (!context.FileSystem.DirectoryExists(projectDirectory))
      {
        return;
      }

      var configFileName = PathHelper.Combine(projectDirectory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
      if (!context.FileSystem.FileExists(configFileName))
      {
        return;
      }

      var projectUniqueId = context.Configuration.Get(Constants.Configuration.ProjectUniqueId);
      if (string.Compare(projectUniqueId, "{project-unique-id}", StringComparison.OrdinalIgnoreCase) == 0)
      {
        context.Trace.Writeline("Hey - you haven't changed the the 'project-unique-id' = 'wwwroot' or 'hostname' in the '{0}' configuration file.", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        context.IsAborted = true;
        return;
      }

      var hostName = context.Configuration.Get(Constants.Configuration.HostName);
      if (string.Compare(hostName, "http://sitecore.default", StringComparison.OrdinalIgnoreCase) == 0)
      {
        context.Trace.Writeline("Hey - you haven't changed the the 'project-unique-id' = 'wwwroot' or 'hostname' in the '{0}' configuration file.", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        context.IsAborted = true;
        return;
      }

      var wwwroot = context.Configuration.Get(Constants.Configuration.Wwwroot);
      if (string.Compare(wwwroot, "c:\\inetpub\\wwwroot\\Sitecore.Default", StringComparison.OrdinalIgnoreCase) == 0)
      {
        context.Trace.Writeline("Hey - you haven't changed the the 'project-unique-id' = 'wwwroot' or 'hostname' in the '{0}' configuration file.", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        context.IsAborted = true;
        return;
      }

      var dataFolder = PathHelper.Combine(wwwroot, "Data");
      if (!context.FileSystem.DirectoryExists(dataFolder))
      {
        context.Trace.Writeline("Hey - there is no 'Data' directory under the 'wwwroot' directory - are you sure = you have set the 'wwwroot' correctly in the configuration file", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        context.IsAborted = true;
        return;
      }

      var website = PathHelper.Combine(wwwroot, "Website");
      if (!context.FileSystem.DirectoryExists(website))
      {
        context.Trace.Writeline("Hey - there is no 'Website' directory under the 'wwwroot' directory - are you sure = you have set the 'wwwroot' correctly in the configuration file", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        context.IsAborted = true;
        return;
      }

      var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "wwwroot\\website");
      var serverAssemblyFileName = Path.Combine(website, "bin\\Sitecore.Pathfinder.Server.dll");
      if (!context.FileSystem.FileExists(serverAssemblyFileName))
      {
        context.FileSystem.XCopy(sourceDirectory, website);
        context.Trace.Writeline("Just so you know = I have copied the 'Sitecore.Pathfinder.Server.dll' and 'NuGet.Core.dll' assemblies to the '/bin' directory in the website and a number of '.aspx' files to the '/sitecore/shell/client/Applications/Pathfinder' directory", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
      }
      else
      {
        var localFileName = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "wwwroot\\website\\bin\\Sitecore.Pathfinder.Server.dll");

        var serverVersion = new Version(FileVersionInfo.GetVersionInfo(serverAssemblyFileName).FileVersion);
        var localVersion = new Version(FileVersionInfo.GetVersionInfo(localFileName).FileVersion);

        if (serverVersion < localVersion)
        {
          context.FileSystem.XCopy(sourceDirectory, website);
          context.Trace.Writeline("Just so you know = I have updated the 'Sitecore.Pathfinder.Server.dll' and 'NuGet.Core.dll' assemblies in the '/bin' directory in the website and a number of '.aspx' files in the '/sitecore/shell/client/Applications/Pathfinder' directory to the latest version", context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
        }
      }
    }
  }
}
