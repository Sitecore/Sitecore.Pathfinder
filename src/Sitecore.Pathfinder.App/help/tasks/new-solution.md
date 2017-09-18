new-project
===========
Creates a new Pathfinder solution.

Remarks
-------
A solution may contain one or more projects.

The `new-solution` task is a wizard that generates a new Pathfinder solution by asking a number of questions.

In addition to creating a scconfig.json, the task also creates a scconfig.solution.json that contains references
to the file location of the projects to compile. By default the task scans all subdirectories for scconfig.json files
and .csproj files.

If a project is located in a directory named 'code', Pathfinder will use the parent directory as project directory
to support Habitat conventions.

Settings
--------
| Setting name                          | Description                            | 
|---------------------------------------|----------------------------------------|
| new-project:default-host-name         | The default host name.                 |
| new-project:default-wwwroot-directory | The default directory of the wwwwroot. |
| new-project:wwwroot-directory         | The wwwroot directory.                |

Example
-------
```cmd
> scc new-solution

Welcome to Sitecore Pathfinder.

Pathfinder needs 4 pieces of information to create a new project; a unique Id for the project, the 
Sitecore website and data folder directories to deploy to, and the hostname of the website. If you 
have not yet created a Sitecore website, use a tool like Sitecore Instance Manager to create it for you.

The project's unique ID can be a unique string (like MyCompany.MyProject) or a Guid. If you do not 
specify a unique ID, Pathfinder will generate a Guid for you.

You should *not* change the project unique ID at a later point, since Sitecore item IDs are dependent on it.

Enter the project unique ID [{865DF9EF-7CB7-4AF4-9E9A-BE6C7AB0F19F}]:

Pathfinder requires physical access to both the Website and DataFolder directories to deploy packages.

Enter the directory of the Website [e:\inetpub\wwwroot\Pathfinder\Website]:

Enter the directory of the DataFolder [e:\inetpub\wwwroot\Pathfinder\Data]:

Finally Pathfinder requires the hostname of the Sitecore website.

Enter the hostname of the website [pathfinder]:

Creating project...
```

