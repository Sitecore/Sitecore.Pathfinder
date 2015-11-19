File: *.item.json
Compatible with JsonDataProvider format.
```js
{
    "Item": {
        "Database": "master",
        "ID ": "{DC45CA60-32DC-417D-ABBC-A85B563CE6DE}", // support ID or Id
        "Name" : "JsonItem",
        "Path" : "/sitecore/content/Home", // support TemplateID or TemplateId
        "TemplateID": "{52F088BE-C7A9-4D26-BC95-9F3CD7937F75}",
        "Template": "/sitecore/templates/Sample/JsonItem",

        "Fields": {
            "Title": "Sitecore", // shared fields
            "Text": {
                "Value": "Welcome to Sitecore"
            },

            "Shared": {
                "Title": "Sitecore",
                "Text": {
                    "Value": "Welcome to Sitecore"
                },
            },

            "Unversioned": {
                "da-DK": {
                    "Body": "Body Text"
                }
            },

            "Versioned": {
                "da-DK": {
                    "1": {
                        "Article": "Article"
                    },
                    "2": {
                        "Article": "Article"
                    }
                }
            },

            "Layout": {
                var actionPanel = new ActonPanel({ "title": "Welcome" });

                var dashboard = new Dashboard({ "columns": 1, "rows": 2 }, actionPanel);

                return dashboard;
            }
        },

        "Children": [
            {
                "Name" : "Child"
            }
        ]
    }
}
```

File: *.content.json
These files can be validated through a schema to ensure that fields corrospond to a template field.
```js
{
    "JsonItem": {
        "Database": "master",
        "ID ": "{DC45CA60-32DC-417D-ABBC-A85B563CE6DE}", // support ID or Id
        "Name" : "JsonItem",
        "Path" : "/sitecore/content/Home", // support TemplateID or TemplateId
        "TemplateID": "{52F088BE-C7A9-4D26-BC95-9F3CD7937F75}",
        "Template": "/sitecore/templates/Sample/JsonItem",

        "Title": "Sitecore", // shared fields
        "Text": {
            "Value": "Welcome to Sitecore"
        },
         
        "Fields.Shared": {
            "Title": "Sitecore",
            "Text": {
                "Value": "Welcome to Sitecore"
            },
        },

        "Fields.Unversioned": {
            "da-DK": {
                "Body": "Body Text"
            }
        },

        "Fields.Versioned": {
            "da-DK": {
                "1": {
                    "Article": "Article"
                },
                "2": {
                    "Article": "Article"
                }
            }
        },

        "Fields.Layout": {
        },

        "Children": [
            {
                "Name" : "Child"
            }
        ]
    }
}
```

File: *.item.xml
```xml
<!-- support ID or Id -->
<!-- support TemplateID or TemplateId -->
<Item xmlns="http://www.sitecore.net/pathfinder/item" 
    Database="master"
    ID="{DC45CA60-32DC-417D-ABBC-A85B563CE6DE}" 
    Name="JsonItem"
    Path="/sitecore/content/Home" 
    TemplateID="{52F088BE-C7A9-4D26-BC95-9F3CD7937F75}"
    Template="/sitecore/templates/Sample/JsonItem">

    <Fields>
        <Field Name="Title" Value="Hello" />
        <Field Name="Text">Hello World</Field>

        <Shared>
            <Field Name="Title" Value="Hello" />
            <Field Name="Text">Hello World</Field>
        </Shared>
        
        <Unversioned Language="da-DK">                     
            <Field Name="UnversionedField">Hello</Field>
        </Unversioned>

        <Versioned Language="da-DK">
            <Version Number="1">
                <Field Name="VersionedField">Version 1</Field>
            </Version>
            <Version Number="2">
                <Field Name="VersionedField">Version 2</Field>
            </Version>
        </Versioned>

        <Layout xmlns="http://www.sitecore.net/pathfinder/layouts/master">
        
            var actionPanel = new ActionPanel(placeholder: "Dashboard.Main");

            var dashboard = new Dashboard(columns: 1, rows: 2) 
            {
               actionPanel
            };

            return dashboard;

        </Layout>
    </Fields>

    <Children>
        <Item />
    </Children>
</Item>
```

File: *.content.xml
These files can be validated through a schema to ensure that fields corrospond to a template field.
```xml
// support ID or Id
// support TemplateID or TemplateId
<Sample.Item xmlns="http://www.sitecore.net/pathfinder/content/master" 
    Database="master"
    ID="{DC45CA60-32DC-417D-ABBC-A85B563CE6DE}"
    Name="JsonItem"
    Path="/sitecore/content/Home"
    TemplateID="{52F088BE-C7A9-4D26-BC95-9F3CD7937F75}"
    Template="/sitecore/templates/Sample/JsonItem"
    Text="Hello World">

    <Fields.Unversioned Language="en-US" Text="Hello" />
    <Fields.Versioned Language="da-DK" Version="1" Text="Hello world"/>
    <Fields.Versioned Language="da-DK" Version="2" Text="Hello friend" />
    <Fields.Layout>
    </Fields.Layout>                                  

    <Children>
        <Sample.Item />
    </Children>
</Sample.Item>
```