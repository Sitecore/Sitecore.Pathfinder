// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Languages.Content;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Languages.Serialization;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Configuration
{
    [Export(typeof(IFactoryService))]
    public class FactoryService : IFactoryService
    {
        [ImportingConstructor]
        public FactoryService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService)
        {
            Configuration = configuration;
            CompositionService = compositionService;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual BinFile BinFile(IProjectBase project, ISnapshot snapshot, string filePath) => new BinFile(project, snapshot, filePath);

        public ConfigFile ConfigFile(IProjectBase project, ISnapshot snapshot, string filePath) => new ConfigFile(project, snapshot, filePath);

        public virtual IProjectItem ContentFile(IProjectBase project, ISnapshot snapshot, string filePath) => new ContentFile(project, snapshot, filePath);

        public virtual DeviceReference DeviceReference(IProjectItem projectItem, SourceProperty<string> deviceNameSourceProperty) => new DeviceReference(projectItem, deviceNameSourceProperty, deviceNameSourceProperty.GetValue());

        public virtual Diagnostic Diagnostic(int msg, string fileName, TextSpan span, Severity severity, string text) => new Diagnostic(msg, fileName, span, severity, text);

        public virtual Field Field(Item item) => new Field(item);

        public Field Field(Item item, string fieldName, string fieldValue)
        {
            var field = new Field(item);
            field.FieldNameProperty.SetValue(fieldName);
            field.ValueProperty.SetValue(fieldValue);
            return field;
        }

        public FieldBuilder FieldBuilder() => new FieldBuilder(this);

        public virtual FileReference FileReference(IProjectItem owner, SourceProperty<string> sourceSourceProperty, string referenceText) => new FileReference(owner, sourceSourceProperty, referenceText);

        public virtual FileReference FileReference(IProjectItem owner, ITextNode textNode, string referenceText) => new FileReference(owner, textNode, referenceText);

        public Item Item(IProjectBase project, Guid guid, string databaseName, string itemName, string itemIdOrPath, string templateIdOrPath) => new Item(project, guid, databaseName, itemName, itemIdOrPath, templateIdOrPath);

        public ItemBuilder ItemBuilder() => new ItemBuilder(this);

        public virtual ItemParseContext ItemParseContext(IParseContext context, ItemParser itemParser, string databaseName, string parentItemPath, bool isImport) => new ItemParseContext(context, itemParser, databaseName, parentItemPath, isImport);

        public virtual LayoutReference LayoutReference(IProjectItem projectItem, SourceProperty<string> layoutSourceProperty) => new LayoutReference(projectItem, layoutSourceProperty, layoutSourceProperty.GetValue());

        public virtual LayoutRenderingReference LayoutRenderingReference(IProjectItem projectItem, SourceProperty<string> renderingTextNode) => new LayoutRenderingReference(projectItem, renderingTextNode, renderingTextNode.GetValue());

        public virtual MediaFile MediaFile(IProjectBase project, ISnapshot snapshot, string databaseName, string itemName, string itemPath, string filePath) => new MediaFile(project, snapshot, databaseName, itemName, itemPath, filePath);

        public virtual ProjectOptions ProjectOptions(string projectDirectory, string databaseName) => new ProjectOptions(projectDirectory, databaseName);

        public virtual IReference Reference(IProjectItem projectItem, SourceProperty<string> sourceSourceProperty, string referenceText) => new Reference(projectItem, sourceSourceProperty, referenceText);

        public virtual IReference Reference(IProjectItem projectItem, ITextNode textNode, string referenceText) => new Reference(projectItem, textNode, referenceText);

        public virtual Rendering Rendering(IProjectBase project, ISnapshot snapshot, string databaseName, string itemPath, string itemName, string filePath, string templateIdOrPath) => new Rendering(project, snapshot, databaseName, itemPath, itemName, filePath, templateIdOrPath);

        public virtual SerializationFile SerializationFile(IProjectBase project, ISnapshot snapshot, string filePath) => new SerializationFile(project, snapshot, filePath);

        public virtual ISnapshot Snapshot(ISourceFile sourceFile) => new Snapshot().With(sourceFile);

        public virtual ISourceFile SourceFile(IFileSystemService fileSystem, string absoluteFileName)
        {
            var projectDirectory = Configuration.GetProjectDirectory();

            var relativeFileName = PathHelper.NormalizeFilePath(PathHelper.UnmapPath(projectDirectory, absoluteFileName)).TrimStart('\\');
            var projectFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(projectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(absoluteFileName))).TrimStart('/');

            return new SourceFile(fileSystem, absoluteFileName, relativeFileName, projectFileName);
        }

        public virtual Template Template(IProjectBase project, Guid guid, string databaseName, string itemName, string itemIdOrPath) => new Template(project, guid, databaseName, itemName, itemIdOrPath);

        public virtual TemplateField TemplateField(Template template, Guid guid) => new TemplateField(template, guid);

        public virtual TemplateSection TemplateSection(Template template, Guid guid) => new TemplateSection(template, guid);

        public virtual TextNode TextNode(ISnapshot snapshot, TextSpan span, string name, string value) => new TextNode(snapshot, name, value, span);
    }
}
