# Pathfinder

### Disclaimer
Pathfinder is a personal side project. It is not endorsed or supported by Sitecore in any way.

## Get Started
Get started, get far, get happy!

An experimental tool chain for Sitecore.

![Pathfinder](docs/img/SitecorePathfinder.png)
 
Watch the videos on YouTube (please notice that some details in the videos are out of date):

* [01 - Idea and concepts](https://www.youtube.com/watch?v=TcJ0IoI7sVM)
* [02 - HelloWorld](https://www.youtube.com/watch?v=jQz5hAVOTzU)
* [03 - Unit Testing](https://www.youtube.com/watch?v=DWU6D7L8ykg)
* [04 - Html Templates](https://www.youtube.com/watch?v=9aTGhW6ErYM)
* [05 - Code Generation, Visual Studio and Grunt](http://youtu.be/ZM3ve1WhwwQ)

Then download [Pathfinder 0.4.2-Alpha](http://vsplugins.sitecore.net/Pathfinder/Sitecore.Pathfinder.0.4.2.zip) to try it out.

# Introduction
Pathfinder is an experimental toolchain for Sitecore, that allows developers to use their favorite tools 
in a familiar fashion to develop Sitecore websites.

The toolchain creates a deliverable package from the source files in a project directory and deploys 
the package to a website where an installer installs the new files and Sitecore items.

The developer process is familiar; edit source files, build and install the package, run tests or review the changes on website, repeat.

## Getting started
The goal of Pathfinder is to make it easy to start working with Sitecore.

1. Install a Sitecore website (e.g. using [SIM (Sitecore Instance Manager](https://marketplace.sitecore.net/modules/sitecore_instance_manager.aspx))
2. Create an empty directory (seperate from the Sitecore website directory)
3. Run `scc.exe new-project` in the directory
4. Enter Project Unique ID, Website directory, Data Folder directory and hostname
5. Done

## How does Pathfinder make Sitecore development easier
* Familiar developer experience: Edit source files, build project, test website, repeat.
* Text editor agnostic (Visual Studio not required - use Notepad, Notepad++, SublimeText, Atom, VS Code etc.)
* Build process agnostic (command-line tool, so it integrates easily with Grunt, Gulp, MSBuild etc.)
* Everything is a file (easy to edit, source control friendly)
* Project directory has whole and single truth (source is not spead across development projects, databases and websites) (contineous integration friendly) 
* Project is packaged into a NuGet package and deployed to the website
  * Dependency tracking through NuGet dependencies
  * NuGet package installer on Sitecore website
  * Sitecore.Pathfinder.Core NuGet package tweaks Sitecore defaults to be easier to work with (e.g. removes initial workflow)
* Web Test Runner for running unit tests inside Sitecore website (supports dynamic compilation of C# source files)
* Support for Html Templates (with [Mustache](https://mustache.github.io/mustache.5.html) tags) makes getting started with the Sitecore Rendering Engine easier
* Validate a Sitecore website against 70 rules using Sitecore Rocks SitecoreCop

## Command line help
To get help, you can execute the Help task by entering `scc help`.

To get help about a specific task, execute the Help task with the name of the task as a parameter: `scc help [task name]`

![Command Line Help](docs/img/CommandLineHelp.PNG)

### Open source
Pathfinder is open source and you can freely make changes. To do so, clone the project from 
[GitHub](https://github.com/JakobChristensen/Sitecore.Pathfinder), and copy the following assemblies to the /components directory.

Also see [How to contribute](CONTRIBUTING.md).

## Basic concepts

### Tasks
The Pathfinder compiler supports a number of tasks. These tasks make up the tool chain. While most tasks provide functionality for compiling and 
deploying a package, other tasks simply implement funtionality.

To execute a task run `scc [task name]` from the command line.

Task Name | Description
----------|------------
check-project | Checks the project for warnings and errors.
copy-dependencies | Copies the dependency packages to the website.
copy-package | Copies the project output to the website.
find-references | Finds all project items that the specified project item references.
find-usages | Finds all project items that references the specified project item.
generate-code | Generates code from items and files in the project.
generate-unittests | Generates basic unit tests for the project.
help | Displays version information and a list of commands.
init-atom | Creates a new Atom project.
init-visualstudio | Creates a new Visual Studio project.
init-vscode | Creates a new Visual Studio Code project.
install-package | Unpacks and installs the project package (including dependencies) in the website.
list-items | Lists the Sitecore items in the project.
list-project | Lists the project items (Sitecore items and files).
new-project | Initializes the project.
pack-dependencies | Creates a Nuget package for Sitecore package in the /sitecore.tools/packages directory.
pack-nuget | Creates a Nuget package from the project.
publish-database | Publishes a Sitecore database (usually the master database).
rename | Finds all project items that references the specified project item (EXPERIMENTAL).
run-unittests | Runs the Unit Test Runner on the website.
show-metrics | Shows various information about the project.
sync-website | Synchronizes project and the website.
validate-website | Runs the Sitecore Rocks SitecoreCop on the website.

### Building
The build tool chain is specified in the build-project/tasks configuration setting. The default value is 
``"check-project pack-nuget copy-dependencies copy-package install-package publish-database show-metrics"``.

1. Check the project for warnings and errors. Any errors will stop the build.
2. Create a Nuget package from the project.
3. Copy dependency files from the /sitecore.project/packages directory to the website ([DataFolder]/Pathfinder/Available).
4. Copy package /sitecore.project/Sitecore.nupkg to the website ([DateFolder]/Pathfinder/Available).
5. Install the package by making a request to the website: /sitecore/shell/client/Applications/Pathfinder/InstallPackage
6. Publish the Master database by making a request to the website: /sitecore/shell/client/Applications/Pathfinder/Publish
7. Show project metrics.

### Configuration
Pathfinder is configured using a global configuration file, a project configuration file and optionally a user configuration file. The user and project 
configuration files can overwrite any settings in the global configuration file.

The global configuration is located in the /sitecore.tools/sccconfig.json. You should never change this file. Instead overwrite settings in 
the project configuration file.

[Global configuration: /sitecore.tools/scconfig.json](src/console/scconfig.json)

The project configuration file is located in the root of the project: /scconfig.json.

[Project configuration: /scconfig.json](src/console/files/project/scconfig.json)

The user configuration file is options and is located next to the project configuration file. It has the extension .user.

/sitecore.tools/scconfig.json (global configuration)
/scconfig.json (project configuration)
/scconfig.json.user (user configuration)

### Extensibility
Pathfinder uses [MEF](https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx) internally and is fully plugable. See section on 
extensions.

### Cross platform
Pathfinder is envisioned to become fully cross platform at some point. Currently Pathfinder is implemented in C# which binds it to the 
Windows platform. 

Pathfinder may take advantage of the upcoming .NET Core framework to support other platform or it may be rewritten in TypeScript
and run using NodeJS.

# Features

## Sitecore items and templates as files
In Pathfinder everything is a file, including Sitecore items. This is so that your project directory can contain the whole and single truth
about your project. Your project is no longer spread across development projects, databases and websites.

This is also how classic development projects work. In a Visual Studio application project every asset, that is needed by the application, is
included or referenced from the project.

Items are stored as files but can have a number of formats. Currently Json, Yaml and Xml formats are supported. Json and Xml are good formats, 
since code editors can support schema validation and IntelliSense.

Json format (extension .item.json): 
```js
{
    "Item": {
        "Template": "/sitecore/templates/Sample/JsonItem",
        "Fields": {
            "Title": {
                "Value": "Hello"
            },
            "Text": {
                "Value": "Hello World"
            },

            "Unversioned": {
                "da-DK": {
                    "UnversionedField": {
                        "Value": "Hello"
                    }
                }
            },

            "Versioned": {
                "da-DK": {
                    "1": {
                        "VersionedField": {
                            "Value": "Version 1"
                        }
                    },
                    "2": {
                        "VersionedField": "Version 2"
                    }
                }
            }
        }
    }
}
```

Yaml format (extension .item.yaml): 
```yaml
Item :
    Template : /sitecore/templates/Sample/YamlItemTemplate
    - Fields :
        - Field : Title
          Value : Hello
        - Field : Text
          Value : Hello World

        - Unversioned :
            - da-DK :
                - Field : UnversionedField
                  Value: >
                        Hello

        - Versioned :
            - da-DK :
                - 1 :
                    - Field : VersionedField
                      Value : Version 1
                - 2 :
                    - Field : VersionedField
                      Value : Version 2
```

Xml format (extension .item.xml) - please notice the namespace, which indicates the Xml schema to use.
```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item" Template="/sitecore/templates/Sample/XmlItemTemplate">

    <Fields>
        <Field Name="Title" Field.ShortHelp="Title field." Field.LongHelp="Title field.">Hello</Field>
        <Field Name="Text" Field.ShortHelp="Text field." Field.LongHelp="Text field.">Hello World</Field>

        <Unversioned Language="da-DK">
            <Field Name="UnversionedField" Field.ShortHelp="Title field." Field.LongHelp="Title field.">Hello</Field>
        </Unversioned>

        <Versioned Language="da-DK">
            <Version Number="1">
                <Field Name="VersionedField" Field.ShortHelp="Checkbox field." Field.LongHelp="Checkbox field.">Version 1</Field>
            </Version>
            <Version Number="2">
                <Field Name="VersionedField">Version 2</Field>
            </Version>
        </Versioned>
    </Fields>
</Item>
```

Content Xml format (extension .content.xml) - please notice that the element names specifies the template and fields are attributes. Spaces
in template or field names are replaced by a dot '.'. 
```xml
<Root Id="{11111111-1111-1111-1111-111111111111}" Database="master" Name="sitecore" ParentItemPath="/">
    <Main.Section Id="{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}" Name="content"/>

    <Main.Section Id="{EB2E4FFD-2761-4653-B052-26A64D385227}" Name="layout">
        <!-- /sitecore/layout/Layouts -->
        <Node Id="{75CC5CE4-8979-4008-9D3C-806477D57619}" Name="Layouts">
            <View.Rendering Id="{5E9D5374-E00A-4053-9127-EBC96A02C721}" Name="MvcLayout" Path="/layout/layouts/MvcLayout.cshtml" Place.Holders="Page.Body"/>
        </Node>

        <!-- /sitecore/layout/Devices -->
        <Node Id="{E18F4BC6-46A2-4842-898B-B6613733F06F}" Name="Devices">
            <Device Id="{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}" Name="Default" />
            <Device Id="{46D2F427-4CE5-4E1F-BA10-EF3636F43534}" Name="Print" />
            <Device Id="{207131FA-F6B2-4488-BCB3-3BF70100B9B8}" Name="App Center Placeholder" />
            <Device Id="{73966209-F1B6-43CA-853A-F1DB1C9A654B}" Name="Feed" />
        </Node>
    </Main.Section>

    <Main.Section Id="{3C1715FE-6A13-4FCF-845F-DE308BA9741D}" Name="templates">
        <!-- /sitecore/templates/Sample -->
        <Template.Folder Id="{73BAECEB-744D-4D4A-A7A5-7A935638643F}" Name="Sample">
            <Template Id="{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}" Name="Sample Item"/>
        </Template.Folder>

        <!-- /sitecore/templates/System -->
        <Template.Folder Id="{4BF98EF5-1D09-4DD1-9AFE-795F9829FD44}" Name="System">
            <Folder Id="{FB6B721E-D64D-4392-A1F0-A15194CBFAD9}" Name="Layout">
                <Folder Id="{531BF4A2-C3B2-4EB9-89D0-FA30C82AB33B}" Name="Renderings">
                    <Template Id="{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}" Name="View Rendering"/>
                </Folder>
            </Folder>
        </Template.Folder>
    </Main.Section>
</Root>
```

You will notice that the examples above do not specify the name of the item. By default the name of the file (without extensions) is used
as item name.

Likewise the directory path is used as item path. The root of the project corresponds to /sitecore, so having the item file
"[Project]\content\Home\HelloWorld.item.xml" will create the item "/sitecore/content/Home/HelloWorld".

### Nested items
An item file can contain multiple nested items. Below is an example:

```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item" Template="/sitecore/templates/Sample/Sample Item">
  <Fields>
    <Field Name="Title" Value="Hello" />
  </Fields>

  <Item Name="Hi" Template="/sitecore/templates/Sample/Sample Item">
    <Fields>
      <Field Name="Title" Value="Hi" />
    </Fields>
  </Item>

  <Item Name="Hey" Template="/sitecore/templates/Sample/Sample Item">
    <Fields>
      <Field Name="Title" Value="Hey" />
    </Fields>
  </Item>
</Item>
```
This create an item with two children; Hi and Hey:

* HelloWorld
  * Hi
  * Hey


### Templates
Template can be defined in items files using a special schema. Below is an example:

```xml
<Template xmlns="http://www.sitecore.net/pathfinder/item">
    <Section Name="Data">
        <Field Name="Title" Type="Single-Line Text"/>
        <Field Name="Text" Type="Rich Text"/>
        <Field Name="Always Render" Type="Checkbox" Sharing="Shared"/>
    </Section>
</Template>
```

Templates can be nested in the same way that multiple items can be nested inside an item file.

#### Inferred templates
If you have a template that is used by a single item, you can have Pathfinder automatically create the template from the fields in the
item - Pathfinder will infer the template fields from the fields you specify in the item.

To infer and create the template add the "Template.CreateFromFields='true'" attribute.

```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item" Template.Create="/sitecore/templates/Sample/InferredTemplate">
    <Fields>
        <Field Name="Text" Value="Hello" Field.Type="Rich Text" />
    </Fields>
</Item>
```
The example above creates the template "InferredTemplate" with a single template field "Text". The type of the field is "Rich Text".

### Directives 
Item files (Json, Xml and Yaml) may contain directives that affect the way the file is parsed. 

Supported directives:

Directive        | Description
-----------------|-------------
File.Include     | Includes another file (specified by the Key attribute)
File.Placeholder | In an included file, specifies location where the inner content of the File.Include directive is injected.


#### File.Include
Item files can include other files. This allows parts of items files to be shared among multiple items. 

Below is how to include other files in an item file.

Json:
```js
{
    "Item": {
        "Template": "/sitecore/templates/Sample/JsonItem",
        "Fields": {
            "File.Include": [
                {
                    "File": "~/includes/Field.include.item.json" 
                },
                {
                    "File": "~/includes/ParameterizedField.include.item.json",
                    "Name": "ParameterizedField",
                    "Value": "Parameterized Value"
                }
            ]
        }
    }
}
```

Yaml:
```yaml
Item :
    - Fields :
        - File.Include : ~/includes/Field.include.item.yaml
        
        - File.Include : ~/includes/ParameterizedField.include.item.yaml
          Name  : ParameterizedField
          Value : Parameterized Value

```

Xml:
```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item">
    <Fields>
        <File.Include File="~/includes/Field.include.item.xml" />
        <File.Include File="~/includes/ParameterizedField.include.item.xml" Name="ParameterizedField" Value="Parameterized Value"/>
    </Fields>
</Item>
```

The first included file looks like this:

```js
{
    "IncludeField": {
        "Value": "Included field."
    }
}
```

Include files are not simple text subsitutions, but are resolved at the lexing level of the compiler (before parsing). The File.Include directive 
is also part of the item schemas, which means that include files cannot be included at arbitrary positions. This is to ensure 
Syntax Highlighting, Validation and IntelliSense still work.

Include files can be parameterized as can be seen in the second include file above. Parameters are simple text substitions. Parameter 
tokens are prefxied with '$' in the include file. Below is the second include file from the example above.

```xml
<Field Name="$Name" Field.ShortHelp="Include field." Field.LongHelp="Include field.">$Value</Field>
```

##### Predefined parameters.

Parameter Name             | Description 
-------------------------- | ------------
$ItemPath                  | The path of the item
$FilePathWithoutExtensions | The file path without extensions
$FilePath                  | The file path including extensions
$Database                  | Database name
$FileNameWithoutExtensions | The file name without extensions
$FileName                  | The file name including extensions
$DirectoryName             | The current directory name

Please notice: Include files do not work everywhere yet.

#### File.Placeholder
A File.Include directive may contain additional content to be injected into the included file (if you are a Sitecore veteran, think Xml Controls and Xaml#).
The injected content is placed inside a Placeholder element. An include file can specify multiple placeholders - each identified by a key. The default key
is empty (or blank).

Below is an example of an include directive with inner content. The content is specified under the Placeholders element and the "" specifies the empty key.
In the example the content is a field named "PlaceholderText" with the value "Placeholder text.".

```js
"File.Include": {
    "File": "~/includes/PlaceholderItem.include.item.json",
    "ItemName": "PlaceholderItem",
    "Placeholders": {
        "": {
            "PlaceholderText": {
                "Value": "Placeholder text.",
                "Field.ShortHelp": "Placeholder Text.",
                "Field.LongHelp": "Placeholder Text."
            }
        }
    }
}
```

The include file looks like this:
```js
{
    "$schema": "../../../../sitecore.pathfinder.console/files/project/sitecore.project/schemas/item.schema.json#",
    "Item": {
        "Name": "$ItemName",
        "Template": "/sitecore/templates/Sample/JsonPlaceholderItem",
        "Template.CreateFromFields": true,
        "Template.ShortHelp": "Short help.",
        "Template.LongHelp": "Long help.",
        "Template.Icon": "Application/16x16/About.png",

        "Fields": {
            "File.Placeholder": { }
        }
    }
}
```

The element "File.Placeholder" specifies that the Placeholder with the empty key should be inserted here.

### Item IDs
Normally you do not need to specify the ID of an item, but in some cases, it may be necessary. Pathfinder supports soft IDs meaning that the
item ID does not have to be a Guid (but it can be).

By default Pathfinder calculates the ID of an item hashing the project unique ID and the file path (without file extensions) like this 
`item.Guid = MD5((Project_Unique_ID + item.FilePath).ToUpperInvariant())`. This means that the item ID is always the same, as long as the 
file path remains the same.

You can explicitly set the ID by specifying the ID in item file as either a Guid or as a soft ID.

* If no ID is specified, the item ID is calculated as `item.Guid = MD5((Project_Unique_ID + item.FilePath).ToUpperInvariant())`.
* If the ID is specified as a Guid, the item ID uses Guid as is.
* If the ID is specified and starts with '{' and ends with '}' (a soft ID), the item ID is calculated as `item.Guid = MD5(item.ID)`.
* If the ID is specified (but not a Guid), the item ID is calculated as `item.Guid = MD5((Project_Unique_ID + item.ID).ToUpperInvariant())`.

If you rename an item, but want to keep the original item ID, specify the ID as the original file path (without extensions), e.g.:
```xml
<Item xmlns="http://www.sitecore.net/pathfinder/item" 
    Id="/sitecore/content/Home/HelloWorld" 
    Template="/sitecore/templates/Sample/Sample Item">
    <Fields>
        <Field Name="Title">Pathfinder</Field>
        <Field Name="Text">Welcome to Pathfinder</Field>
    </Fields>
</Item>
```

### Content item format
Content item files also contain items, but the schema is different. When you synchronize the website, Pathfinder generates and downloads a
schema, that contains all templates in the database (master and core database). If you change a template, you will have to synchronize the
website again.

The schema of content files ensures, that you can only specify fields that appear in the template, and provide some nice Intellisense in many
code editors. The format of content item files is also more compact than other types of item files.

So the advantages of content item files are validation against the template and a more compact format, but you have to synchronize the 
website, if you change a template.

```xml
<Root Database="master" Name="sitecore" ParentItemPath="/">
    <Main.Section Name="layout">
        <Node Name="Layouts">
            <View.Rendering Name="MvcLayout" Path="/layout/layouts/MvcLayout.cshtml" Place.Holders="Page.Body"/>
        </Node>
    </Main.Section>
</Root>
```

If the item contains characters, that are not allowed in Xml item names (including spaces), the characters are replaced by a dot (.).

### Media files
If you drop a media file (.gif, .png, .jpg, .bmp, .pdf, .docx) into your project folder, Pathfinder will upload the file to the Media Library.
The Sitecore item will be implicit created. 

### Layouts and renderings
Layout and rendering files (.aspx, .ascx, .cshtml, .html) are copied to the website directory and the Sitecore items are automatically created.
You no longer have to explicitly create and configure a Sitecore Rendering or Layout item. The relevate fields (including the Path field) are
populated automatically.

### Json layout format
To specify a layout in Json, use the format below.

```js
{
    "Item": {
        "Layout": {
            "Devices": [
                {
                    "Name": "Default",
                    "Layout": "/sitecore/layout/Layouts/MvcLayout",
                    "Renderings": [
                        {
                            "HelloWorld": { "Text": "Welcome" }
                        },
                        {
                            "BodyText": { }
                        },
                        {
                            "Footer": { }
                        }
                    ]
                }
            ]
        }
    }
}
```

### Populating additional fields for implicitly created items
Supposed you have an MVC View rendering HelloWorld.cshtml and want to set the Parameters field, simply create a HelloWorld.item.json (or 
HelloWorld.item.xml or HelloWorld.item.yaml) next to the HelloWorld.cshtml file.

* HelloWorld.cshtml
* HelloWorld.item.json

When determining the item name, Pathfinder uses the field up until the first dot - in this case "HelloWorld". When two or more files have the
same item name (and item path), they are merged into a single item. Pathfinder will report an error if a field is set more than once with different
values.

### Supported file formats

Extension            | Description 
-------------------- | ------------
.item.json           | Item in Json format
.item.yaml           | Item in Yaml format
.item.xml            | Item in Xml format
.master.content.yaml | Item in Yaml content format (master database)
.core.content.yaml   | Item in Yaml content format (core database)
.master.content.xml  | Item in Xml content format (master database)
.core.content.xml    | Item in Xml content format (core database)
.master.layout.json  | Layout definition in Json format (master database)
.core.layout.json    | Layout definition in Json format (core database)
.master.layout.yaml  | Layout definition in Yaml format (master database)
.core.layout.yaml    | Layout definition in Yaml format (core database)
.master.layout.xml   | Layout definition in Xml format (master database)
.core.layout.xml     | Layout definition in Xml format (core database)
.dll                 | Binary file - copied to /bin folder
.aspx                | Layout file
.ascx                | SubLayout
.cshtml              | MVC View Rendering
.html                | Html file (MVC View Rendering) with Mustache syntax support
.htm                 | Html file
.js                  | JavaScript content file
.css                 | Stylesheet content file
.config              | Config content file

You can control which types of files are included in the deployment package by override in the build-package/ignore-directories and 
build-package/ignore-filenames settings from the global scconfig.json (located in the /sitecore.tools/scconfig.json file).

### File system mapping
The filesystem structure of the project does not necessary corresponds to the desired structure on the website.

In the scconfig.json file, you can map files and items to different locations on the website.

```js
"files": {
    "html": {
        "project-directory": "",
        "include": "/*.html",
        "website-directory": "/MyProject/layout/renderings",
        "item-path": "/sitecore/layout/renderings/MyProject",
        "database": "master"
    },
    "img": {
        "project-directory": "/img",
        "include": "**",
        "website-directory": "",
        "item-path": "/sitecore/media library/MyProject"
    }
}
```

## Synchronizing project and website
In Pathfinder the project contains the whole truth. However a project may need to use items, template, renderings from a standard 
Sitecore website. A good example is a SPEAK based module.

These external references can be imported into the project by using the ``sync-website`` task. This task makes a request to the website
to collect the needed information. The information is downloaded as a zip file and unpacked in the project directory.

The sync-website task is configured on the 'sync-website' section in the scconfig.json configuration file.

```js
"sync": {
    "Json schema for layouts in Master database": {
        "file": "sitecore.project/schemas/master.layout.schema.json",
        "database": "master"
    },
    "Json schema for layouts in Core database": {
        "file": "sitecore.project/schemas/core.layout.schema.json",
        "database": "core"
    },
    "Xml schema for layouts in Master database": {
        "file": "sitecore.project/schemas/master.layout.xsd",
        "database": "master",
        "namespace": "http://www.sitecore.net/pathfinder/layouts/master"
    },
    "Xml schema for layouts in Core database": {
        "file": "sitecore.project/schemas/core.layout.xsd",
        "database": "core",
        "namespace": "http://www.sitecore.net/pathfinder/layouts/core"
    },
    "Xml schema for content in Master database": {
        "file": "sitecore.project/schemas/master.content.xsd",
        "database": "master",
        "namespace": "http://www.sitecore.net/pathfinder/content/master"
    },
    "Xml schema for content in Core database": {
        "file": "sitecore.project/schemas/core.content.xsd",
        "database": "core",
        "namespace": "http://www.sitecore.net/pathfinder/content/core"
    }
}
```

By default various schema files for Json and Xml are generated and downloaded. The ``file`` property determines where the generated
is unpack in the project directory.

It is possible to add additional resources to the list. To add SPEAK items as external references add the following lines.

```js
"External references for Speak": {
    "file": "sitecore.project/external/speak.core.content.xml",
    "database": "core",
    "path": "/sitecore/client/Speak"
},
"External references for Business Component Library": {
    "file": "sitecore.project/external/bcl.core.content.xml",
    "database": "core",
    "path": "/sitecore/client/Business Component Library"
}
```

## Deploying
By default Pathfinder copies the build package to a website and installs it. The package is copied to the 
[DataFolder]/Data/Pathfinder/Available directory. Any dependencies from the sitecore.project/packages directory are also copied to
this directory.

### Installation
The package is installed by making a request to the /sitecore/shell/client/Applications/Pathfinder/InstallPackage with the name of the 
package on the query string. This webpage uses Nuget to unpack the files to the [DataFolder]/Pathfinder/Installed directory. Once the files 
are available, Pathfinder rebuilds the project and emits items and files to the website.

Any dependency packages are unpacked before the package and are processed in the same way.

### Dependency packages
A project can depend on other Nuget packages using the standard Nuget dependency mechanism. Dependency packages are located in the
/sitecore.project/packages directory. As part of deploying these packages are copied to the website and installed.

To add a new dependency package, copy the file (.nupkg) to the sitecore.project/packages directory. In the Nuspec file sitecore.project/sitecore.nuspec
add the filename to the ``dependencies`` tag like this (see [Nuspec Reference](https://docs.nuget.org/create/nuspec-reference)):

```xml
<dependencies>
    <dependency id="SitecorePathfinderCore" version="1.0.0" />
    <dependency id="SitecorePowerShellExtensions32ForSitecore8" version="1.0.0" />
</dependencies>
```

The SitecorePowerShellExtensions32ForSitecore8.nupkg will be copied to the [DataFolder]/Pathfinder/Available directory.

Standard Sitecore Packages cannot be used directly as dependencies since Nuget does not understand Sitecore packages. Instead you have to wrap
a Sitecore Package in a Nuget package. There are different way to do this. 

First of all you can convert the Sitecore Package to a Nuget package using a community tool like this

* [CreateSitecoreNugetPackage](http://hermanussen.eu/sitecore/wordpress/2013/05/turn----any----sitecore----package----into----a----nuget----package/) by Robin Hermanussen

Alternatively Pathfinder contains the 'pack-dependencies' task that simply converts all *.zip files in the /sitecore.project/packages directory 
to Nuget packages. For each zip file it creates a Nuget package where the zip files is located in the content/packages directory in the .nupkg file. 
Pathfinder understands, that any zip files in the content/packages directory is a Sitecore Package and installs it.

Finally you can create the Nuget package manually by creating a Nuspec file like this:

```xml
<package xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\">
    <metadata>
        <id>MyPackage</id>
        <title>My Package</title>
        <version>1.0.0</version>
        <authors>Me</authors>
        <requireLicenseAcceptance>false</requireLicenseAcceptance>
        <description>My package</description>
    </metadata>
    <files>
        <file src="mypackage.zip" target="content\packages\mypackage.zip"/>
    </files>
</package>
```

### Bypassing package creation
The `install-project` installs the project directly from the project directory. This skips the package creation and package copying (both
project packages and dependency packages). This saves a bit of time.

### Watching a project
The `watch-project` watches a project for changes and installs the project when a changes occurs. 

## Extensions
Pathfinder includes the Roslyn compiler to compile extensions on the fly. Extensions are C# files that are compiled and loaded dynamically through 
[MEF](https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx). This allows you to extend Pathfinder with new tasks, checkers, code 
generation handler and much more. 

When Pathfinder starts it looks through the /sitecore.tools/files/extensions directory to find any extension files, and if any file is newer than the
Sitecore.Pathfinder.Extensions.dll assembly, it recompiles the files and saves the output as Sitecore.Pathfinder.Extensions.dll.

For instance to make a new checker, duplicate a file in /sitecore.tools/files/extensions/checkers and start Pathfinder. Pathfinder will detect the
new file and recompile the assembly.

## Unit testing
[Watch the video](https://www.youtube.com/watch?v=DWU6D7L8ykg)

Unit testing in Sitecore can be tricky for a number of reasons. One reason is that sometimes you want your 
unit test to be executed within the Sitecore web context. Unless you have advanced mocking capabilities, this
requires you to make a request to a Sitecore website and run the tests.

Pathfinder installs a Web Test Runner in your Sitecore website. When you run the `run-unittests` task, Pathfinder
copies the unit test C# files to the server, compiles them and runs the tests.

This makes it easier to write server-side unit tests in you project and execute the in a Sitecore web context.

## Website validation
Pathfinder integrates with the Sitecore Rocks SitecoreCop feature. SitecoreCop examines the website and can identify
over 70 different types of issues. To validate the website, run the task `validate-website`.

![SitecoreCop validations 1](docs/img/SitecoreCop2.png)
![SitecoreCop validations 2](docs/img/SitecoreCop3.png)

## Layout Engines

### Sitecore rendering engine
Pathfinder supports the Sitecore Rendering Engine by supporting a special format for the __Rendering field. 
The format is similar to Html and Xaml, and is parsed when the package is installed into Xml format, that 
Sitecore expects. 

Here is an example of the format in Json.
```js
{
    "Layout": {
        "Devices": [
            {
                "Name": "Default",
                "Layout": "/sitecore/layout/layouts/MvcLayout",
                "Renderings": [
                    {
                      "HelloWorld": {
                      } 
                    }
                ]
            }
        ]
    }
}
```

### Html Templates
[Watch the video](https://www.youtube.com/watch?v=9aTGhW6ErYM)

Pathfinder also supports Html Templating which is simpler way of working with layouts. It resembles working with Mustache
Html Templates in JavaScript. However the Html Templates are resolved on the server and adapted to the Sitecore 
rendering engine.

When using Html Template, you do not have to specify a layout definition or use MVC views. Html Templates are not as 
powerful as the full Sitecore Rendering Engine, placeholders or using MVC views.

On an item, you specify the file name of the Html Template, you want to use, in the "Layout.HtmlFile" property (please notice 
that this property has been renamed from the video where it was called "HtmlTemplate").
```js
{
  "Item": {
    "Layout.HtmlFile": "/layout/renderings/HtmlTemplate.html",
  }
}
```

The Html Template is a Html file that also supports Mustache like tags.

```html
<h1>Fields</h1>
<p>
    {{Title}}
</p>
<p>
    {{Text}}
</p>
{{> Footer.html}}
```

The `{{Title}}` tags will be replace with the Title field in the current context item.

Please notice that Mustache lists and partials are supported (see the Footer tag in the last line). Pathfinder extends the 
Mustache syntax to support Sitecore placeholders - the tag `{{% placeholder-name}}` will be rendered as a Sitecore
placeholder.

Pathfinder creates .html as View renderings and these renderings can used as any other Sitecore rendering.

## Code Generation
*Please notice that the video is out-of-date. Code Generation now uses extensions and not Razor files.*

[Watch the video](http://youtu.be/ZM3ve1WhwwQ)

Pathfinder can generate code based on your project. The most obvious thing is to generate a C# class for each template in
the project.

To generate code, execute the task `generate-code`. This wil iterate through the elements in the project and check if
a code generator is available for that item. If so, the code generator is executed.

Code generators are simply extensions that are located in the /sitecore.tools/extensions/codegen directory.

Normally you want to run the `generate-code` task before building an assembly, so the C# source files are up-to-date.

# Starter kits
Pathfinder comes with 2 starter kits to get you started easily. Starter kits are located in the sitecore.tools/files/starterkits
directory. 

To install a starter kit, simple unpack the Nuget package in the root of your project.

Starter Kit | Description
------------|------------
Clean Blog  | A simple blogging website.
TodoMVC     | A SPEAK based Todo list.

### CleanBlog
Clean Blog is a read-only blog website based on the [Start Bootstrap](http://startbootstrap.com/) - 
[Clean Blog](http://startbootstrap.com/template-overviews/clean-blog/) template. It is very basic and uses Html Template files
instead of Sitecore Renderings.

### TodoMVC
The TodoMvc starter kit is a SPEAK based application that implements a Todo application similar to [TodoMVC](http://todomvc.com/).
It uses Sitecore renderings and shows how to implement a new SPEAK rendering (TodoMvcList). It also uses an ASP.NET handler to
create, update and delete todo items.

# Package User interface
Pathfinder installs a UI for managing packages on the website. While the Package Manager can uninstall Nuget packages, it
does not (yet) remove files are items from the website - only the installed Nuget package is removed.

The Package Manager is located at the Url: /sitecore/shell/client/Applications/Pathfinder/Packages

![Package Manager](docs/img/PackageManager1.png)

![Package Manager](docs/img/PackageManager2.png)


# Environment

## Notepad
Everything in Pathfinder is a file, so you can use Notepad to edit any file.

To build the project, simply run the ``scc`` file from the command-line.

## Atom 

[Atom](https://atom.io/) is a good code editor with lots of plugins. You need to install a build package to be
able to run the Pathfinder build pipeline, e.g. [Build](https://github.com/noseglid/atom-build) by nosegild.

After creating an Atom project, the default build task has been configured to execute the build pipeline in Pathfinder. 
In Atom the build task can be executed by pressing Ctrl+Alt+B.

To create a Atom project, run this command ``scc init-atom``. This will create a .atom-build.json file
that contains default configuration for Pathfinder.

## Sublime Text 3
To run Pathfinder as a Build System in Sublime Text 3, configure it like this:

```js
{
	"shell_cmd": "scc.exe",
    "working_dir": "${project_path:${folder}}"
}
```

## Visual Studio Code

[Visual Studio Code](https://code.visualstudio.com/) is a nice code editor. After creating a VS Code project,
the default build task has been configured to execute the build pipeline in Pathfinder. In Code the build task 
can be executed by pressing Ctrl+Shift+B.

To create a VS Code project, run this command ``scc init-vscode``. This will create a .vscode directory
that contains default configuration for Pathfinder.

## Visual Studio

To create a Visual Studio project, run this command ``scc init-visualstudio`` after having initialized the project. This will create a .csproj file and some additional files to 
support Visual Studio and Grunt. Afterwards make sure the run the installgrunt.cmd to install GruntJS. 

To manually create a Visual Studio project:

1. Create a web project in Visual Studio
1. Add a reference to Sitecore.Kernel
1. Install the Pathfinder Nuget package
1. Install GruntJS. Run installgrunt.cmd or
   1. Install GruntJS in the project: npm install grunt --save-dev
   1. Install grunt-shell: npm install --save-dev grunt-shell
1. Right-click gruntfile.js and select Task Runner Explorer
1. Add the following lines to gruntfile.js

```js
module.exports = function (grunt) {
    grunt.initConfig({
        shell: {
            "build-project": {
                command: "scc.exe"
            },
            "run-unittests": {
                command: "scc.exe run-unittests"
            },
            "generate-unittests": {
                command: "scc.exe generate-unittests"
            },
            "generate-code": {
                command: "scc.exe generate-code"
            },
            "sync-website": {
                command: "scc.exe sync-website"
            },
            "validate-website": {
                command: "scc.exe validate-website"
            }
        }
    });

    grunt.registerTask("build-project", ["shell:build-project"]);
    grunt.registerTask("run-unittests", ["shell:run-unittests"]);
    grunt.registerTask("generate-unittests", ["shell:generate-unittests"]);
    grunt.registerTask("generate-code", ["shell:generate-code"]);
    grunt.registerTask("sync-website", ["shell:sync-website"]);
    grunt.registerTask("validate-website", ["shell:validate-website"]);

    grunt.loadNpmTasks("grunt-shell");
};
```

10. In Task Runner Explorer, right-click 'build-project' and select Bindings | After Build. This will run Pathfinder after each build.
10. If you want to use code generate, right-click 'generate-code' and select Binding | Before Build.

# External tools
There are many good 3. party tools for Sitecore. These are some of my personal thoughts on how to use/integrate these tools with Pathfinder. Please notice
that these thoughts do not reflect the official Sitecore opinions on the various tools. I do apologies in advance for misunderstanding or not knowing 
any of the tools in depth.

Many tools are available as either Sitecore Packages or Nuget packages.

Sitecore Packages can be installed by downloading the package to /sitecore.tools/packages, wrapping it in a Nuget
package (using the ``pack-dependencies`` task) and making it a dependency. 

Nuget Packages can be installed by downloading the package to /sitecore.tools/packages,  and making it a dependency. 

### TDS
[TDS](https://www.hhogdev.com/Downloads/Team-Development-for-Sitecore.aspx) is a commercial Visual Studio plugin that primarily implements 
good serialization/deserialization functionality in Sitecore. Over time TDS has grown and support many additional features like best practices 
and code generation. TDS is battle proven and robust.

There is some overlap in functionality between Pathfinder and TDS. It could be interesting to have TDS support the Pathfinder compiler, but that is 
probably unlikely to happen.

### Razl
[Razl](https://www.hhogdev.com/products/razl.aspx) is another tool from Hedgehog that allows you to compare the items in two Sitecore websites.
Razl is very useful.

There is no overlap between Razl and Pathfinder and I see no obvious integration.

### Visual Studio Online
[Visual Studio Online](https://www.visualstudio.com/en-us/products/what-is-visual-studio-online-vs.aspx): Services for teams to share code, 
track work, and ship software - for any language, all in a single package. It's the perfect complement to your IDE.

It is a goal that Pathfinder integrates with Visual Studio Online. Pathfinder fits in the Build part of Visual Studio Online and it is pretty easy to 
setup.

![Pathfinder](docs/img/VisualStudioOnline1.png)

1. Add a Batch Script build task and set the Path to "scc.exe".
2. *Optional:* Add a Visual Studio Build build task and set the Solution to **/*.sln.
3. *Optional:* Add a Visual Studio Test build task and set the Test Assembly to your output assembly.

This will execute Pathfinder whenever Visual Studio Online builds your project.

If Visual Studio Online does not have access to your website, you should probably remove the deployment tasks from the build-project/tasks settings.

### SQL LocalDb
Sitecore requires a SQL Server and it can be a hassle to setup. Consider using LocalDB instead.

See [http://sitecoresupport.blogspot.dk/2012/03/sitecore-on-sql-2012-denali-with.html](http://sitecoresupport.blogspot.dk/2012/03/sitecore-on-sql-2012-denali-with.html)

1. Install SQL Server LocalDB using Web Platform Installer.
1. Modify /App_Config/ConnectionStrings.xml to use "Server=(localdb)\v11.0; Integrated Security=true; AttachDbFileName=E:\db\Sitecore.Core.MDF" ...
1. Change the AppPool Identity of the IIS Website to "LocalSystem"
1. Start SQL LocalDB service using "sqllocaldb.exe start v11.0"

### IIS Express
Sitecore requires IIS to run. Consider using IIS Express.

See [http://chrismcleod.me/2011/01/14/iis-express-website-here-shell-extension/](http://chrismcleod.me/2011/01/14/iis-express-website-here-shell-extension/)

1. Install IIS Express Website Here

### Sitecore Powershell Extensions
[Powershell Extensions](https://marketplace.sitecore.net/en/Modules/Sitecore_PowerShell_console.aspx) adds Powershell capabilities to Sitecore. 
You can open a Powershell window in the Sitecore desktop and execute Powershell scripts. Powershell Extensions are very well documented and very
popular.

Powershell Extensions is a Sitecore package and can be installed as such. 

It could be interesting to have Pathfinder be able to execute Powershell scripts on the website. Pathfinder could contain a Powershell task that
sends a Powershell script to Powershell Extensions which executes it and returns the result. The question is if developer would prefer this approach
over opening a Powershell Extensions window in the Sitecore desktop.

### Glass.Mapper
[Glass.Mapper](http://www.glass.lu/) is the only ORM you will need to improve your Sitecore and Umbraco development. It only takes a few minutes to 
setup and then you are ready to start using classes and interfaces in your project.

Glass.Mapper is popular and battle proven.

Pathfinder could work with Glass.Mapper by having Pathfinder generate classes for Glass.Mapper using the 'generate-code' task. 

### Zen Garden
[Zen Garden](https://www.cognifide.com/accelerators/zen-garden-sitecore) accelerates deployment of responsive websites quickly and cost effectively, 
minimising reliance on technical skills while improving maintenance of governance standards. 

Zen Garden is a full fletched framework and I don't see any obvious integrations with Pathfinder.

### BrainJocks SCORE
[BrainJocks SCORE](http://www.brainjocks.com/company/score) is a comprehensive development framework that will transform your Sitecore experience.

SCORE seems like a full fletched framework and I don't see any obvious integrations with Pathfinder.

### Sitecore Rocks
[Sitecore Rocks](https://github.com/JakobChristensen/Sitecore.Rocks.Docs) is both a Visual Studio plugin (Sitecore Rocks Visual Studio) and 
a stand-alone Windows application (Sitecore Rocks Windows), which is installed using Click-Once. Sitecore Rocks is both a development and an 
administration tool. It is big and somewhat bloated, but popular. Sitecore developer training uses Sitecore Rocks.

Sitecore Rocks allows you to edit items directly on the website which Pathfinder discourages.

The Sitecore Rocks and Pathfinder can probably be integrated in numereous ways. Sitecore Rocks does not implement a deployment mechanism and 
serialization support is very basic. Sitecore Rocks could contain a Visual Studio project template for Pathfinder.

### Sitecore Habitat
[Sitecore Habitat](https://github.com/Sitecore/Habitat) is a solution framework focusing on three aspects:

* Simplicity - A consistent and discoverable architecture
* Flexibility - Change and add quickly and without worry
* Extensibility - Simply add new features without steep learning curve

Sitecore Habitat could be a fantastic solution on top of Pathfinder. It describes how to architect a good Sitecore website which is a goal
of Pathfinder. 

It would be great if Pathfinder could build a Habitat-based website out of the box. The developer would get a tool chain and a website 
framework as a starting point.

Pathfinder and Habitat must be integrated.

### FakeDB
[FakeDb](https://github.com/sergeyshushlyapin/Sitecore.FakeDb) is the unit testing framework for Sitecore that enables creation and manipulation 
of Sitecore content in memory. It is designed to minimize efforts for the test content initialization keeping focus on the minimal test 
data rather than comprehensive content tree representation.  

FakeDB is available as a Nuget package and can be installed as such. 

FakeDB and Pathfinder could be integrated in a number of ways. Pathfinder could generate code that exposes the items in the project to FakeDB, so
developers do not have to do this manually. Also if the items change, it is easy to update the FakeDB code. 

It would be really nice if Pathfinder could execute tests against FakeDB. It is not clear to me, if FakeDB requires a HttpContext - if not, this
could be a good way to execute tests locally.

### JsonDataProvider
[JsonDataProvider](https://github.com/Sitecore/Sitecore.JsonDataProvider) allows storing parts of content tree in separate *.json files.
Unlike serialization, items exist only in *.json files which makes synchronization unnecessary.
Single *.json file stores all descendant items of the specific item that exists in a SQL database.

The JsonDataProvider could be very valuable for Pathfinder if it could handle any item changes by the Pathfinder installer. That way the items in 
original website are not touched and the website remains clean. To remove any installed items, simply remove the JsonDataProvider Json file.

This is one to keep an eye on.

### Unicorn
[Unicorn](https://github.com/kamsar/Unicorn) is a utility for Sitecore that solves the issue of moving templates, renderings, and other database 
items between Sitecore instances. The default Sitecore data provider is extended to automatically serialize item changes as they are made to 
Sitecore. This means that at any given time, what's serialized is the "master copy."  

Unicorn removes the manual process of serializing items which solves a lot of issues and it can be used for CI.

Unicorn can be installed as a Nuget package.

Unicorn could be interesting for Pathfinder if it can update files in the project when the website is changed. Suppose items are create on the website
in a custom manner. Unicorn could feed these items back to the project.

Unicorn also supports the [Rainbow](https://github.com/kamsar/Rainbow) project which changes the serialization format to Yaml which Pathfinder supports. 
(Rainbow was the inspiration for supporting Yaml in Pathfinder).

### Sitecore Courier
[Sitecore Courier](https://github.com/adoprog/Sitecore-Courier) lets you build Sitecore Update packages automatically, by analyzing serialized 
Sitecore items and packaging only changed items. Packages will be generated automatically, by comparing serialization from TAG (source) to the 
TRUNK (target). Incremental package will contain only changed items.

This is a cool idea. It can build update packages by comparing item in a source folder with a tag in source control. This should scale well
for website with lots of content.

Pathfinder works a bit differently by defining that the project must contain all assets used by the project. However Sitecore Courier could be 
use to create an update package for a new version of a project. 

Pathfinder could also use Sitecore Courier internally to deploy the package instead of using Nuget. It might be faster.

### Sitecore Ship
[Sitecore Ship](https://github.com/kevinobee/Sitecore.Ship) is a lightweight means to install Sitecore Update packages via HTTP requests.

There is a big overlap between Sitecore Ship and Pathfinder, but Sitecore Ship does one thing and probably does it very well. Since it is developed
by Kevin Obee, it is probably very secure.

Pathfinder could use Sitecore Ship internally to deploy update packages. It might be faster.

# Roadmap

Short term

* Exclude specified images from uploading
* Use Sitecore renderings in Html Templates
* Fix bugs and do enhancements

Medium term

* More starter kits
* More documentation
* Support for Sitecore Habitat
* More support for Visual Studio Online

Long term

* Make documents (Json, Xml, Yaml) editable, so that the project can be refactored (e.g. the Rename task).
* Uninstall package (possible use Sitecore Rocks Anti-Packages).
* Some sort of UI for editing item files in a graphical manner. Should probably be webpages on the website.
* JsonDataProvider document - support JsonDataProvider files for editing
* Continuous Integration/Delivery - how?
* Live Update - update browser when project is built
* Consider moving tasks to NPM

# Thanks
See [Thanks](THANKS.md).
