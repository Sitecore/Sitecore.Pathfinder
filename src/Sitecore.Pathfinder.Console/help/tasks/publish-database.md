publish-database
===============
Publishes a Sitecore database (usually the 'master' database).

Remarks
-------
The `publish-database` tasks makes a web request to the website to publish a database.
The database can be specified as a parameter to the task.

Settings
--------
| Setting name | Description             | 
|--------------|-------------------------|
| database     | The database to publish |

Example
-------
```cmd
> scc publish-database master
```