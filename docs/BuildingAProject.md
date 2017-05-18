# Building a project
The build tool chain is specified in the build-project/tasks configuration setting. The default value is 
``"check-project, clean-output, write-exports, publish-project, show-metrics"``.

1. Check the project for warnings and errors. Any errors will stop the build.
2. Clean the output directory.
1. Writes export declarations to the [Project]/sitecore.project/exports.xml file.
1. Publish the project to the output directory
1. Show project status.

## Alternate tasks configuration
In a default build, Pathfinder will execute the tasks specified in the `tasks:build` configuration, but you easily can execute other tasks.

1. Specify the task to execute directly on the command-line: e.g. `scc publish-database`.
1. Specify an alternative tasks list in configuration and execute it on the command line, e.g.:
    ```js
    {
        "tasks": {
            "custom" : "restore-packages, check-project, show-metrics"
        }
    }
    ```
    To run, execute `scc custom`
4. Specify an new configuration file on the command-line to overlay the existing configuration: `scc /config myconfig.json`.

## Dependencies and exports
A project can declare items and resources that are used by other projects. The `write-exports` task writes export declarations of all
items and templates in the project to the [Project]/dist/project.exports.xml file.

### Reference packages
A project can depend on other packages. Reference packages are located in the [Tools]/files/references directory. 

To add a new dependency package, add it to the `references` configuration section. 

```js
"references": {
    "Sitecore.BusinessComponentLibrary": "1.0.0",
    "Sitecore.Core": "1.0.0",
    "Sitecore.Launchpad": "1.0.0",
    "Sitecore.Speak": "1.0.0"
}
```
