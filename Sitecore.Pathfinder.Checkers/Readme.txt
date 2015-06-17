Sitecore.Pathfinder.Checkers
============================

The Sitecore.Pathfinder.Checkers assemblies is not references in the project and is not included in the 
project output.

Instead the source files are linked into the /Sitecore.Pathfinder.Core/files/extensions folder. From here they
are copied to the output directory as .cs files. When Pathfinder runs, it will compile the source files 
on-the-fly.

This project is used for developing the checkers and including the code when refactoring.
