compare-projects
================
Compares two projects.
         
Remarks
-------
The 'compare-projects' task compares the items and templates in the current directory with the items and templates in another project directory.

Settings
--------
None.

Example
-------
```cmd
> scc compare-projects ../project2

/sitecore/content/Home/HelloWorld/Icon [Item]
- Applications/16x16/about.png
/sitecore/content/Home/HelloWorld/Text/Value [Field]
- Welcome to Sitecore Pathfinder
+ Welcome to Sitecore Pathfinder 123
/sitecore/content/Home/HelloWorld/Title/Value [Field]
- Pathfinder
+ Pathfinder 123
```
