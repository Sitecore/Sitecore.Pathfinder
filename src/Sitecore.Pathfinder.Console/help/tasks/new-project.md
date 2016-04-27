new-project
===========
Creates a new Pathfinder project.

Remarks
-------
The `new-project` tasks is a wizard that generates a new Pathfinder project by asking a number of questions.

Settings
--------
| Setting name                          | Description                            | 
|---------------------------------------|----------------------------------------|
| new-project:default-host-name         | The default host name.                 |
| new-project:default-wwwroot-directory | The default directory of the wwwwroot. |
| new-project:wwwroot-directory         | The wwwroot  directory.                |

Example
-------
```cmd
> scc new-project

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

Do you want to install an editor configuration [Y]:

1: Atom
2: VisualStudio.ClassLibrary
3: VisualStudio.Website
4: VSCode
Select editor [1]: 4

Do you want to install a starter kit [Y]:

1: CleanBlog
2: HelloWorld
3: TodoMvc
Select starter kit [1]: 1

Creating project...
```

