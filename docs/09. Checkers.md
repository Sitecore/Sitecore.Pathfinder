# Checkers

## ConfigurationChecker

### TypeNotFound
"Type does not exist in a referenced assembly"


## FieldsChecker

### FieldContainsLoremIpsum
"Field contains 'Lorem Ipsum' text: The field "{fieldName}" contains the test data text: "Lorem Ipsum...". Replace or remove the text data."

### NumberIsNotValid
"Number is not valid: The field "{fieldName}" has a type of 'Number', but the value is not a valid number. Replace or remove the value."
 
### DateIsNotValid
"Date is not valid: The field "{fieldName}" has a type of 'Date', but the value is not a valid date. Replace or remove the value."

### DateTimeIsNotValid
"Datetime is not valid: The field "{fieldName}" has a type of 'Datetime', but the value is not a valid date. Replace or remove the value."


## FilesChecker

### AvoidLargeMediaFiles
"Media file size exceeds 5MB. Consider reducing the size of the file"


## ItemsChecker

### AvoidManyChildren
"Avoid items with many children: The item has {count} children. Items with more than 100 children decrease performance. Change the structure of the tree to reduce the number of children"

### AvoidManyVersions
"Avoid items with many version: The item has {count} versions in the {language} language. Items with more than 10 version decrease performance. Remove some of the older versions."

### AvoidSpacesInItemNames
"Avoid spaces in item names. Use a display name instead"

### FieldIsNotDefinedInTemplate
"Field is not defined in the template"

### ItemsWithSameDisplayName
"Items with same display name on same level: Two or more items have the same display name "{displayNames}" on the same level. Change the display name of one or more of the items."

### ItemsWithSameName
"Items with same name on same level: Two or more items have the same name "{itemName}" on the same level. Change the name of one or more of the items."

### ItemTemplateNotFound
"Template not found"

### ReminderDateIsAfterArchiveDate
"The Reminder date is after the Archive date: Change either the Reminder date or the Archive date."

### UnpublishDateIsBeforePublishDate
"The Publish date is after the Unpublish date: Change either the Publish date or the Unpublish date"

### ValidToDateIsBeforeValidFromDate
"The Valid From date is after the Valid To date: Change either the Valid From date or the Valid To date"


## ReferencesChecker

### ReferenceNotFound
"Reference not found"


## TemplateHelpChecker

### TemplateFieldLongHelpShouldEndWithDot
"Template field long help text should end with '.'"
        
### TemplateFieldLongHelpShouldStartWithCapitalLetter
"Template field long help text should start with a capital letter"

### TemplateFieldShortHelpShouldEndWithDot
"Template field short help text should end with '.'"

### TemplateFieldShortHelpShouldStartWithCapitalLetter
"Template field short help text should start with a capital letter"

### TemplateFieldShouldHaveLongHelp
"Template field should have a long help text"

### TemplateFieldShouldHaveShortHelp
"Template field should have a short help text"

### TemplateLongHelpShouldEndWithDot
"Template long help text should end with '.'"

### TemplateLongHelpShouldStartWithCapitalLetter
"Template long help text should start with a capital letter"

### TemplateShortHelpShouldEndWithDot
"Template short help text should end with '.'"

### TemplateShortHelpShouldStartWithCapitalLetter
"Template short help text should start with a capital letter"

### TemplateShouldHaveLongHelp
"Template should have a long help text"

### TemplateShouldHaveShortHelp
"Template should have a short help text"


## TemplatesChecker

### AvoidDeprecatedFieldType
"Avoid deprecated field type"

### AvoidDuplicateFieldNames
"Avoid duplicate template field names: The template contains two or more field with the same name "{fieldName}". Even if these fields are located in different sections, it is still not recommended as the name is ambiguous. Rename one or more of the fields"

### AvoidEmptyTemplate
"Empty templates should be avoided. Consider using the 'Folder' template instead"

### AvoidEmptyTemplateSection
"Avoid empty template section"

### AvoidSpacesInTemplateNames
"Avoid spaces in template names. Use a display name instead"

### DeleteUnusedTemplates
"Template is not referenced and can be deleted"

### FieldIdTemplateFieldId
"Field ID and Template Field ID differ"

### TemplateShouldHaveIcon
"Template should should have an icon"


## PathfinderProjectFileSystemConventions

### MissingCoreSitecoreDirectory
"The ~/items/core directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/core/sitecore directory"

### MissingItemsDirectory
"Missing ~/items directory for containing Sitecore items.", "To fix, create the ~/items directory"

### MissingMasterOrCoreItemsDirectory
"The ~/items directory should have either a 'master' or 'core' subdirectory for containing Sitecore items. To fix, create either a ~/items/master or a ~/items/core directory"

### MissingMasterSitecoreDirectory
"The ~/items/master directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/master/sitecore directory"

### MissingSccCmdFile
"Missing the ~/scc.cmd file. To fix, copy the scc.cmd file from the [Tools]/files/project directory"

### MissingScconfigJsonFile
"Missing the ~/scconfig.json file. The file contains the Pathfinder project configuration. To fix, copy the scconfig.json file from the [Tools]/files/project directory"

### MissingSitecoreProjectDirectory
"Missing ~/sitecore.project directory. To fix, create the ~/sitecore.project directory"

### MissingSitecoreProjectSchemasDirectory
"Missing the ~/sitecore.project/schemas directory. This directory contains Xml and Json schema files that help text editors with Completion and Validation. To fix, create the ~/sitecore.project/schemas directory"

### MissingWebSitecoreDirectory
"The ~/items/web directory should have a 'sitecore' subdirectory that matches the Sitecore root item. To fix, create the ~/items/web/sitecore directory"

### MissingWwwRootDirectory
"Missing the ~/wwwroot directory. This directory contains files that are copied to the website directory without change. To fix, create the ~/wwwroot directory"


## SitecoreTemplateConventions

### AvoidSettingSharedAndUnversionedInItems
"In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)"

### AvoidSettingSharedAndUnversionedInTemplates
"In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)"

### DefaultValueFieldIsObsolete
"In a template field, the 'Default value' field is no longer used. To fix, clear the 'Default value' field and set the value on the Standard Values item"

### TemplateIdOfStandardValuesShouldMatchParentId
"The Template ID of a Standard Values item should be match the ID of the parent item. To fix, moved the Standard Values item under the correct template"

### TemplateMustLocatedInTemplatesSection
"All templates should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section"

### TemplateNodeOrFolderShouldBeTemplateFolder
"In the '/sitecore/templates' section, folder items use the 'Template folder' template - not the 'Folder' or 'Node' template. To fix, change the template of the item to 'Template Folder"

### TemplateSectionShouldOnlyContainTemplates
"The '/sitecore/templates' section should only contain item with template 'Template', 'Template section', 'Template field', 'Template folder' or standard values items. To fix, move the item outside the '/sitecore/templates' section"

### TemplatesMustLocatedInTemplatesSection
"All items with template 'Template', 'Template section', 'Template field' and 'Template folder' should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section"


## ProjectChecker

### ProjectMustBeOutsideWebsite
"Project should not be located inside the website. To fix, move the project to a new directory"


## LayoutsChecker

### PlaceholdersShouldHaveAPlaceholderSettingsName
"Placeholders should have a Placeholder Settings item: To fix, create a '/sitecore/layout/Placeholder Settings/{placeholder}' item"

### RenderingNameAndFileNameShouldMatch
"Rendering item name should match file name: To fix, rename the rendering file or rename the rendering item"

### RenderingShouldBeInViewsFolder
"View rendering should be located in the ~/views/ directory: To fix, move the file to the ~/views/ directory"


## HabitatChecker

### DataSourceTemplatesMustInheritFromDataTemplates
"Data Source templates must not inherit from {itemName} as it is not a Data Template. To fix, either remove the inheritance or make the base template a Data Template"

### DataSourceTemplatesMustNotFields
"Page Type Templates must not have fields. To fix, move the fields to a Data Template"

### DataSourceTemplatesMustNotHaveLayout
"Data Source Templates must not have a layout. To fix, remove the layout from the template"

### DataTemplatesMustNotBeInstantiated
"Data Templates must not be instantiated. To fix, change the template of the item to a Page Type template or Data Source template"

### DataTemplatesMustNotHaveLayout
"Data Templates must not have a layout. To fix, create a Page Type Template that inherits from this template and assign the layout to that template"

### DataTemplatesNotAllowedInLayer
"Data Templates are not allowed in the '{Layer}' layer. To fix, move the template to another layer"

### FolderTemplatesMustHaveFolderPostfix
"Templates without fields should have the 'Folder' postfix. To fix, rename the template to '{itemName}Folder'"

### FolderTemplatesMustNotHaveFields
"Folder templates must not have any fields. To fix, remove the fields"

### LayoutMustBeInCorrectLayer
"Layout items should be located in the correct layer '{path}'. To fix, move the layout item into the '{path}' layer"

### LayoutMustBeInCorrectModule
"Layout items should be located in the correct module '{path}'. To fix, move the layout item into the '{path}' module"

### MissingCodeDirectory
"The root directory should have a 'code' subdirectory. To fix, create the ~/code directory"

### MissingSerializationDirectory
"The root directory should have a 'serialization' subdirectory. To fix, create the ~/serialization directory"

### MissingTestsDirectory
"The root directory should have a 'tests' subdirectory. To fix, create the ~/tests directory"

### ModelsMustBeInCorrectLayer
"Model items should be located in the correct layer '{path}'. To fix, move the Model item into the '{path}' layer"

### ModelsMustBeInCorrectModule
"Model items should be located in the correct module '{path}'. To fix, move the Model item into the '{path}' module"

### PageTypeTemplatesMustHaveLayout
"Page Type Templates must have a layout. To fix, assign a layout to the template"

### PageTypeTemplatesMustInheritFromDataTemplates
"Page Type templates cannot inherit from {baseTemplate.ItemName} as it is not a Data Template. Fix fix, either remove the inheritance or make the base template a Data Template"

### PageTypeTemplatesMustNotHaveFields
"Page Type Templates must not have fields. To fix, move the fields to a Data Template"

### PageTypeTemplatesNotAllowedInLayer
"Page Type templates are not allowed in the '{Layer}' layer"

### PageTypeTemplatesShouldHaveStandardValues
"Page Type templates should have a Standard Values item. To fix, create a Standard Value item"

### PlaceholderSettingsMustBeInCorrectLayer
"Placeholder Settings items should be located in the correct layer '{path}'. To fix, move the Placeholder Settings item into the '{path}' layer"

### PlaceholderSettingsMustBeInCorrectModule
"Placeholder Settings items should be located in the correct module '{path}'. To fix, move the Placeholder Settings item into the '{path}' module"

### RenderingsMustBeInCorrectLayer
"Rendering items should be located in the correct layer '{path}'. To fix, move the rendering item into the '{path}' layer"

### RenderingsMustBeInCorrectModule
"Rendering items should be located in the correct module '{path}'. To fix, move the rendering item into the '{path}' module"

### RootDirectoryNameDoesNotMatchModuleName
"The root directory name should match the module name. To fix, either rename the current directory or change the 'module' configuration"

### SettingsItemsNotAllowedInLayer
"Settings items are not allowed in the '{Layer}' layer"

### SettingsMustBeInCorrectLayer
"Settings items should be located in the correct layer '{path}'. To fix, move the settings item into the '{path}' layer"

### SettingsMustBeInCorrectModule
"Settings items should be located in the correct module '{path}'. To fix, move the settings item into the '{path}' module"

### TemplateMustBeInCorrectLayer
"Templates should be located in the correct layer '{path}'. To fix, move the template into the '{path}' layer"

### TemplateMustBeInCorrectModule
"Templates should be located in the correct module '{path}'. To fix, move the template into the '{path}' module"

### AvoidUsingFolderTemplate
"Avoid using the 'Folder' template. To fix, create a new 'Folder' template, assign Insert Options and change the template of this item"

### FolderTemplatesShouldHaveInsertOptions
"Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item"

### RenderingParametersTemplateMustDeriveFromStandardRenderingParameters
"Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item"

### ControllerRenderingNotAllowedInLayer
"Controller renderings are not allowed in the '{Layer}' layer"

### ConfigFileMustBeInCorrectModuleDirectory
"Config file must be the correct module directory", configFile.Snapshots.First().SourceFile, $"To fix, move the file to the '{path}' directory"
