namespace Sitecore.Pathfinder
{
  public static class Constants
  {
    public const string ConfigFileName = "system:configfilename";

    public const string Database = "database";

    public const string DataFolderName = "dataFolderName";

    public const string Debug = "system:debug";

    public const string HostName = "HostName";

    public const string IgnoreDirectories = "ignoredirectories";

    public const string IgnoreFileNames = "ignorefilenames";

    public const string InstallUrl = "deploying:installurl";

    public const string ItemPath = "itemPath";

    public const string PackageDirectory = "deploying:packagedirectory";

    public const string Pathfinder = "Pathfinder";

    public const string ProjectDirectory = "projectdirectory";

    public const string PublishUrl = "deploying:publishurl";

    public const string SolutionDirectory = "solutiondirectory";

    public const string ToolsDirectory = "system:toolspath";

    public const string Wwwroot = "wwwroot";

    public static readonly char[] Pipe =
    {
      '|'
    };

    public static readonly char[] Space =
    {
      ' '
    };

    public static class Configuration
    {
      public const string ProjectUniqueId = "project-unique-id";
    }

    public static class Emitters
    {
      public const double BinFiles = 9999;

      public const double ContentFiles = 4000;

      public const double Items = 2000;

      public const double Layouts = 2500;

      public const double MediaFiles = 1500;

      public const double Templates = 1000;
    }

    public static class Parsers
    {
      public const double BinFiles = 9999;

      public const double ContentFiles = 9000;

      public const double Items = 3000;

      public const double Media = 2000;

      public const double Renderings = 5000;

      public const double System = 100;

      public const double Templates = 1000;
    }

    public static class Templates
    {
      public const string Layout = "{3A45A723-64EE-4919-9D41-02FD40FD1466}";

      public const string StandardTemplate = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";

      public const string Sublayout = "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}";

      public const string ViewRendering = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";
    }
  }
}
