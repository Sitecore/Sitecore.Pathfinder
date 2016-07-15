check-website
=============
Runs checkers on the website.

Remarks
-------
The `check-website` run checkers on the website without a Pathfinder project. 

The task create a virtual project from the items and templates in the 'master' database and executes
checkers on the virtual project. 

Since the task runs on the website, it can execute additional task which cannot be run in a project.

Settings
--------
None.

Example
-------
```cmd
> scc check-website --website-directory c:\inetpub\wwwroot\Sitecore\Website

```

