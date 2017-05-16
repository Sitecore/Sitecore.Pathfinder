# Sitecore Pathfinder

[![Build status](https://ci.appveyor.com/api/projects/status/21a8xc3s80mcic81?svg=true)](https://ci.appveyor.com/project/JakobChristensen/sitecore-pathfinder) [![MyGet Prerelease](https://img.shields.io/myget/sitecore-pathfinder/vpre/Sitecore.Pathfinder.svg?label=version)](https://www.myget.org/feed/sitecore-pathfinder/package/nuget/Sitecore.Pathfinder) 

Get started, get far, get happy!

An experimental CLI for Sitecore.

![Pathfinder](docs/img/SitecorePathfinder.png)
 
Download [the latest Pathfinder build](https://github.com/JakobChristensen/Sitecore.Pathfinder/releases) to 
try it out.

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
* Lint or check you project and website with more than 120 checkers

## Quickstart

1. Create an new empty directory
1. Execute `scc g hello` - this creates the files for the HelloWorld sample
1. Execute `scc b` - this creates a Sitecore Package in the ./dist directory containing files and items
1. Install the package in a Sitecore website
1. Navigate to http://mysite/home/HelloWorld

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

## FAQ
Read the [FAQ](FAQ.md)

## Documentation
For more documentation see [Pathfinder Documentation](docs/README.md).

## Deploying
Pathfinder only outputs a package (Sitecore Packages, Nuget packages, Unicorn files etc.) - 
it does *not* install the package in the website.

### Sitecore Packages
To install a Sitecore Package, use the Sitecore Package Installer to install the packages.

### Unicorn
Pathfinder can write Unicorn files in the output directory. You can configure Pathfinder to mirror these
files to the Unicorn serialization directory, then have Unicorn deserialize these files.

You can configure the mirroring in the output section of the scconfig.json file, like this:

```js
{
    "output": {
        "unicorn": {
            "items-directory": "items",
            "mirror-items-to-unicorn-physicalRootPath": true,
            "mirror-items-source-directory": "/master/sitecore/content/Home",
            "unicorn-physicalRootPath": "c:\\inetpub\\wwwroot\\Pathfinder\\Data\\Unicorn\\items\\Home"
        }
    }
}
```

You need to be careful setting the directories, and be aware that Unicorn does not like empty folders.

### Nuget packages
To install Nuget packages, you can use Sitecore.Pathfinder.Server. It monitors the [Data]/Pathfinder
directory for files and installs any new files.

Install the Sitecore.Pathfinder.zip Sitecore Package in the website, and configure the output
directory to point to the [Data]/Pathfinder directory. 