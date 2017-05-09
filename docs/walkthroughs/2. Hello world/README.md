# Hello world

This walkthrough shows how to build a Hello World website.

The following assumes, that you have already setup Pathfinder and created a new Sitecore website.

## Create an empty folder

Create a new empty folder that will contain your project. This folder should be separate from the
Sitecore website, so you project folder only contains the project files.

![CreateFolder](CreateFolder.png)

## Create HelloWorld item file

Create a new item file by executing the `generate-file` task.

```
scc g y HelloWorld
```

![CreateItemFile](CreateItemFile.png)

## Edit the HelloWorld item

Open the file in Notepad and paste the following text into it.

```yaml
- Sample Item: HelloWorld
    ItemPath: /sitecore/content/Home/HelloWorld
    Database: master
    - Fields:
        - en:
            Title: Hello World
            Text: Hello from Pathfinder
```

This item file create the /sitecore/content/Home/HelloWorld item.

## Compile the project

Compile the project by executing `scc b` in the command prompt. 

![CompileProject](CompileProject.png)

Check that there are no errors or warnings.

There should now be a Package.zip file in the ./dist directory. 

Try installing the Package.zip file in Sitecore and check that the /sitecore/content/Home/HelloWorld has been created.

