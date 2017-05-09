# Sitecore Pathfinder

[![Build status](https://ci.appveyor.com/api/projects/status/21a8xc3s80mcic81?svg=true)](https://ci.appveyor.com/project/JakobChristensen/sitecore-pathfinder) [![MyGet Prerelease](https://img.shields.io/myget/sitecore-pathfinder/vpre/Sitecore.Pathfinder.svg?label=version)](https://www.myget.org/feed/sitecore-pathfinder/package/nuget/Sitecore.Pathfinder) 

Get started, get far, get happy!

An experimental CLI for Sitecore.

![Pathfinder](docs/img/SitecorePathfinder.png)
 
Download [the latest Pathfinder build](https://github.com/JakobChristensen/Sitecore.Pathfinder/releases) to 
try it out.

## Common commands 

Command | Description
------- | -----------
scc build | Build a Sitecore Package
scc build unicorn | Build a directory of Unicorn files
scc build nuget | Build a Nuget package
scc test | Validate the project againt many checkers (currently more that 130 checks)
scc generate yaml [filename] | Create a new Yaml item file
scc generate json [filename] | Create a new Json item file
scc generate xml [filename] | Create a new Xml item file
scc init | Create an initial configuration file (scconfig.json) - Optional

Follow the walk-throughs:

* [Setting up](docs/walkthroughs/1. Setting up/README.md)
* [HelloWorld](docs/walkthroughs/2. Hello world/README.md)

## Introduction
Pathfinder is an experimental CLI for Sitecore, that allows developers to use their favorite tools 
in a familiar fashion to develop Sitecore websites.

The CLI creates a deliverable package from the source files in a project directory. The package can
be installed in Sitecore using the Package Installer, Unicorn or a custom installer.

The developer process is familiar; edit source files, build and install the package, run tests or review the 
changes on website, repeat.

Pathfinder works with Unicorn, Sitecore Rocks and many other Sitecore tools.

_Please notice that this document is a brain dump, so concepts and functionality are probably not explained 
in a friendly manner._

### How does Pathfinder make Sitecore development easier
* Familiar developer experience: Edit source files, build project, test, repeat...
* Text editor agnostic (Visual Studio not required - use Notepad, Notepad++, SublimeText, Atom, VS Code etc.)
* Build process agnostic (command-line tool, so it integrates easily with Grunt, Gulp, MSBuild etc.)
* Everything is a file (easy to edit, search and replace across multiple files, source control friendly)
* Project directory has whole and single truth (source is not spread across development projects, databases and websites) (contineous integration friendly) 
* Project is packaged into a Sitecore Package, NuGet package, Unicorn files or a directory of files
* Lint or check you project and website with over 120 checkers

## FAQ
Read the [FAQ](FAQ.md)

## Documentation
For more documentation see [Pathfinder Documentation](docs/README.md).
