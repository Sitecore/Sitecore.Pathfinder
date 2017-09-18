list-information
================
Lists various information.

Settings
--------
None.

Example
-------
```cmd
> scc list checkers
```


scc list items
==============
The `scc list items` tasks outputs a list of all the items in the project (excluding imported items).

Example
-------
```cmd
> scc list items

/sitecore/content/Home/HelloWorld
/sitecore/content/Home/Welcome
/sitecore/layout/renderings/HelloWorld
/sitecore/media library/lighthouse
```


scc list files
============== 
The `scc list files` tasks outputs a list of all the files in the project.

Example
-------
```cmd
> scc list files

~/jsx/timer.jsx
~/views/React.page.html
```

      
scc list output
===============
Display the items and files as they will be installed on the website.

The `list-output` task displays the full path of items and files as they are installed on the website.

This is useful for see what items and files are installed and where.

Please notice that some files may omitted from the installation by the NuGet specification.

Example
-------
```cmd
> scc list-output

```


scc list checkers
================= 
Lists the available checkers. The 'list checkers' task outputs the names of the available checkers.

Example
-------
```cmd
> scc list checkers

AvoidDeprecatedFieldType [Templates]
AvoidDuplicateFieldNames [Templates]
AvoidEmptyTemplate [Templates]
AvoidEmptyTemplateSection [Templates]
AvoidLargeMediaFiles [Files]
AvoidManyChildren [Items]
AvoidManyVersions [Items]
AvoidSettingSharedAndUnversionedInItems [SitecoreTemplate]
AvoidSettingSharedAndUnversionedInTemplates [SitecoreTemplate]
AvoidSpacesInItemNames [Items]
AvoidSpacesInTemplateNames [Templates]
Conventions [Convention]
DefaultValueFieldIsObsolete [SitecoreTemplate]
DeleteUnusedTemplates [Templates]
FieldContainsLoremIpsum [Fields]
FieldIdTemplateFieldId [Templates]
FieldIsNotDefinedInTemplate [Items]
GuidClash [Guid]
ItemsWithSameDisplayName [Items]
ItemsWithSameName [Items]
ItemTemplateNotFound [Items]
MissingCoreSitecoreDirectory [PathfinderProjectFileSystem]
MissingItemsDirectory [PathfinderProjectFileSystem]
MissingMasterOrCoreItemsDirectory [PathfinderProjectFileSystem]
MissingMasterSitecoreDirectory [PathfinderProjectFileSystem]
MissingSccCmdFile [PathfinderProjectFileSystem]
MissingScconfigJsonFile [PathfinderProjectFileSystem]
MissingSitecoreProjectDirectory [PathfinderProjectFileSystem]
MissingSitecoreProjectPackageDirectory [PathfinderProjectFileSystem]
MissingSitecoreProjectSchemasDirectory [PathfinderProjectFileSystem]
MissingWebSitecoreDirectory [PathfinderProjectFileSystem]
MissingWwwRootDirectory [PathfinderProjectFileSystem]
NumberIsNotValid [Fields]
ReferenceNotFound [References]
ReminderDateIsAfterArchiveDate [Items]
TemplateFieldLongHelpShouldEndWithDot [TemplateHelp]
TemplateFieldLongHelpShouldStartWithCapitalLetter [TemplateHelp]
TemplateFieldShortHelpShouldEndWithDot [TemplateHelp]
TemplateFieldShortHelpShouldStartWithCapitalLetter [TemplateHelp]
TemplateFieldShouldHaveLongHelp [TemplateHelp]
TemplateFieldShouldHaveShortHelp [TemplateHelp]
TemplateIdOfStandardValuesShouldMatchParentId [SitecoreTemplate]
TemplateLongHelpShouldEndWithDot [TemplateHelp]
TemplateLongHelpShouldStartWithCapitalLetter [TemplateHelp]
TemplateMustLocatedInTemplatesSection [SitecoreTemplate]
TemplateNodeOrFolderShouldBeTemplateFolder [SitecoreTemplate]
TemplateSectionShouldOnlyContainTemplates [SitecoreTemplate]
TemplateShortHelpShouldEndWithDot [TemplateHelp]
TemplateShortHelpShouldStartWithCapitalLetter [TemplateHelp]
TemplateShouldHaveIcon [Templates]
TemplateShouldHaveLongHelp [TemplateHelp]
TemplateShouldHaveShortHelp [TemplateHelp]
TemplatesMustLocatedInTemplatesSection [SitecoreTemplate]
TypeNotFound [Configuration]
UnpublishDateIsBeforePublishDate [Items]
UseIdInsteadOfPath [Templates]
ValidToDateIsBeforeValidFromDate [Items]
```

scc list project
================
Lists the project items (Sitecore items and files)

The `scc list project` tasks outputs a list of all the items and files in the project (including imported items).

Example
-------
```cmd
> scc list-project

/sitecore (Item)
/sitecore/content (Item)
/sitecore/layout (Item)
/sitecore/media library (Item)
/sitecore/shell (Item)
/sitecore/social (Item)
/sitecore/system (Item)
/sitecore/templates (Item)
/sitecore/content/Home (Item)
/sitecore/layout/Devices/Default (Item)
/sitecore/layout/Devices/Print (Item)
/sitecore/layout/Devices/Feed (Item)
/sitecore/layout/Layouts/Sample Layout (Item)
/sitecore/layout/Layouts/System/App Center Sync/App Center Placeholder/App Center Placeholder Layout (Item)
/sitecore/layout/Layouts/System/Feed Delivery Layout (Item)
/sitecore/layout/Layouts/System/FXM/ExperienceEditor (Item)
/sitecore/layout/Layouts/System/Simulated Device Layout (Item)
/sitecore/layout/Renderings/App Center Sync/AppEditor (Item)
/sitecore/layout/Renderings/App Center Sync/HiddenComponents/HiddenComponentsBegin (Item)
/sitecore/layout/Renderings/App Center Sync/HiddenComponents/HiddenComponentsEnd (Item)
/sitecore/layout/Renderings/App Center Sync/HiddenComponents/HiddenComponentsHead (Item)
```

scc list roles
==================
Lists the available project roles.

The 'list-project-roles' task outputs the names of the available project roles.

Example
-------
```cmd
> scc list-project-roles

default
none
pathfinder
```
