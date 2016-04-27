check-project
===========
Checks the project for warnings and errors.

Remarks
-------
The 'check-project' tasks executes checks and conventions.

Settings
--------
| Setting name                      | Description                                                                    | 
|-----------------------------------|--------------------------------------------------------------------------------|
| check-project:disabled-categories | Disables checker categories (Items, Fields, Templates, TemplateFields, Media). |
| check-project:disabled-checkers   | Disables specific checkers.                                                    |

Example
-------
```cmd
> scc check-project

scc.cmd(0,0): information SCC3041: Checking project...
scc.cmd(0,0): information SCC3042: Errors: 0, warnings: 0, messages: 0, checks: 38, conventions: 20
```

