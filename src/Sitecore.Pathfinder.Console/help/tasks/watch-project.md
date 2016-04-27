watch-project
=============
Watches the project directory and install changes immediately.

Remarks
-------
The 'project-website-mappings' settings enables the serializing data provider on the website to serialize changed items back to the projects.
This task should be called when the 'project-website-mappings' settings have been changed (or you can just kill the 'w3wp.exe' process or restart IIS).

Settings
--------
| Setting name                   | Description                                                             | 
|--------------------------------|-------------------------------------------------------------------------|
| watch-project:include          | Specifies which files to look for                                       |
| watch-project:exclude          | Specifies which files to ignore                                         |
| watch-project:publish-database | Indicates if the database should published after installing the project |

Example
-------
```cmd
> scc watch-project
```