namespace Sitecore.Pathfinder
{
  using System.Collections.Generic;

  public static class Texts
  {
    public const int Text1001 = 1001;

    public const int Text1002 = 1002;

    public const int Text1003 = 1003;

    public const int Text1004 = 1004;

    public const int Text1005 = 1005;

    public const int Text1006 = 1006;

    public const int Text1007 = 1007;

    public const int Text1008 = 1008;

    public const int Text1009 = 1009;

    public const int Text1010 = 1010;

    public const int Text1011 = 1011;

    public const int Text1012 = 1012;

    public const int Text1013 = 1013;

    public const int Text1014 = 1014;

    public const int Text1015 = 1015;

    public const int Text1016 = 1016;

    public const int Text1017 = 1017;

    public const int Text1018 = 1018;

    public const int Text1019 = 1019;

    public const int Text1020 = 1020;

    public const int Text1021 = 1021;
    public const int Text1022 = 1022;
    public const int Text1023 = 1023;

    public const int Text2000 = 2000;

    public const int Text2003 = 2003;

    public const int Text2004 = 2004;

    public const int Text2005 = 2005;

    public const int Text2006 = 2006;

    public const int Text2007 = 2007;

    public const int Text2008 = 2008;

    public const int Text2009 = 2009;

    public const int Text2010 = 2010;

    public const int Text2011 = 2011;

    public const int Text2012 = 2012;

    public const int Text2013 = 2013;

    public const int Text2014 = 2014;

    public const int Text2015 = 2015;

    public const int Text2016 = 2016;

    public const int Text2017 = 2017;

    public const int Text2018 = 2018;

    public const int Text2019 = 2019;

    public const int Text2020 = 2020;

    public const int Text2021 = 2021;

    public const int Text2022 = 2022;

    public const int Text2023 = 2023;

    public const int Text2024 = 2024;

    public const int Text2025 = 2025;

    public const int Text2026 = 2026;

    public const int Text2027 = 2027;

    public const int Text2028 = 2028;

    public const int Text2029 = 2029;

    public const int Text2030 = 2030;

    public const int Text2031 = 2031;

    public const int Text2032 = 2032;

    public const int Text2033 = 2033;

    public const int Text2034 = 2034;

    public const int Text2035 = 2035;

    public const int Text2036 = 2036;

    public const int Text2037 = 2037;

    public const int Text3000 = 3000;

    public const int Text3001 = 3001;

    public const int Text3002 = 3002;

    public const int Text3006 = 3006;

    public const int Text3008 = 3008;

    public const int Text3009 = 3009;

    public const int Text3010 = 3010;

    public const int Text3011 = 3011;

    public const int Text3012 = 3012;

    public const int Text3013 = 3013;

    public const int Text3014 = 3014;

    public const int Text3016 = 3016;

    public const int Text3017 = 3017;

    public const int Text3018 = 3018;

    public const int Text3019 = 3019;

    public const int Text3020 = 3020;

    public const int Text3021 = 3021;

    public const int Text3022 = 3022;

    public const int Text3023 = 3023;
    public const int Text3024 = 3024;
    public const int Text3025 = 3025;
    public const int Text3026 = 3026;
    public const int Text3027 = 3027;
    public const int Text3028 = 3028;
    public const int Text3029 = 3029;
    public const int Text3030 = 3030;

    public const int Text9998 = 9998;

    public const int Text9999 = 9999;

    // todo: make text messages pluggable
    public static readonly Dictionary<int, string> Messages = new Dictionary<int, string>
    {
      { Text1001, "Cleaning output directory..." }, 
      { Text1002, "Incremental build started..." }, 
      { Text1003, "Source files changed: {0}" }, 
      { Text1004, "Creating Nuspec file..." }, 
      { Text1005, "Creating Nupkg file..." }, 
      { Text1006, "Creating 'layout' directory..." }, 
      { Text1007, "Copying package to website..." }, 
      { Text1008, "Installing package on website..." }, 
      { Text1009, "Publishing..." }, 
      { Text1010, "Linting..." }, 
      { Text1011, "Loading project..." }, 
      { Text1012, "Pipeline is empty. There are no tasks to execute." },
      { Text1013, "Hi there." },
      { Text1014, "Your project directory was missing, so I have created it. You must update the 'project-unique-id', 'wwwroot' and 'hostname' in the '{0}' configuration file before continuing." },
      { Text1015, "Your configuration file and sample files were missing, so I have created them. You must update the 'project-unique-id', 'wwwroot' and 'hostname' in the '{0}' configuration file before continuing." },
      { Text1016, "Hey - you haven't changed the the 'project-unique-id', 'wwwroot' or 'hostname' in the '{0}' configuration file." },
      { Text1017, "Hey - there is no 'Data' directory under the 'wwwroot' directory - are you sure, you have set the 'wwwroot' correctly in the configuration file" },
      { Text1018, "Hey - there is no 'Website' directory under the 'wwwroot' directory - are you sure, you have set the 'wwwroot' correctly in the configuration file" },
      { Text1019, "Just so you know, I have copied the 'Sitecore.Pathfinder.Server.dll' and 'NuGet.Core.dll' assemblies to the '/bin' directory in the website and a number of '.aspx' files to the '/sitecore/shell/client/Applications/Pathfinder' directory" },
      { Text1020, "Just so you know, I have updated the 'Sitecore.Pathfinder.Server.dll' and 'NuGet.Core.dll' assemblies in the '/bin' directory in the website and a number of '.aspx' files in the '/sitecore/shell/client/Applications/Pathfinder' directory to the latest version" },
      { Text1021, "Linting items: {0}" },
      { Text1022, "NuGet file size: {0}" },
      { Text1023, "Project files: {0} / project items: {1}" },

      { Text2000, "Item file is not valid: {0}" }, 
      { Text2003, "Item not found" }, 
      { Text2004, "Failed to create template." }, 
      { Text2005, "'Path' element must have a 'Path' attribute" }, 
      { Text2006, "'Template' element must have a 'Name' attribute" }, 
      { Text2007, "'Section' element must have a 'Name' attribute" }, 
      { Text2008, "'Field' element must have a 'Name' attribute" }, 
      { Text2009, "'Item' element must have a 'Name' attribute" }, 
      { Text2010, "'Item' element must have a 'Template' or 'Template.Create' attribute" }, 
      { Text2011, "'Field' element must have a 'Name' attribute" }, 
      { Text2012, "Field '{0}' is already defined" }, 
      { Text2013, "Failed to upload media" }, 
      { Text2014, "Layout file is not valid" }, 
      { Text2015, "Unexpected element" }, 
      { Text2016, "Template missing: {0}" }, 
      { Text2017, "Template missing: {0}" }, 
      { Text2018, "Database not found: {0}" }, 
      { Text2019, "Failed to create item path: {0}" }, 
      { Text2020, "Layout contains errors" }, 
      { Text2021, "Media item not found: {0}" }, 
      { Text2022, "Failed to upload media" }, 
      { Text2023, "Failed to add new template" }, 
      { Text2024, "Cannot apply a layout to a template. The template needs a Standard Values." }, 
      { Text2025, "Installation started..." }, 
      { Text2026, "{0}" }, 
      { Text2027, "{0}" }, 
      { Text2028, "Directory is empty" }, 
      { Text2029, "Layout \"{0}\" not found." }, 
      { Text2030, "Item not found: {0}" }, 
      { Text2031, "Failed to add new template" }, 
      { Text2032, "'Component' element expected" }, 
      { Text2033, "'Component' attribute expected" }, 
      { Text2034, "Component not found: {0}" }, 
      { Text2035, "Field is not defined in the template" }, 
      { Text2036, "Template missing: {0}" }, 
      { Text2037, "Base Template missing: {0}" }, 
      { Text3000, "Configuration failed spectacularly" }, 
      { Text3001, "Task '{0}' not found. Skipping." }, 
      { Text3002, "System configuration file not found: {0}" }, 
      { Text3006, "The configuration element 'packaging:nuspec:filename' is missing" }, 
      { Text3008, "The server returned an error: {0}" }, 
      { Text3009, "An error occured: {0}" }, 
      { Text3010, "Directory is empty" }, 
      { Text3011, "Package contains errors and will not be deployed" }, 
      { Text3012, "The file does not contain valid XML: {0}" }, 
      { Text3013, "{0}" }, 
      { Text3014, "{0}" }, 
      { Text3016, "Directory is empty: {0}" }, 
      { Text3017, "Cannot writer serialization item: Database is null" }, 
      { Text3018, "Cannot writer serialization item: ID is null" }, 
      { Text3019, "Cannot writer serialization item: Path is null" }, 
      { Text3020, "Cannot writer serialization item: ParentID is null" }, 
      { Text3021, "Cannot writer serialization item: ParentID is null" }, 
      { Text3022, "Cannot writer serialization item: TemplateID is null" }, 
      { Text3023, "Cannot writer serialization item: TemplateName is null" }, 
      { Text3024, "Reference not found {0}" }, 
      { Text3025, "Source file is empty" }, 
      { Text3026, "Json file is not valid" }, 
      { Text3027, "Value is specified in both 'Value' attribute and in element. Using value from attribute" }, 
      { Text3028, "Could not create item: {0}" }, 
      { Text3029, "Unique ID clash: {0} / {1}" }, 
      { Text3030, "Item not found when updating layout: {0}" }, 

      { Text9998, "{0}" }, 
      { Text9999, "An error occured: {0}" }
    };
  }
}
