Version Next
============
* Add: Project changed event for invalidating caches (2015-12-01)
* Add: Project roles - enables/disabled checkers and conventions (2015-12-01)
* Add: Rules Engine (2015-12-01)
* Add: Convention checker (2015-12-01)
* Add: XPath expressions (2015-12-03)
* Add: PathMapper API (2015-12-07)
* Add: Serializing Data Provider (2015-12-09)
* Add: Stricter checking using build-project:schema setting (2015-12-12)
* Add: Renamed /content directory to /items to avoid conflict with ASP.NET 5 (2015-12-12)

Version 0.5.0
=============
* Add: Support for Include files (2015-10-21)
* Add: Pack and install multiple NuGet packages (2015-10-29) - Todd Mitchell
* Add: Support for user config files (scconfig.json.user) (2015-10-29) - Alistair Denneys
* Add: Colored Console output (2015-11-01) - Max Reimer-Nielsen
* Add: Allows sitecore.tools to be added to the PATH environment variable (2015-11-01) - Martin Svarrer Christensen
* Add: Troubleshooting task; republish, rebuild search indexes, rebuild link database (2015-11-01) - SPEAK team
* Add: Revamped the entire process for creating a new project (2015-11-04) - Martin Svarrer Christensen
* Add: reset-website tak for deleting items and files from the website (2015-11-06) - SPEAK team
* Add: install-project task for installing a project directly from the project directory (2015-11-06)
* Add: watch-project task for watching a project for changes and installing the project (2015-11-06)
* Add: build-project:force-update setting which indicates if media and files are always overwritten (2015-11-06)
* Add: build-project:file-search-pattern sets the file search pattern for project directory visitor (2015-11-06)
* Add: Replaced externals with projects. Exports are now defined in a standard NuGet package (2015-11-09) - Dmitry Kostenko
* Add: Repository directory for installable files. list-repository and install-repository tasks (2015-11-09)
* Add: Support for Unicorn files (2015-11-17) - Emil Okkels Klein
* Changed: Renamed list-repository to list-addins and install-repository to install-addin (2015-11-17)
* Add: Added update-addins task to update installed add-ins (2015-11-17)
* Add: Added /disable-extensions=true switch to prevent extensions from loading (2015-11-17)
* Add: Added support for T4 templates (2015-11-18) - Emil Okkels Klein
* Removed: run-unittests and generate-unittests tasks. These have been replaced with generate-code and T4 templates (2015-11-18)
* Add: scc can now run scripts files (PowerShell, .cmd and -bat) (2015-11-19)
* Add: Script files for installing FakeDb and NUnit-Runners (2015-11-19)
* Add: import-website task for importing a website into a Pathfinder project (2015-11-20)
