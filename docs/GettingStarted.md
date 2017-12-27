# Getting started
The goal of Pathfinder is to make it easy to start working with Sitecore.

## Installing Pathfinder
1. Run `npm install -g sitecore-pathfinder` to install Sitecore Pathfinder.

## Creating a new project
1. Install a Sitecore website (e.g. using [SIM (Sitecore Instance Manager)](https://marketplace.sitecore.net/modules/sitecore_instance_manager.aspx)
1. Create an empty directory (seperate from the Sitecore website directory)
1. Run `scc init` in the directory
1. Recommended: Some tasks require read/access to your tool directory and project directory, so make sure appropriate secutity settings are applied.
1. Done

## Creating a Pathfinder project in an existing directory
1. In the existing directory, run the `scc init` command. This will create the scconfig.json file in the directory
2. Done

## Importing an existing project
You usually only do this once at the beginning of a project. 

In Sitecore Rocks right-click the root item, that you want to import into the project, and select Tools | Xml| Export as Yaml command.

## The smoothest setup
* Add the Pathfinder binaries to your environment path, so it can be executed from anywhere.
* Use a project folder structure like this - it is compatible with ASP.NET 5, NodeJS and Sitecore serialization.
```
<project name>\
    items\               # Sitecore items
        core\            # Sitecore core database items
        master\          # Sitecore master database items
```

* Use the default website structure.
```
<website name>\          
    Data\                # Sitecore data folder
    Databases\           # Sitecore databases
    Website\             # Sitecore website
```

* Keep your project and website separate - this makes it easier to see what is in your project, and you can upgrade your website to another version easily.
* Do not disable any checkers - the checkers are there for a good reason, and you should strive for no warnings or errors.
* Set the project-role to enable additional checkers, conventions and functionality.
* If you are using Visual Studio, consider adding scc.cmd as either a Post-Build step in Build Events or as an MSBuild AfterBuild task in the .csproj file.
* Use source control.

### Performance tips
* Don't use serialization items (*.item) - they are slow to deserialize.

## Command line help
To get help, you can execute the Help task by entering `scc help`.

To get help about a specific task, execute the Help task with the name of the task as a parameter: `scc help [task name]`

![Command Line Help](img/CommandLineHelp.PNG)
