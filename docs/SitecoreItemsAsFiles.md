# Sitecore items and templates as files
In Pathfinder everything is a file, including Sitecore items. This is so that your project directory can contain the whole and single truth
about your project. Your project is no longer spread across development projects, databases and websites.

This is also how classic development projects work. In a Visual Studio application project every asset, that is needed by the application, is
included or referenced from the project.

Items are stored as files but can have a number of formats. Currently Json, Yaml and Xml formats are supported. Yaml is recommended, but Json 
and Xml are good formats, since code editors can support schema validation and IntelliSense.

Yaml format (extension .content.yaml): 
```yaml
- Sample Item: HelloWorld
    ItemPath: /sitecore/content/Home/HelloWorld
    Database: master
    - Fields:
        - en:
            Title: Sitecore
            Text: Welcome to Sitecore
        - da:
            Title: Sitecore
            Text: Velkommen til Sitecore
    - Items: 
        - Sample Item: Subitem
            - Fields:
                - en:
                    Title: Sitecore
                    Text: Welcome to Sitecore
                - da:
                   Title: Sitecore
                   Text: Velkommen til Sitecore
        - Template:
            Name: MyTemplate
            - Section:
                Name: Fields
                - Field:
                    Name: IsActive
                    Type: Checkbox
                    Sharing: Shared
                - Field:
                    Name: Text
                    Type: Single-Line Text
                    Sharing: Unversioned
```

Json format (extension .content.json): 
```js
{
    "Sample Item": {
        "Name": "HelloWorld",
        "ItemPath": "/sitecore/content/Home/HelloWorld",
        "Database": "master",
        "Fields": {
            "en": {
                "Title": "Sitecore",
                "Text": "Welcome to Sitecore"
            },
            "da": {
                "Title": "Sitecore",
                "Text": "Velkommen til Sitecore"
            }
        },
        "Items": {
            "Sample Item": [
                {
                    "Name": "Subitem",
                    "Fields": {
                        "en": {
                            "Title": "Sitecore",
                            "Text": "Welcome to Sitecore"
                        },
                        "da": {
                            "Title": "Sitecore",
                            "Text": "Velkommen til Sitecore"
                        }
                    }
                }
            ],

            "Template": [
                {
                    "Name": "MyTemplate",
                    "Icon": "Applications/16x16/about.png",
                    "ShortHelp": "Short help.",
                    "LongHelp": "Long help.",
                    "Section": [
                        {
                            "Name": "Fields",
                            "Field": [
                                {
                                    "Name": "IsActive",
                                    "Type": "Checkbox",
                                    "Sharing": "Shared"
                                },
                                {
                                    "Name": "Text",
                                    "Type": "Single-Line Text",
                                    "Sharing": "Unversioned"
                                }
                            ]
                        }
                    ]
                }
            ]
        }
    }
}
```

Xml format (extension .content.xml) - please notice the namespace, which indicates the Xml schema to use.
```xml
<SampleItem TemplateName="Sample Item" Name="HelloWorld" ItemPath="/sitecore/content/Home/HelloWorld" Database="master">
    <Fields>
        <en Title="Sitecore" Text="Welcome to Sitecore" />
        <da Title="Sitecore" Text="Velkommen til Sitecore" />
    </Fields>
    <Items>
        <SampleItem TemplateName="Sample Item" Name="Subitem">
            <Fields>
                <en>
                    <Title>Sitecore</Title>
                    <Text>Welcome to Sitecore</Text>
                </en>
                <da>
                    <Title>Sitecore</Title>
                    <Text>Velkommen til Sitecore</Text>
                </da>
            </Fields>
        </SampleItem>

        <Template Name="MyTemplate">
            <Section Name="Fields">
                <Field Name="IsActive" Type="Checkbox" Sharing="Shared" />
                <Field Name="Text" Type="Single-Line Text" Sharing="Unversioned" />
            </Section>
        </Template>
    </Items>
</SampleItem>
```

The directory path can be used as item path by configuring the "project-website-mappings" settings. By default the 
[Project]/items/master/sitecore directory of project corresponds to /sitecore in the master database, so having the item file
"[Project]\items\master\sitecore\content\Home\HelloWorld.content.yaml" will create the item "/sitecore/content/Home/HelloWorld" in the
master database.

### Inferred templates
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

### Standard Values
You can specify a special __Standard Values item in a template to defined Standard Values.

```yaml
- Folder: Tests
    ItemPath: /sitecore/client/Applications/SitecoreWorks/content/TemplateStandardValuesTest
    Database: master
    - Items: 
        - Template:
            Name: TemplateStandardValuesTest
            - Section: Fields
                - Field: IsActive
                    Type: Checkbox
                    Sharing: Shared
                - Field: Text
                    Type: Single-Line Text
                    Sharing: Unversioned
            - TemplateStandardValuesTest: __Standard Values
                - Fields:
                    IsActive: true
                    - en:
                        Text: Welcome to Sitecore
                - Layout:
                    - Default: Sample Layout
                        - main:
                            - Sample Sublayout:
                                - centercolumn:
                                    - Sample Inner Sublayout:
                                        - content:
                                            - Sample Rendering:
```

## Item IDs
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

## Populating additional fields for implicitly created items

Supposed you have an image Lighthouse.jpg and want to set the Alt field, simply create a Lighthouse.content.json next to the 
Lighthouse.jpg file.

* Lighthouse.jpg
* Lighthouse.content.json

When determining the item name, Pathfinder uses the field up until the first dot - in this case "HelloWorld". When two or more files have the
same item name (and item path), they are merged into a single item. Pathfinder will report an error if a field is set more than once with different
values.

## Search and replace tokens
You can define tokens in the scconfig.json that will be replaced when item files are read. 

Json:
```js
{
    // global search and replace tokens - case-sensitive
    "search-and-replace-tokens": {
        "replace" : "with"
    },
}
```

Any occurance of the text "$replace" in an item file will be replaced with the text "with".

### Predefined parameters.

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

## Media files
If you drop a media file (.gif, .png, .jpg, .bmp, .pdf, .docx) into your project folder, Pathfinder will upload the file to the Media Library.
The Sitecore item will be implicit created. 

## Layouts and renderings
Layout and rendering files (.aspx, .ascx, .cshtml, .html) are copied to the website directory and the Sitecore items are automatically created.
You no longer have to explicitly create and configure a Sitecore Rendering or Layout item. The relevate fields (including the Path field) are
populated automatically.

