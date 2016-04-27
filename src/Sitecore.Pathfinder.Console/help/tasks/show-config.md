show-config
===========
Displays the effective configuration.

Remarks
-------
The `show-config` task output the expanded configuration.

Settings
--------
None.

Example
-------
```cmd
> scc show-config

{
  "arg0": "show-config",
  "build-project": {
    "compile-bin-files": {
      "exclude": "",
      "include": ""
    },
    "force-update": false,
    "media": {
      "template": {
        "bmp": "/sitecore/templates/System/Media/Unversioned/Image",
        "doc": "/sitecore/templates/System/Media/Unversioned/Doc",
        "docx": "/sitecore/templates/System/Media/Unversioned/Docx",
        "gif": "/sitecore/templates/System/Media/Unversioned/Image",
        "jpeg": "/sitecore/templates/System/Media/Unversioned/Jpeg",
        "jpg": "/sitecore/templates/System/Media/Unversioned/Jpeg",
        "m4v": "/sitecore/templates/System/Media/Unversioned/Movie",
        "mp3": "/sitecore/templates/System/Media/Unversioned/Mp3",
        "mp4": "/sitecore/templates/System/Media/Unversioned/Movie",
        "pdf": "/sitecore/templates/System/Media/Unversioned/Pdf",
        "png": "/sitecore/templates/System/Media/Unversioned/Image",
        "tiff": "/sitecore/templates/System/Media/Unversioned/Image",
        "wav": "/sitecore/templates/System/Media/Unversioned/Audio",
        "zip": "/sitecore/templates/System/Media/Unversioned/Zip"
      }
    },
    "parse-all-files": true,
    "run-sitecore-validators": true,
    "schema": {
      "Children-childnodes": "Item, Template",
      "Field-attributes": "Name, Value, Language, Version, Field.Type, Field.SortOrder, Field.Sharing, Field.Source, Field.ShortHelp, Field.LongHelp",
      "Fields-childnodes": "Field, Unversioned, Versioned, Layout",
      "Item-attributes": "Name, Id, Database, Icon, Template, IsEmittable, IsExternalReference, Layout.File, Template.CreateFromFields, Template.Id, Template.Icon, Template.BaseTemplates, Template.ShortHelp, Template.LongHelp",
      "Item-childnodes": "Fields, Children, Template",
      "Template-attributes": "Name, Id, Database, Icon, IsEmittable, IsExternalReference, ShortHelp, LongHelp, BaseTemplates, Layout.File",
      "Template-childnodes": "Section, Sections",
      "TemplateField-attributes": "Name, Id, Type, Sharing, Source, ShortHelp, LongHelp, SortOrder, StandardValue",
      "TemplateField-childnodes": "",
      "TemplateSection-attributes": "Name, Id, Icon",
      "TemplateSection-childnodes": "Field"
    }
  ...
}
```