init-vscode
===========
Configures the project for use with Visual Studio Code.

Remarks
-------
The 'init-vscode' task adds a '.vscode' directory to the project which contains the 'tasks.json' and 'settings.json' files.
These configures VS Code build tasks and schema settings.

The task simply unzips the [TOOLS]/files/editors/VSCode.zip file into the project.

Settings
--------
None.

Example
-------
```cmd
> scc init-vscode
```

