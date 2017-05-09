# Extensibility
Pathfinder uses [MEF](https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx) internally and is fully pluggable. 

## Extensions
Extensions are .dll files that loaded dynamically through 
[MEF](https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx). This allows you to extend Pathfinder with new tasks, checkers, code 
generation handler and much more. 

When Pathfinder starts it looks through the [Tools]/files/extensions, [Project]/sitecore.project/extensions and [Project]/node_modules
directories to find any extension files.

### Npm modules
Extension can be installed using Npm modules. As mentioned above Pathfinder scans the [Project]/node_modules on startup.

Since a project may contain many Node modules, Pathfinder looks for a manifest file named "pathfinder.json" in the first level of
directories under [Project]/node_modules. Only directories that contain a manifest file, will be added as a extension.

The manifest is just a marker and can be empty (for now).

Any assemblies in the directory are loaded.
