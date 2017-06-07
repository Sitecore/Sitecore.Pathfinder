// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
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
    [Export(typeof(IFactoryService)), Shared]
    public class FactoryService : IFactoryService
    {
        [ImportingConstructor]
        public FactoryService([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public virtual BinFile BinFile(IProjectBase project, ISnapshot snapshot, string filePath) => new BinFile(project, snapshot, filePath);

        public ConfigFile ConfigFile(IProjectBase project, ISnapshot snapshot, string filePath) => new ConfigFile(project, snapshot, filePath);

        public virtual IProjectItem ContentFile(IProjectBase project, ISnapshot snapshot, string filePath) => new ContentFile(project, snapshot, filePath);

        public virtual DeviceReference DeviceReference(IProjectItem projectItem, SourceProperty<string> deviceNameSourceProperty, string databaseName) => new DeviceReference(projectItem, deviceNameSourceProperty, deviceNameSourceProperty.GetValue(), databaseName);

        public virtual Diagnostic Diagnostic(int msg, string fileName, TextSpan span, Severity severity, string text) => new Diagnostic(msg, fileName, span, severity, text);

        public virtual Field Field(Item item) => new Field(item);

        public Field Field(Item item, string fieldName, string fieldValue)
        {
            var field = new Field(item);
            field.FieldNameProperty.SetValue(fieldName);
            field.ValueProperty.SetValue(fieldValue);
            return field;
        }

        public virtual FileReference FileReference(IProjectItem owner, SourceProperty<string> sourceSourceProperty, string referenceText) => new FileReference(owner, sourceSourceProperty, referenceText);

        public virtual FileReference FileReference(IProjectItem owner, ITextNode textNode, string referenceText) => new FileReference(owner, textNode, referenceText);

        public Item Item(Database database, Guid guid, string itemName, string itemIdOrPath, string templateIdOrPath) => new Item(database, guid, itemName, itemIdOrPath, templateIdOrPath);

        public virtual ItemParseContext ItemParseContext(IParseContext context, ItemParser itemParser, Database database, string parentItemPath, bool isImport) => new ItemParseContext(context, itemParser, database, parentItemPath, isImport);

        public virtual LayoutReference LayoutReference(IProjectItem projectItem, SourceProperty<string> layoutSourceProperty, string databaseName) => new LayoutReference(projectItem, layoutSourceProperty, layoutSourceProperty.GetValue(), databaseName);

        public virtual LayoutRenderingReference LayoutRenderingReference(IProjectItem projectItem, SourceProperty<string> renderingTextNode, string databaseName) => new LayoutRenderingReference(projectItem, renderingTextNode, renderingTextNode.GetValue(), databaseName);

        public virtual MediaFile MediaFile(Database database, ISnapshot snapshot, string itemName, string itemPath, string filePath) => new MediaFile(database, snapshot, itemName, itemPath, filePath);

        public virtual ProjectOptions ProjectOptions(string databaseName) => new ProjectOptions(databaseName);

        public virtual IReference Reference(IProjectItem projectItem, SourceProperty<string> sourceSourceProperty, string referenceText, string databaseName) => new Reference(projectItem, sourceSourceProperty, referenceText, databaseName);

        public virtual IReference Reference(IProjectItem projectItem, ITextNode textNode, string referenceText, string databaseName) => new Reference(projectItem, textNode, referenceText, databaseName);

        public virtual Rendering Rendering(Database database, ISnapshot snapshot, string itemPath, string itemName, string filePath, string templateIdOrPath) => new Rendering(database, snapshot, itemPath, itemName, filePath, templateIdOrPath);

        public virtual SerializationFile SerializationFile(IProjectBase project, ISnapshot snapshot, string filePath) => new SerializationFile(project, snapshot, filePath);

        public virtual ISnapshot Snapshot(ISourceFile sourceFile) => new Snapshot().With(sourceFile);

        public virtual ISourceFile SourceFile(IFileSystemService fileSystem, string absoluteFileName) => new SourceFile(Configuration, fileSystem, absoluteFileName);

        public virtual Template Template(Database database, Guid guid, string itemName, string itemIdOrPath) => new Template(database, guid, itemName, itemIdOrPath);

        public virtual TemplateField TemplateField(Template template, Guid guid) => new TemplateField(template, guid);

        public virtual TemplateSection TemplateSection(Template template, Guid guid) => new TemplateSection(template, guid);

        public virtual TextNode TextNode(ISnapshot snapshot, TextSpan span, string name, string value) => new TextNode(snapshot, name, value, span);
    }
}

