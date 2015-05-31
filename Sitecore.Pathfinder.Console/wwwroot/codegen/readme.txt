This directory contains template files for code generation.

You need to specify how project items and template files are related 
in the project "scconfig.json" configuration file.

Here is an example:

// maps a code generation template file to a project item type
"codegen": {
  ".sitecore.tools\\wwwroot\\codegen\\Template.cshtml": "Sitecore.Pathfinder.Projects.Templates.Template, Sitecore.Pathfinder.Core"
}

This maps any project item of the C# class "Sitecore.Pathfinder.Projects.Templates.Template" to the "Template.cshtml" template file.