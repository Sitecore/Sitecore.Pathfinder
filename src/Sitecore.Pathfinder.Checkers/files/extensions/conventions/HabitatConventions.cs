// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    [Export]
    public class HabitatConventions : Checker
    {
        protected static readonly Guid StandardRenderingParametersGuid = new Guid("{8CA06D6A-B353-44E8-BC31-B528C7306971}");

        private string _layerName;

        private string _moduleName;

        [ImportingConstructor]
        public HabitatConventions([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected string Layer => _layerName ?? (_layerName = Configuration.GetString("habitat:layer"));

        [NotNull]
        protected string Module => _moduleName ?? (_moduleName = Configuration.GetString("habitat:module"));

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidUsingFolderTemplate(ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName == "Folder"
                select Warning(Msg.C1000, $"Avoid using the 'Folder' template. To fix, create a new 'Folder' template, assign Insert Options and change the template of this item", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> ConfigFileMustBeInCorrectModuleDirectory(ICheckerContext context)
        {
            var path = "/App_Config/include/" + Layer + "/" + Module;

            return from configFile in context.Project.ProjectItems.OfType<ConfigFile>()
                where !configFile.FilePath.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                select Error(Msg.C1000, $"Config file must be the correct module directory", configFile.Snapshots.First().SourceFile, $"To fix, move the file to the '{path}' directory");
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> ControllerRenderingNotAllowedInLayer(ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-controller-rendering"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from item in context.Project.Items
                where item.TemplateName == "Controller"
                select Error(Msg.C1000, $"Controller renderings are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataSourceTemplatesMustInheritFromDataTemplates(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template)
                from baseTemplate in template.GetBaseTemplates()
                where baseTemplate.ItemName != "Standard template" && !IsDataTemplate(baseTemplate)
                select Error(Msg.C1000, $"Data Source templates must not inherit from {baseTemplate.ItemName} as it is not a Data Template. To fix, either remove the inheritance or make the base template a Data Template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataSourceTemplatesMustNotFields(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template) && template.Fields.Any()
                select Error(Msg.C1000, $"Page Type Templates must not have fields. To fix, move the fields to a Data Template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataSourceTemplatesMustNotHaveLayout(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template) && HasLayout(template)
                select Error(Msg.C1000, $"Data Source Templates must not have a layout. To fix, remove the layout from the template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataTemplatesMustNotBeInstantiated(ICheckerContext context)
        {
            return from item in context.Project.Items
                where IsDataTemplate(item.Template)
                select Error(Msg.C1000, $"Data Templates must not be instantiated. To fix, change the template of the item to a Page Type template or Data Source template", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataTemplatesMustNotHaveLayout(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataTemplate(template) && HasLayout(template)
                select Error(Msg.C1000, $"Data Templates must not have a layout. To fix, create a Page Type Template that inherits from this template and assign the layout to that template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DataTemplatesNotAllowedInLayer(ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-data-templates"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from template in context.Project.Templates
                where IsDataTemplate(template)
                select Error(Msg.C1000, $"Data Templates are not allowed in the '{Layer}' layer. To fix, move the template to another layer", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> FolderTemplatesMustHaveFolderPostfix(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !IsDataSourceTemplate(template) && !IsPageTypeTemplate(template) && !IsRenderingParametersTemplate(template) && !template.Fields.Any() && !template.ItemName.EndsWith("Folder")
                select Warning(Msg.C1000, $"Templates without fields should have the 'Folder' postfix. To fix, rename the template to '{template.ItemName}Folder'", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> FolderTemplatesMustNotHaveFields(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where template.ItemName.EndsWith("Folder") && template.Fields.Any()
                select Error(Msg.C1000, $"Folder templates must not have any fields. To fix, remove the fields", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> FolderTemplatesShouldHaveInsertOptions(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where template.ItemName.EndsWith("Folder") && template.StandardValuesItem != null && string.IsNullOrEmpty(template.StandardValuesItem["__Masters"])
                select Error(Msg.C1000, $"Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> LayoutMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/layout/Layouts/" + Layer;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Layouts/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Layout items should be located in the correct layer '{path}'. To fix, move the layout item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> LayoutMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/layout/Layouts/" + Layer + "/" + Module;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Layouts/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Layout items should be located in the correct module '{path}'. To fix, move the layout item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> MissingCodeDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/code"))
            {
                yield return Warning(Msg.C1000, "The root directory should have a 'code' subdirectory. To fix, create the ~/code directory");
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> MissingSerializationDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/serialization"))
            {
                yield return Warning(Msg.C1000, "The root directory should have a 'serialization' subdirectory. To fix, create the ~/serialization directory");
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> MissingTestsDirectory(ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/tests"))
            {
                yield return Warning(Msg.C1000, "The root directory should have a 'tests' subdirectory. To fix, create the ~/tests directory");
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> ModelsMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/layout/Models/" + Layer;
            return from item in context.Project.Items
                where item.TemplateName == "Model" && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Model items should be located in the correct layer '{path}'. To fix, move the Model item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> ModelsMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/layout/Models/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where item.TemplateName == "Model" && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Model items should be located in the correct module '{path}'. To fix, move the Model item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PageTypeTemplatesMustHaveLayout(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && !HasLayout(template)
                select Error(Msg.C1000, $"Page Type Templates must have a layout. To fix, assign a layout to the template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PageTypeTemplatesMustInheritFromDataTemplates(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template)
                from baseTemplate in template.GetBaseTemplates()
                where baseTemplate.ItemName != "Standard Template" && !IsDataTemplate(baseTemplate)
                select Error(Msg.C1000, $"Page Type templates cannot inherit from {baseTemplate.ItemName} as it is not a Data Template. Fix fix, either remove the inheritance or make the base template a Data Template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PageTypeTemplatesMustNotHaveFields(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && template.Fields.Any()
                select Error(Msg.C1000, $"Page Type Templates must not have fields. To fix, move the fields to a Data Template", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PageTypeTemplatesNotAllowedInLayer(ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-page-type-templates"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from template in context.Project.Templates
                where IsPageTypeTemplate(template)
                select Error(Msg.C1000, $"Page Type templates are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PageTypeTemplatesShouldHaveStandardValues(ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && template.StandardValuesItem == null
                select Error(Msg.C1000, $"Page Type templates should have a Standard Values item. To fix, create a Standard Value item", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PlaceholderSettingsMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/layout/Placeholder Settings/" + Layer;
            return from item in context.Project.Items
                where item.TemplateName == "Placeholder" && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Placeholder Settings items should be located in the correct layer '{path}'. To fix, move the Placeholder Settings item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> PlaceholderSettingsMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/layout/Placeholder Settings/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where item.TemplateName == "Placeholder" && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Placeholder Settings items should be located in the correct module '{path}'. To fix, move the Placeholder Settings item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> RenderingParametersTemplateMustDeriveFromStandardRenderingParameters(ICheckerContext context)
        {
            var standardRenderingParametersTemplate = context.Project.ProjectItems.OfType<Template>().FirstOrDefault(t => t.Uri.Guid == StandardRenderingParametersGuid);
            if (standardRenderingParametersTemplate == null)
            {
                return new[]
                {
                    Error(Msg.C1000, "'StandardRenderingParameters' template not found. Are you missing an import?", SourceFile.Empty)
                };
            }

            return from template in context.Project.Templates
                where IsRenderingParametersTemplate(template) && !template.Is(standardRenderingParametersTemplate)
                select Error(Msg.C1000, $"Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> RenderingShouldBeInViewsFolder(ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                where !rendering.FilePath.StartsWith("~/views/", StringComparison.OrdinalIgnoreCase)
                select Warning(Msg.C1000, "View rendering should be located in the ~/views/ directory", rendering.Snapshots.First().SourceFile, $"To fix, move the file to the ~/views/ directory");
        }                                      

        [Export("Check")]
        public IEnumerable<Diagnostic> RenderingsMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/layout/Renderings/" + Layer;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Renderings/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Rendering items should be located in the correct layer '{path}'. To fix, move the rendering item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> RenderingsMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/layout/Renderings/" + Layer + "/" + Module;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Renderings/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Rendering items should be located in the correct module '{path}'. To fix, move the rendering item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> RootDirectoryNameDoesNotMatchModuleName(ICheckerContext context)
        {
            if (Path.GetFileName(Directory.GetCurrentDirectory()) != Module)
            {
                yield return Warning(Msg.C1000, "The root directory name should match the module name. To fix, either rename the current directory or change the 'module' configuration");
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> SettingsItemsNotAllowedInLayer(ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-settings-items"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from item in context.Project.Items
                where IsSettingsItem(item)
                select Error(Msg.C1000, $"Settings items are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> SettingsMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/system/Settings/" + Layer;
            return from item in context.Project.Items
                where IsSettingsItem(item) && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Settings items should be located in the correct layer '{path}'. To fix, move the settings item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> SettingsMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/system/Settings/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where IsSettingsItem(item) && !item.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Settings items should be located in the correct module '{path}'. To fix, move the settings item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateMustBeInCorrectLayer(ICheckerContext context)
        {
            var path = "/sitecore/templates/" + Layer;
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Templates should be located in the correct layer '{path}'. To fix, move the template into the '{path}' layer", TraceHelper.GetTextNode(template));
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateMustBeInCorrectModule(ICheckerContext context)
        {
            var path = "/sitecore/templates/" + Layer + "/" + Module;
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith(path)
                select Error(Msg.C1000, $"Templates should be located in the correct module '{path}'. To fix, move the template into the '{path}' module", TraceHelper.GetTextNode(template));
        }

        protected virtual bool HasLayout(Template template)
        {
            return template.StandardValuesItem != null && !string.IsNullOrEmpty(template.StandardValuesItem["__Renderings"]);
        }

        protected virtual bool IsDataSourceTemplate(Template template)
        {
            // todo: better identification of Data Source Templates
            return !template.ItemName.StartsWith("_") && !template.Fields.Any() && !HasLayout(template) && !IsRenderingParametersTemplate(template);
        }

        protected virtual bool IsDataTemplate(Template template)
        {
            return template.ItemName.StartsWith("_") && template.Fields.Any();
        }

        protected virtual bool IsPageTypeTemplate(Template template)
        {
            // todo: better identification of Page Type Templates
            return !template.ItemName.StartsWith("_") && !template.Fields.Any() && HasLayout(template);
        }

        protected virtual bool IsRenderingParametersTemplate(Template template)
        {
            return template.ItemName.StartsWith("ParametersTemplate_");
        }

        protected virtual bool IsSettingsItem(Item item)
        {
            return item.ItemIdOrPath.StartsWith("/sitecore/system/Settings", StringComparison.OrdinalIgnoreCase);
        }
    }
}
