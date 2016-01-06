Get started, get far, get happy!

An experimental tool chain for Sitecore.

## Introduction
Pathfinder is an experimental toolchain for Sitecore, that allows developers to use their favorite tools 
in a familiar fashion to develop Sitecore websites.

The toolchain creates a deliverable package from the source files in a project directory and deploys 
the package to a website where an installer installs the new files and Sitecore items.

The developer process is familiar; edit source files, build and install the package, run tests or review the 
changes on website, repeat.

*Please notice that this document is a brain dump, so concepts and functionality are probably not explained 
in a friendly manner.*

### How does Pathfinder make Sitecore development easier
* Familiar developer experience: Edit source files, build project, test, repeat...
* Text editor agnostic (Visual Studio not required - use Notepad, Notepad++, SublimeText, Atom, VS Code etc.)
* Build process agnostic (command-line tool, so it integrates easily with Grunt, Gulp, MSBuild etc.)
* Everything is a file (easy to edit, search and replace across multiple files, source control friendly)
* Project directory has whole and single truth (source is not spead across development projects, databases and websites) (contineous integration friendly) 
* Project is packaged into a NuGet package and deployed to the website
  * Dependency tracking through NuGet dependencies
  * NuGet package installer on Sitecore website
  * SitecorePathfinderCore NuGet package tweaks Sitecore defaults to be easier to work with (e.g. removes initial workflow)
* Support for Html Templates (with [Mustache](https://mustache.github.io/mustache.5.html) tags) makes getting started with the Sitecore Rendering Engine easier
* Code Generation for generating strongly typed item model, factories and unit tests
* Validate a Sitecore website against 70 rules using Sitecore Rocks SitecoreCop

## Documentation
For more documentation see [Pathfinder Documentation](https://github.com/JakobChristensen/Sitecore.Pathfinder/blob/master/docs/README.md).