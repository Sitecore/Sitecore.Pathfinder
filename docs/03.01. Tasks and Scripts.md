#Tasks and scripts

## Tasks
The Pathfinder compiler supports a number of tasks and these tasks make up the tool chain. Most tasks provide functionality for 
compiling and deploying a package.

To execute a task run `scc [task name]` from the command line. If you do not specify a task name, the 'help' task is
executed.

Task Name | Description
----------|------------
check-project | Checks the project for warnings and errors.
find-references | Finds all project items that the specified project item references.
find-usages | Finds all project items that references the specified project item.
generate-code | Generates code from items and files in the project.
help | Displays version information and a list of commands.
init-project | Creates a default scconfig.json file.
list-information | Lists various information about the project.
publish-project | Creates a deployable package in the output directory.
show-config | Shows the effective configuration.
show-metrics | Shows various information about the project.
write-exports | Writes export declarations
