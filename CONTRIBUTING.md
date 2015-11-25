# How to contribute
Pathfinder is envisioned to be an open source project with many contributors.

So feel free to clone the repo and submit pull requests.

## General feedback and discussions?
Please start a discussion in the [issues](https://github.com/JakobChristensen/Sitecore.Pathfinder/issues) section.

## Bugs and feature requests?
Also please start a discussion in the [issues](https://github.com/JakobChristensen/Sitecore.Pathfinder/issues) section.

## Getting started
Clone the project from [GitHub](https://github.com/JakobChristensen/Sitecore.Pathfinder), and copy the following assemblies 
to the /lib/Sitecore directory.

* Sitecore.ContentSearch.dll
* Sitecore.ContentSearch.Linq.dll
* Sitecore.Kernel.dll
* Sitecore.Mvc.dll
* Sitecore.Zip.dll 

The Sitecore.Pathfinder.Console project is the default project and you should set it as StartUp Project. 

# Architecture

## Project overview
Pathfinder consists of two parts - a part that runs on a development machine and a part that runs inside a Sitecore website.
The development (client) part is responsible for building the deployment package and the Sitecore (server) part is responsible 
for installing the package. Both parts share the Pathfinder compiler which loads the projects and compiles it.

### Client  
The client is the command line tool. It provides a number of tasks that can be executed, like building the project or 
initializing the development folder. Some tasks communicate with the server. 

### Server 
The server responds to any requests that the client might make. It has access to the Sitecore API. It's primary task is to
install deployment packages.

## Pathfinder compiler
The Pathfinder compiler loads and compiles a project from a list of source files while collecting diagnostics. 

Internally the compiler goes through a number of steps. 

* Initializing
* Parsing
* Compiling
* Checking
* Emitting


#### Initializing
During initialization the compiler loads external references located in the /sitecore.project/external directory. The external
references are needed because the project must hold the whole truth. 

#### Parsing
The compiler accepts a list of source files to be compiled. For each file the compiler determines the appropriate loader
and parser. The loader loads the file (if need be) and passes it to parsing which creates in-memory objects that represent
the contents of the file. When all files have been loaded the project contains a in-memory model of all the source files. 

#### Compiling
The model is then passed to compiling which transforms the model into objects that are appropriate for Sitecore, for
instance for each media file, an appropriate media item model is created. 

When all files have been processed, the compiler processes the item models and for each field, compiles it. This typically 
transforms the field from a user inputed value to a value that is appropriate for Sitecore, for instance Link fields are 
usually inputted as "/sitecore/content/home" and the compiled value is 
'&lt;link ... id="{98E48AE6-9997-4BFA-AFAB-862B3E6EC486}" ... />'. 

Once the fields have been compiled, the compiler processes the item model and for each field, looks for references to other
project items. A references is typically a Guid or a value that starts with "/sitecore".

#### Checking
The compiler collects diagnostics throughout the entire parsing and compiling processes. The checking step validates the
entire project. The executes the dynamically compiled checkers and checks that all references are valid.

Checks:

* The project item has a unique Guid
* Reference not found
* The size of media file does not exceed 5MB.
* Item name does not contain spaces.
* Item must have a template.
* Item field is not defined in the template.
* Empty templates should be avoided. Consider using the Folder template instead.
* Template should have a short help text.
* Template short help text should end with '.'
* Template short help text should end with a capital letter.
* Template should should have a long help text.
* Template long help text should end with '.'.
* Template long help text should end with a capital letter.
* Template should should have an icon.
* Template field should have a short help text.
* Template field short help text should end with '.'.
* Template field short help text should end with a capital letter.
* Template field should should have a long help text.
* Template field long help text should end with '.'.
* Template field long help text should end with a capital letter.
* Template section is empty.

## Emitting
On the server, the final step is emitting the project model to Sitecore. This step copies files to the website directory and
create or modifies any items in Sitecore.

## Coding 

### Guidelines
Make other developers happy by doing the following.

Make every class as immutable as possible. 

[Say no to null](http://elegantcode.com/2010/05/01/say-no-to-null/). Never pass null as parameter to a method. No method 
should accept a null parameter.

Annotate method parameters and method returns with [NotNull] or [CanBeNull] and collections with [ItemNotNull] to avoid
null reference exceptions.

Do not use static classes (except for extension methods, of course). Use dependency injection instead.

Do not use regions.

Be aware that the compiler will compile source files out of order.

### MEF
Pathfinder uses [MEF](https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx) to support Dependency Injection,
Composition, extensibility and file-less configuration.

Pathfinder supplies a number of services, for instance a ``IFileSystemService`` that abstracts access to the file system.
Services are automatically registered in MEF during startup (using the 
[Export](https://msdn.microsoft.com/en-us/library/system.componentmodel.composition.exportattribute(v=vs.110).aspx) 
attribute).

Pathfinder uses constructor dependency injection, so you must provider a constructor that takes all requred services
as parameters (remember to annotate the constructor with the 
[ImportingConstructor](https://msdn.microsoft.com/en-us/library/system.componentmodel.composition.importingconstructorattribute(v=vs.110).aspx)
attribute). If you must add additional parameters, implement a ``With(...)`` method on the class.

```cs
var myNewService = myService.With(addtionalParameter);
```

The ``ICompositionService`` can be used to instantiate new objects, if they are exported.

```cs
public class MyClass 
{
  public MyClass([NotNull] ICompositionService compositionService) 
  {
    CompositionService = compositionService;
  }

  [NotNull]
  protected ICompositionService CompositionService { get; }

  protected void MyMethod() 
  {
    var foo = CompositionService.Resolve<IFoo>().With("additional parameters");
  }
}
```

Alternatively the ``IFactoryService`` can be used to instantiate new objects.

To enable plugins, Pathfinder adds any assemblies in the /sitecore.project/extensions and /sitecore.tools/extensions directories to 
the MEF graph. This means that developers can xcopy an assembly to an extensions folder, and Pathfinder will automatically
include the assembly without any configuration.

### Services

Services                | Description
------------------------|-------------
ICheckerService         | Check the project for warnings and errors.
IConfigurationService   | Load settings from the scconfig.json files.
IFactoryService         | Instantiate new objects.
IFileSystemService      | File system abstraction.
IParseService           | Parse a source file.
IPipelineService        | Pipelines (yay!).
IProjectService         | Load a project.
IQueryService           | Query the project.
IReferenceParserService | Parse references.
ISnapshotService        | Load a snapshot.
ITraceService           | Tracing.


### Command pattern
Pathfinder usually uses the Command pattern to provide functionality that can be extended. It also has the benefit of encapsulating
functionality in a single class which makes the system more robust.

```cs
[InheritedExport]
public interface IMyCommand
{
    bool CanExecute([NotNull] IMyContext context);
    void Execute([NotNull] IMyContext context);
}

[Export]
public class MyClass 
{
  [ImportingConstructor]
  public MyClass([ImportMany] [NotNull] IEnumerable<IMyCommand> myCommands) 
  {
    MyCommands = myCommands;
  }

  [NotNull]
  protected IEnumerable<IMyCommand> MyCommands { get; }

  public void Execute([NotNull] MyContext context) 
  {
    foreach (var command in MyCommands) 
    {
      if (command.CanExecute(context)) 
      {
        command.Execute(context);
      }
    }
  }
}
```

Any class that implements the ``IMyCommand``, is exported to MEF, since ``IMyCommand`` is annotated with the ``[InheritedExport]`` attribute.
The list of registered commands is injected in the MyClass constructor by MEF using the ``[ImportMany]`` attribute.

Any assembly that contains a command, will be discovered by MEF and supplied to MyClass for execution. 


