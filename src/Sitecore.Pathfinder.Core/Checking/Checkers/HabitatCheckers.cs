// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class HabitatCheckers : Checker
    {
        protected static readonly Guid StandardRenderingParametersGuid = new Guid("{8CA06D6A-B353-44E8-BC31-B528C7306971}");

        [CanBeNull]
        private string _layerName;

        [CanBeNull]
        private string _moduleName;

        [ImportingConstructor]
        public HabitatCheckers([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected string Layer => _layerName ?? (_layerName = Configuration.GetString("habitat:layer"));

        [NotNull]
        protected string Module => _moduleName ?? (_moduleName = Configuration.GetString("habitat:module"));

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> AvoidUsingFolderTemplate([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName == "Folder"
                select Warning(context, Msg.C1068, "Avoid using the 'Folder' template. To fix, create a new 'Folder' template, assign Insert Options and change the template of this item", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> ConfigFileMustBeInCorrectModuleDirectory([NotNull] ICheckerContext context)
        {
            var path = "/App_Config/include/" + Layer + "/" + Module;

            return from configFile in context.Project.ProjectItems.OfType<ConfigFile>()
                where !configFile.FilePath.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                select Error(context, Msg.C1069, "Config file must be the correct module directory", configFile.Snapshot.SourceFile, $"To fix, move the file to the '{path}' directory");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> ControllerRenderingNotAllowedInLayer([NotNull] ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-controller-rendering"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from item in context.Project.Items
                where item.TemplateName == "Controller"
                select Error(context, Msg.C1070, $"Controller renderings are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataSourceTemplatesMustInheritFromDataTemplates([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template)
                from baseTemplate in template.GetBaseTemplates()
                where baseTemplate.ItemName != "Standard template" && !IsDataTemplate(baseTemplate)
                select Error(context, Msg.C1071, $"Data Source templates must not inherit from {baseTemplate.ItemName} as it is not a Data Template. To fix, either remove the inheritance or make the base template a Data Template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataSourceTemplatesMustNotFields([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template) && template.Fields.Any()
                select Error(context, Msg.C1072, "Page Type Templates must not have fields. To fix, move the fields to a Data Template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataSourceTemplatesMustNotHaveLayout([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataSourceTemplate(template) && HasLayout(template)
                select Error(context, Msg.C1073, "Data Source Templates must not have a layout. To fix, remove the layout from the template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataTemplatesMustNotBeInstantiated([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where IsDataTemplate(item.Template)
                select Error(context, Msg.C1074, "Data Templates must not be instantiated. To fix, change the template of the item to a Page Type template or Data Source template", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataTemplatesMustNotHaveLayout([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsDataTemplate(template) && HasLayout(template)
                select Error(context, Msg.C1075, "Data Templates must not have a layout. To fix, create a Page Type Template that inherits from this template and assign the layout to that template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> DataTemplatesNotAllowedInLayer([NotNull] ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-data-templates"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from template in context.Project.Templates
                where IsDataTemplate(template)
                select Error(context, Msg.C1076, $"Data Templates are not allowed in the '{Layer}' layer. To fix, move the template to another layer", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> FolderTemplatesMustHaveFolderPostfix([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !IsDataSourceTemplate(template) && !IsPageTypeTemplate(template) && !IsRenderingParametersTemplate(template) && !template.Fields.Any() && !template.ItemName.EndsWith("Folder")
                select Warning(context, Msg.C1077, $"Templates without fields should have the 'Folder' postfix. To fix, rename the template to '{template.ItemName}Folder'", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> FolderTemplatesMustNotHaveFields([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where template.ItemName.EndsWith("Folder") && template.Fields.Any()
                select Error(context, Msg.C1078, "Folder templates must not have any fields. To fix, remove the fields", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> FolderTemplatesShouldHaveInsertOptions([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where template.ItemName.EndsWith("Folder") && template.StandardValuesItem != null && string.IsNullOrEmpty(template.StandardValuesItem["__Masters"])
                select Error(context, Msg.C1079, "Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> LayoutMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Layouts/" + Layer;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.Indexes.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Layouts/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1080, $"Layout items should be located in the correct layer '{path}'. To fix, move the layout item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> LayoutMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Layouts/" + Layer + "/" + Module;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.Indexes.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Layouts/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1081, $"Layout items should be located in the correct module '{path}'. To fix, move the layout item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> MissingCodeDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/code"))
            {
                yield return Warning(context, Msg.C1082, "The root directory should have a 'code' subdirectory. To fix, create the ~/code directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> MissingSerializationDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/serialization"))
            {
                yield return Warning(context, Msg.C1083, "The root directory should have a 'serialization' subdirectory. To fix, create the ~/serialization directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> MissingTestsDirectory([NotNull] ICheckerContext context)
        {
            if (!DirectoryExists(context, "~/tests"))
            {
                yield return Warning(context, Msg.C1084, "The root directory should have a 'tests' subdirectory. To fix, create the ~/tests directory");
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> ModelsMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Models/" + Layer;
            return from item in context.Project.Items
                where item.TemplateName == "Model" && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1085, $"Model items should be located in the correct layer '{path}'. To fix, move the Model item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> ModelsMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Models/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where item.TemplateName == "Model" && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1086, $"Model items should be located in the correct module '{path}'. To fix, move the Model item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PageTypeTemplatesMustHaveLayout([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && !HasLayout(template)
                select Error(context, Msg.C1087, "Page Type Templates must have a layout. To fix, assign a layout to the template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PageTypeTemplatesMustInheritFromDataTemplates([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template)
                from baseTemplate in template.GetBaseTemplates()
                where baseTemplate.ItemName != "Standard Template" && !IsDataTemplate(baseTemplate)
                select Error(context, Msg.C1088, $"Page Type templates cannot inherit from {baseTemplate.ItemName} as it is not a Data Template. Fix fix, either remove the inheritance or make the base template a Data Template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PageTypeTemplatesMustNotHaveFields([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && template.Fields.Any()
                select Error(context, Msg.C1089, "Page Type Templates must not have fields. To fix, move the fields to a Data Template", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PageTypeTemplatesNotAllowedInLayer([NotNull] ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-page-type-templates"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from template in context.Project.Templates
                where IsPageTypeTemplate(template)
                select Error(context, Msg.C1090, $"Page Type templates are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PageTypeTemplatesShouldHaveStandardValues([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where IsPageTypeTemplate(template) && template.StandardValuesItem == null
                select Error(context, Msg.C1091, "Page Type templates should have a Standard Values item. To fix, create a Standard Value item", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PlaceholderSettingsMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Placeholder Settings/" + Layer;
            return from item in context.Project.Items
                where item.TemplateName == "Placeholder" && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1092, $"Placeholder Settings items should be located in the correct layer '{path}'. To fix, move the Placeholder Settings item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> PlaceholderSettingsMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Placeholder Settings/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where item.TemplateName == "Placeholder" && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1093, $"Placeholder Settings items should be located in the correct module '{path}'. To fix, move the Placeholder Settings item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> RenderingParametersTemplateMustDeriveFromStandardRenderingParameters([NotNull] ICheckerContext context)
        {
            var standardRenderingParametersTemplate = context.Project.ProjectItems.OfType<Template>().FirstOrDefault(t => t.Uri.Guid == StandardRenderingParametersGuid);
            if (standardRenderingParametersTemplate == null)
            {
                return new[]
                {
                    Error(context, Msg.C1094, "'StandardRenderingParameters' template not found. Are you missing an import?", SourceFile.Empty)
                };
            }

            return from template in context.Project.Templates
                where IsRenderingParametersTemplate(template) && !template.Is(standardRenderingParametersTemplate)
                select Error(context, Msg.C1095, "Folder templates should specify Insert Options on their Standard Values item. To fix, assign appropriate Insert Options to the Standard Values item", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> RenderingShouldBeInViewsFolder([NotNull] ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                where !rendering.FilePath.StartsWith("~/views/", StringComparison.OrdinalIgnoreCase)
                select Warning(context, Msg.C1096, "View rendering should be located in the ~/views/ directory", rendering.Snapshot.SourceFile, "To fix, move the file to the ~/views/ directory");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> RenderingsMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Renderings/" + Layer;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.Indexes.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Renderings/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1097, $"Rendering items should be located in the correct layer '{path}'. To fix, move the rendering item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> RenderingsMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/layout/Renderings/" + Layer + "/" + Module;
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                let item = context.Project.Indexes.FindQualifiedItem<Item>(rendering.RenderingItemUri)
                where item != null && item.ItemIdOrPath.StartsWith("/sitecore/layout/Renderings/") && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1098, $"Rendering items should be located in the correct module '{path}'. To fix, move the rendering item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> RootDirectoryNameDoesNotMatchModuleName([NotNull] ICheckerContext context)
        {
            if (Path.GetFileName(Directory.GetCurrentDirectory()) != Module)
            {
                yield return Warning(context, Msg.C1099, "The root directory name should match the module name. To fix, either rename the current directory or change the 'module' configuration");
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> SettingsItemsNotAllowedInLayer([NotNull] ICheckerContext context)
        {
            if (Configuration.GetBool("habitat:" + Layer.ToLowerInvariant() + ":allow-settings-items"))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return from item in context.Project.Items
                where IsSettingsItem(item)
                select Error(context, Msg.C1100, $"Settings items are not allowed in the '{Layer}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> SettingsMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/system/Settings/" + Layer;
            return from item in context.Project.Items
                where IsSettingsItem(item) && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1101, $"Settings items should be located in the correct layer '{path}'. To fix, move the settings item into the '{path}' layer", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> SettingsMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/system/Settings/" + Layer + "/" + Module;
            return from item in context.Project.Items
                where IsSettingsItem(item) && !item.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1102, $"Settings items should be located in the correct module '{path}'. To fix, move the settings item into the '{path}' module", TraceHelper.GetTextNode(item));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateMustBeInCorrectLayer([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/templates/" + Layer;
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1103, $"Templates should be located in the correct layer '{path}'. To fix, move the template into the '{path}' layer", TraceHelper.GetTextNode(template));
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<IDiagnostic> TemplateMustBeInCorrectModule([NotNull] ICheckerContext context)
        {
            var path = "/sitecore/templates/" + Layer + "/" + Module;
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith(path)
                select Error(context, Msg.C1104, $"Templates should be located in the correct module '{path}'. To fix, move the template into the '{path}' module", TraceHelper.GetTextNode(template));
        }

        protected virtual bool HasLayout([NotNull] Template template)
        {
            return template.StandardValuesItem != null && !string.IsNullOrEmpty(template.StandardValuesItem["__Renderings"]);
        }

        protected virtual bool IsDataSourceTemplate([NotNull] Template template)
        {
            // todo: better identification of Data Source Templates
            return !template.ItemName.StartsWith("_") && !template.Fields.Any() && !HasLayout(template) && !IsRenderingParametersTemplate(template);
        }

        protected virtual bool IsDataTemplate([NotNull] Template template)
        {
            return template.ItemName.StartsWith("_") && template.Fields.Any();
        }

        protected virtual bool IsPageTypeTemplate([NotNull] Template template)
        {
            // todo: better identification of Page Type Templates
            return !template.ItemName.StartsWith("_") && !template.Fields.Any() && HasLayout(template);
        }

        protected virtual bool IsRenderingParametersTemplate([NotNull] Template template)
        {
            return template.ItemName.StartsWith("ParametersTemplate_");
        }

        protected virtual bool IsSettingsItem([NotNull] Item item)
        {
            return item.ItemIdOrPath.StartsWith("/sitecore/system/Settings", StringComparison.OrdinalIgnoreCase);
        }
    }
}
