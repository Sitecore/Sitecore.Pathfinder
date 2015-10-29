// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Parsing.Items;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Layouts;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Configuration
{
    [Export(typeof(IFactoryService))]
    public class FactoryService : IFactoryService
    {
        [ImportingConstructor]
        public FactoryService([NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public virtual BinFile BinFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new BinFile(project, snapshot, filePath);
        }

        public virtual IProjectItem ContentFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new ContentFile(project, snapshot, filePath);
        }

        public virtual DeviceReference DeviceReference(IProjectItem projectItem, SourceProperty<string> deviceNameSourceProperty)
        {
            return new DeviceReference(projectItem, deviceNameSourceProperty);
        }

        public virtual Diagnostic Diagnostic(string fileName, TextSpan span, Severity severity, string text)
        {
            return new Diagnostic(fileName, span, severity, text);
        }

        public virtual Field Field(Item item, ITextNode textNode)
        {
            return new Field(item, textNode);
        }

        public Field Field(Item item, ITextNode textNode, string fieldName, string fieldValue)
        {
            var field = new Field(item, textNode);
            field.FieldNameProperty.SetValue(fieldName);
            field.ValueProperty.SetValue(fieldValue);
            return field;
        }

        public virtual FileReference FileReference(IProjectItem owner, SourceProperty<string> sourceSourceProperty)
        {
            return new FileReference(owner, sourceSourceProperty);
        }

        public virtual Item Item(IProject project, Guid guid, ITextNode textNode, string databaseName, string itemName, string itemIdOrPath, string templateIdOrPath)
        {
            return new Item(project, guid, textNode, databaseName, itemName, itemIdOrPath, templateIdOrPath);
        }

        public ItemBuilder ItemBuilder()
        {
            return CompositionService.Resolve<ItemBuilder>();
        }

        public FieldBuilder FieldBuilder()
        {
            return CompositionService.Resolve<FieldBuilder>();
        }

        public virtual ItemParseContext ItemParseContext(IParseContext context, ItemParser itemParser, string databaseName, string parentItemPath, bool isExtern)
        {
            return new ItemParseContext(context, itemParser, databaseName, parentItemPath, isExtern);
        }

        public virtual LayoutReference LayoutReference(IProjectItem projectItem, SourceProperty<string> layoutSourceProperty)
        {
            return new LayoutReference(projectItem, layoutSourceProperty);
        }

        public virtual LayoutRenderingReference LayoutRenderingReference(IProjectItem projectItem, SourceProperty<string> renderingTextNode)
        {
            return new LayoutRenderingReference(projectItem, renderingTextNode);
        }

        public virtual MediaFile MediaFile(IProject project, ISnapshot snapshot, string databaseName, string itemName, string itemPath, string filePath)
        {
            return new MediaFile(project, snapshot, databaseName, itemName, itemPath, filePath);
        }

        public virtual IProject Project(ProjectOptions projectOptions, List<string> sourceFileNames)
        {
            return CompositionService.Resolve<IProject>().Load(projectOptions, sourceFileNames);
        }

        public virtual ProjectOptions ProjectOptions(string projectDirectory, string databaseName)
        {
            return new ProjectOptions(projectDirectory, databaseName);
        }

        public virtual IReference Reference(IProjectItem projectItem, SourceProperty<string> sourceSourceProperty)
        {
            return new Reference(projectItem, sourceSourceProperty);
        }

        public virtual IReference Reference(IProjectItem projectItem, SourceProperty<string> sourceSourceProperty, string referenceText)
        {
            return new Reference(projectItem, sourceSourceProperty, referenceText);
        }

        public virtual Rendering Rendering(IProject project, ISnapshot snapshot, string databaseName, string itemPath, string itemName, string filePath, string templateIdOrPath)
        {
            return new Rendering(project, snapshot, databaseName, itemPath, itemName, filePath, templateIdOrPath);
        }

        public virtual SerializationFile SerializationFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new SerializationFile(project, snapshot, filePath);
        }

        public virtual ISnapshot Snapshot(ISourceFile sourceFile)
        {
            var snapshot = CompositionService.Resolve<Snapshot>().With(sourceFile);
            return snapshot;
        }

        public virtual ISourceFile SourceFile(IFileSystemService fileSystem, string sourceFileName, string projectFileName)
        {
            return new SourceFile(fileSystem, sourceFileName, projectFileName);
        }

        public virtual Template Template(IProject project, Guid guid, ITextNode textNode, string databaseName, string itemName, string itemIdOrPath)
        {
            return new Template(project, guid, textNode, databaseName, itemName, itemIdOrPath);
        }

        public virtual TemplateField TemplateField(Template template, ITextNode templateFieldTextNode)
        {
            return new TemplateField(template, templateFieldTextNode);
        }

        public virtual TemplateSection TemplateSection(ITextNode templateSectionTextNode)
        {
            return new TemplateSection(templateSectionTextNode);
        }

        public virtual TextNode TextNode(ISnapshot snapshot, TextSpan span, string name, string value)
        {
            return new TextNode(snapshot, name, value, span);
        }
    }
}
