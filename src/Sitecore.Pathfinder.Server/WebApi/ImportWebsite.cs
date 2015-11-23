// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.IO;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Json;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.WebApi
{
    public class ImportWebsite : IWebApi
    {
        [Diagnostics.NotNull]
        private static readonly Regex SafeName = new Regex("^[a-zA-Z0-9_\\-\\.]+$", RegexOptions.Compiled);

        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; private set; }

        [Diagnostics.NotNull]
        protected IFileSystemService FileSystem { get; private set; }

        [Diagnostics.NotNull]
        protected ITraceService Trace { get; private set; }

        [Diagnostics.NotNull]
        protected string WebsiteDirectory { get; private set; }

        public ActionResult Execute(IAppService app)
        {
            WebsiteDirectory = FileUtil.MapPath("/");
            FileSystem = app.CompositionService.Resolve<IFileSystemService>();
            Trace = app.CompositionService.Resolve<ITraceService>();
            Factory = app.CompositionService.Resolve<IFactoryService>();

            foreach (var pair in app.Configuration.GetSubKeys("import-website"))
            {
                var key = "import-website:" + pair.Key;

                var databaseName = app.Configuration.GetString(key + ":database");
                if (key == "import-website:exclude-fields")
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(databaseName))
                {
                    ImportItems(app, key);
                }
                else
                {
                    ImportFiles(app, key);
                }
            }

            return null;
        }

        protected virtual void BuildDevice([Diagnostics.NotNull] DeviceBuilder deviceBuilder, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] Item deviceItem, [Diagnostics.NotNull] XElement deviceElement, [Diagnostics.NotNull] [ItemNotNull] List<Item> renderingItems)
        {
            deviceBuilder.DeviceName = deviceItem.Name;
            deviceBuilder.LayoutItemPath = item.Database.GetItem(deviceElement.GetAttributeValue("l"))?.Paths.Path ?? string.Empty;

            foreach (var renderingElement in deviceElement.Elements())
            {
                var renderingItem = item.Database.GetItem(renderingElement.GetAttributeValue("id"));
                if (renderingItem == null)
                {
                    continue;
                }

                var renderingModel = new RenderingBuilder(deviceBuilder);

                ParseRendering(renderingModel, renderingItem, renderingElement, renderingItems);

                deviceBuilder.Renderings.Add(renderingModel);
            }

            foreach (var rendering in deviceBuilder.Renderings)
            {
                rendering.ParentRendering = deviceBuilder.Renderings.FirstOrDefault(r => r.Placeholders.Contains(rendering.Placeholder, StringComparer.OrdinalIgnoreCase));
                if (rendering.ParentRendering == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(rendering.Placeholder))
                {
                    continue;
                }

                // if the renderings placeholder is the parents first placeholder, omit the rendering placeholder since it is the default
                if (string.Equals(rendering.ParentRendering.Placeholders.FirstOrDefault(), rendering.Placeholder, StringComparison.OrdinalIgnoreCase))
                {
                    rendering.Placeholder = string.Empty;
                }
            }
        }

        [Diagnostics.NotNull]
        protected virtual Projects.Items.Item BuildItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] [ItemNotNull] IEnumerable<string> excludedFields, [Diagnostics.NotNull] string format)
        {
            var itemBuilder = new ItemBuilder(Factory)
            {
                DatabaseName = item.Database.Name,
                Guid = item.ID.ToString(),
                ItemName = item.Name,
                TemplateIdOrPath = item.Template.InnerItem.Paths.Path,
                ItemIdOrPath = item.Paths.Path
            };

            var versions = item.Versions.GetVersions(true);
            var sharedFields = item.Fields.Where(f => f.Shared && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value)).ToList();
            var unversionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();
            var versionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && !f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();

            // shared fields
            foreach (var field in sharedFields.OrderBy(f => f.Name))
            {
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

                if (field.Name == "__Renderings")
                {
                    var layoutBuilder = new LayoutBuilder();
                    BuildLayout(layoutBuilder, item, value);

                    var stringWriter = new StringWriter();
                    switch (format.ToLowerInvariant())
                    {
                        case "item.json":
                            layoutBuilder.WriteAsJson(stringWriter);
                            break;
                        case "item.xml":
                            layoutBuilder.WriteAsXml(stringWriter);
                            break;
                        case "item.yaml":
                            layoutBuilder.WriteAsYaml(stringWriter);
                            break;
                    }

                    value = stringWriter.ToString();
                }

                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // unversioned fields
            foreach (var field in unversionedFields.OrderBy(f => f.Name))
            {
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // versioned fields
            foreach (var field in versionedFields.OrderBy(f => f.Name))
            {
                var value = field.Value;
                if (Data.ID.IsID(value))
                {
                    var i = item.Database.GetItem(value);
                    if (i != null)
                    {
                        value = i.Paths.Path;
                    }
                }

                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name,
                    Version = field.Item.Version.Number
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            return itemBuilder.Build(project, TextNode.Empty);
        }

        protected virtual void BuildLayout([Diagnostics.NotNull] LayoutBuilder layoutBuilder, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string layout)
        {
            if (string.IsNullOrEmpty(layout))
            {
                return;
            }

            var layoutElement = layout.ToXElement();
            if (layoutElement == null)
            {
                return;
            }

            var renderingItems = item.Database.GetItemsByTemplate(ServerConstants.Renderings.ViewRenderingId, TemplateIDs.XSLRendering, TemplateIDs.Sublayout, ServerConstants.Renderings.WebcontrolRendering, ServerConstants.Renderings.UrlRendering, ServerConstants.Renderings.MethodRendering).GroupBy(i => i.Name).Select(i => i.First()).OrderBy(i => i.Name).ToList();

            foreach (var deviceElement in layoutElement.Elements())
            {
                var deviceId = deviceElement.GetAttributeValue("id");
                if (string.IsNullOrEmpty(deviceId))
                {
                    continue;
                }

                var deviceItem = item.Database.GetItem(deviceId);
                if (deviceItem == null)
                {
                    continue;
                }

                var deviceModel = new DeviceBuilder(layoutBuilder);

                BuildDevice(deviceModel, item, deviceItem, deviceElement, renderingItems);

                layoutBuilder.Devices.Add(deviceModel);
            }
        }

        [Diagnostics.NotNull]
        protected virtual Template BuildTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item)
        {
            var templateItem = new TemplateItem(item);

            var templateBuilder = new TemplateBuilder(Factory);
            templateBuilder.DatabaseName = templateItem.Database.Name;
            templateBuilder.Guid = templateItem.ID.ToString();
            templateBuilder.TemplateName = templateItem.Name;
            templateBuilder.ItemIdOrPath = templateItem.InnerItem.Paths.Path;
            templateBuilder.Icon = templateItem.InnerItem.Appearance.Icon;

            foreach (var templateSectionItem in templateItem.GetSections())
            {
                var templateSectionBuilder = new TemplateSectionBuilder(Factory).With(templateBuilder, TextNode.Empty);
                templateSectionBuilder.SectionId = templateSectionItem.ID.ToString();
                templateSectionBuilder.SectionName = templateSectionItem.Name;

                foreach (var templateFieldItem in templateSectionItem.GetFields())
                {
                    var templateFieldBuilder = new TemplateFieldBuilder(Factory).With(templateSectionBuilder, TextNode.Empty);
                    templateFieldBuilder.FieldId = templateFieldItem.ID.ToString();
                    templateFieldBuilder.FieldName = templateFieldItem.Name;
                    templateFieldBuilder.Source = templateFieldItem.Source;
                    templateFieldBuilder.Type = templateFieldItem.Type;

                    templateSectionBuilder.Fields.Add(templateFieldBuilder);
                }

                templateBuilder.Sections.Add(templateSectionBuilder);
            }

            return templateBuilder.Build(project, TextNode.Empty);
        }

        [NotNull]
        protected virtual string GetUniqueRenderingName([NotNull] [ItemNotNull] IEnumerable<Item> duplicates, [Diagnostics.NotNull] Item renderingItem)
        {
            var paths = duplicates.Where(r => r.Name == renderingItem.Name && r.ID != renderingItem.ID).Select(r => r.Paths.Path).ToList();
            var parts = renderingItem.Paths.Path.Split(Constants.Slash, StringSplitOptions.RemoveEmptyEntries);

            var result = string.Empty;

            for (var i = parts.Length - 1; i >= 0; i--)
            {
                result = "/" + parts[i] + result;

                var r = result;
                if (!paths.Any(p => p.EndsWith(r, StringComparison.InvariantCultureIgnoreCase)))
                {
                    break;
                }
            }

            result = result.Mid(1).Replace("/", ".");

            return result;
        }

        protected virtual void ImportFiles([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var sourceDirectory = PathHelper.NormalizeFilePath(Path.Combine(WebsiteDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":website-directory")).TrimStart('\\'))).TrimEnd('\\');
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":project-directory")).TrimStart('\\'))).TrimEnd('\\');

            if (!FileSystem.DirectoryExists(sourceDirectory))
            {
                return;
            }

            var searchPattern = app.Configuration.GetString(key + ":search-pattern", "*");
            var include = app.Configuration.GetString(key + ":include");
            var exclude = app.Configuration.GetString(key + ":exclude");

            IEnumerable<string> fileNames;

            var fileNameList = app.Configuration.GetList(key + ":file-names").ToList();
            if (fileNameList.Any())
            {
                fileNames = fileNameList.Select(f => Path.Combine(sourceDirectory, PathHelper.NormalizeFilePath(f).TrimStart('\\')));
            }
            else if (!string.IsNullOrEmpty(include) || !string.IsNullOrEmpty(exclude))
            {
                var pathMatcher = new PathMatcher(include, exclude);
                fileNames = FileSystem.GetFiles(sourceDirectory, searchPattern, SearchOption.AllDirectories).Where(f => pathMatcher.IsMatch(f)).ToList();
            }
            else
            {
                fileNames = FileSystem.GetFiles(sourceDirectory, searchPattern, SearchOption.AllDirectories);
            }

            FileSystem.CreateDirectory(projectDirectory);
            foreach (var fileName in fileNames)
            {
                if (!FileSystem.FileExists(fileName))
                {
                    continue;
                }

                var targetFileName = Path.Combine(projectDirectory, PathHelper.UnmapPath(sourceDirectory, fileName));
                try
                {
                    FileSystem.Copy(fileName, targetFileName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(Msg.M1022, ex.Message, fileName);
                }
            }
        }

        protected virtual void ImportItems([Diagnostics.NotNull] IAppService app, [Diagnostics.NotNull] string key)
        {
            var project = app.CompositionService.Resolve<IProject>();

            var excludedFields = app.Configuration.GetList("import-website:exclude-fields");

            var databaseName = app.Configuration.GetString(key + ":database");
            var rootItemPath = app.Configuration.GetString(key + ":root-item-path").TrimEnd('/');
            var queryText = app.Configuration.GetString(key + ":query");
            var useDirectories = app.Configuration.GetBool(key + ":use-directories", true);
            var format = app.Configuration.GetString(key + ":format", "item.xml");
            var projectDirectory = PathHelper.NormalizeFilePath(Path.Combine(app.ProjectDirectory, PathHelper.NormalizeFilePath(app.Configuration.GetString(key + ":project-directory")).TrimStart('\\'))).TrimEnd('\\');

            var database = Sitecore.Configuration.Factory.GetDatabase(databaseName);
            using (new SecurityDisabler())
            {
                foreach (var item in database.Query(queryText))
                {
                    var fileName = item.Paths.Path;
                    if (!fileName.StartsWith(rootItemPath))
                    {
                        Trace.TraceError(Texts.Item_is_not_in_root_item_path__Skipping_, fileName);
                        return;
                    }

                    fileName = useDirectories ? fileName.Mid(rootItemPath.Length).TrimStart('/') : item.Name.GetSafeCodeIdentifier();
                    fileName = Path.Combine(projectDirectory, PathHelper.NormalizeFilePath(fileName)) + "." + format;

                    FileSystem.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
                    if (item.Template.ID == TemplateIDs.Template)
                    {
                        WriteTemplate(project, item, fileName, format);
                    }
                    else
                    {
                        WriteItem(project, item, fileName, format, excludedFields);
                    }
                }
            }
        }

        protected virtual void ParseRendering([Diagnostics.NotNull] RenderingBuilder renderingBuilder, [Diagnostics.NotNull] Item renderingItem, [Diagnostics.NotNull] XElement renderingElement, [Diagnostics.NotNull] [ItemNotNull] List<Item> renderingItems)
        {
            var parameters = new UrlString(renderingElement.GetAttributeValue("par"));
            renderingBuilder.Id = parameters.Parameters["Id"];
            renderingBuilder.Placeholder = renderingElement.GetAttributeValue("ph");

            foreach (var placeholder in renderingItem["Place Holders"].Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries))
            {
                renderingBuilder.Placeholders.Add(placeholder.Replace("$Id", renderingBuilder.Id).Trim());
            }

            var fields = new Dictionary<string, Data.Templates.TemplateField>();

            var parametersTemplateItem = renderingItem.Database.GetItem(renderingItem["Parameters Template"]);
            if (parametersTemplateItem != null)
            {
                var template = TemplateManager.GetTemplate(parametersTemplateItem.ID, renderingItem.Database);
                if (template != null)
                {
                    foreach (var field in template.GetFields(true))
                    {
                        if (field.Template.BaseIDs.Length != 0)
                        {
                            fields[field.Name.ToLowerInvariant()] = field;
                        }
                    }
                }
            }

            // rendering name
            var name = renderingItem.Name;

            var duplicates = renderingItems.Where(i => i.Name == renderingItem.Name).ToList();
            if (duplicates.Count > 1)
            {
                name = GetUniqueRenderingName(duplicates, renderingItem);
            }

            renderingBuilder.UnsafeName = !SafeName.IsMatch(name);
            renderingBuilder.Name = name;

            // parameters
            foreach (var key in parameters.Parameters.Keys.OfType<string>().Where(k => !string.IsNullOrEmpty(k)).OrderBy(k => k))
            {
                var value = parameters.Parameters[key];
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                value = HttpUtility.UrlDecode(value) ?? string.Empty;

                Data.Templates.TemplateField field;
                if (fields.TryGetValue(key.ToLowerInvariant(), out field))
                {
                    switch (field.Type.ToLowerInvariant())
                    {
                        case "checkbox":
                            if (!value.StartsWith("{Binding"))
                            {
                                value = MainUtil.GetBool(value, false) ? "True" : "False";
                            }

                            break;

                        case "droptree":
                            if (Data.ID.IsID(value))
                            {
                                var i = renderingItem.Database.GetItem(value);
                                if (i != null)
                                {
                                    value = i.Paths.Path;
                                }
                            }

                            break;
                    }

                    var source = new Sitecore.Text.UrlString(field.Source);
                    var defaultValue = source.Parameters["defaultvalue"] ?? string.Empty;

                    if (string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                // todo: Hacky, hacky, hacky
                if ((key == "IsEnabled" || key == "IsVisible") && value == "True")
                {
                    continue;
                }

                renderingBuilder.Attributes[key] = value;
            }

            // data source
            var dataSource = renderingElement.GetAttributeValue("ds");
            if (Data.ID.IsID(dataSource))
            {
                var dataSourceItem = renderingItem.Database.GetItem(dataSource);
                if (dataSourceItem != null)
                {
                    dataSource = dataSourceItem.Paths.Path;
                }
            }

            renderingBuilder.DataSource = dataSource;

            // caching
            renderingBuilder.Cacheable = renderingElement.GetAttributeValue("cac") == @"1";
            renderingBuilder.VaryByData = renderingElement.GetAttributeValue("vbd") == @"1";
            renderingBuilder.VaryByDevice = renderingElement.GetAttributeValue("vbdev") == @"1";
            renderingBuilder.VaryByLogin = renderingElement.GetAttributeValue("vbl") == @"1";
            renderingBuilder.VaryByParameters = renderingElement.GetAttributeValue("vbp") == @"1";
            renderingBuilder.VaryByQueryString = renderingElement.GetAttributeValue("vbqs") == @"1";
            renderingBuilder.VaryByUser = renderingElement.GetAttributeValue("vbu") == @"1";
        }

        protected virtual void WriteItem([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string format, [Diagnostics.NotNull] [ItemNotNull] IEnumerable<string> excludedFields)
        {
            var itemToWrite = BuildItem(project, item, excludedFields, format);

            using (var stream = new StreamWriter(fileName))
            {
                switch (format.ToLowerInvariant())
                {
                    case "item.json":
                        itemToWrite.WriteAsJson(stream);
                        break;
                    case "item.xml":
                        itemToWrite.WriteAsXml(stream);
                        break;
                    case "item.yaml":
                        itemToWrite.WriteAsYaml(stream);
                        break;
                }
            }

            // write media file
            if (!string.IsNullOrEmpty(item["Blob"]))
            {
                var mediaItem = new MediaItem(item);
                var mediaFileName = Path.ChangeExtension(fileName, mediaItem.Extension);

                using (var stream = new FileStream(mediaFileName, FileMode.Create))
                {
                    FileUtil.CopyStream(mediaItem.GetMediaStream(), stream);
                }
            }
        }

        protected virtual void WriteTemplate([Diagnostics.NotNull] IProject project, [Diagnostics.NotNull] Item item, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string format)
        {
            var templateToWrite = BuildTemplate(project, item);

            using (var stream = new StreamWriter(fileName))
            {
                switch (format.ToLowerInvariant())
                {
                    case "item.json":
                        templateToWrite.WriteAsJson(stream);
                        break;
                    case "item.xml":
                        templateToWrite.WriteAsXml(stream);
                        break;
                    case "item.yaml":
                        templateToWrite.WriteAsYaml(stream);
                        break;
                }
            }
        }
    }
}
