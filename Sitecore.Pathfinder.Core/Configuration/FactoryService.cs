// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
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

        [NotNull]
        public virtual BinFile BinFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new BinFile(project, snapshot, filePath);
        }

        public virtual IProjectItem ContentFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new ContentFile(project, snapshot, filePath);
        }

        public virtual DeviceReference DeviceReference(IProjectItem projectItem, Attribute<string> deviceNameAttribute, string targetQualifiedName)
        {
            return new DeviceReference(projectItem, deviceNameAttribute, targetQualifiedName);
        }

        public virtual Diagnostic Diagnostic(string fileName, TextPosition position, Severity severity, string text)
        {
            return new Diagnostic(fileName, position, severity, text);
        }

        public virtual ExternalReferenceItem ExternalReferenceItem(IProject project, string externalReference, ISnapshot snapshot, string databaseName, string itemName, string itemIdOrPath)
        {
            return new ExternalReferenceItem(project, externalReference, snapshot, databaseName, itemName, itemIdOrPath);
        }

        public virtual Field Field(Item item, string fieldName, string language, int version, string value, string valueHint = "")
        {
            return new Field(item, fieldName, language, version, value, valueHint);
        }

        public virtual FileReference FileReference(IProjectItem owner, Attribute<string> sourceAttribute, string targetQualifiedName)
        {
            return new FileReference(owner, sourceAttribute, targetQualifiedName);
        }

        public virtual Item Item(IProject project, string itemPath, ITextNode textNode, string databaseName, string itemName, string itemIdOrPath, string templateIdOrPath)
        {
            return new Item(project, itemPath, textNode, databaseName, itemName, itemIdOrPath, templateIdOrPath);
        }

        public virtual ItemParseContext ItemParseContext(IParseContext context, ItemParser itemParser, string parentItemPath)
        {
            return new ItemParseContext(context, itemParser, parentItemPath);
        }

        public virtual LayoutReference LayoutReference(IProjectItem projectItem, Attribute<string> layoutAttribute, string targetQualifiedName)
        {
            return new LayoutReference(projectItem, layoutAttribute, targetQualifiedName);
        }

        public virtual LayoutRenderingReference LayoutRenderingReference(IProjectItem projectItem, Attribute<string> renderingTextNode, string targetQualifiedName)
        {
            return new LayoutRenderingReference(projectItem, renderingTextNode, targetQualifiedName);
        }

        public virtual MediaFile MediaFile(IProject project, ISnapshot snapshot, string filePath, Item mediaItem)
        {
            return new MediaFile(project, snapshot, filePath, mediaItem);
        }

        public virtual IProject Project(ProjectOptions projectOptions, List<string> sourceFileNames)
        {
            return CompositionService.Resolve<IProject>().Load(projectOptions, sourceFileNames);
        }

        public virtual ProjectOptions ProjectOptions(string projectDirectory, string databaseName)
        {
            return new ProjectOptions(projectDirectory, databaseName);
        }

        public virtual IReference Reference(IProjectItem projectItem, Attribute<string> sourceAttribute, string targetQualifiedName)
        {
            return new Reference(projectItem, sourceAttribute, targetQualifiedName);
        }

        public virtual Rendering Rendering(IProject project, ISnapshot snapshot, string filePath, Item item)
        {
            return new Rendering(project, snapshot, filePath, item);
        }

        public virtual SerializationFile SerializationFile(IProject project, ISnapshot snapshot, string filePath)
        {
            return new SerializationFile(project, snapshot, filePath);
        }

        public virtual ISnapshot Snapshot(ISourceFile sourceFile)
        {
            return new Snapshot(sourceFile);
        }

        public virtual ISourceFile SourceFile(IFileSystemService fileSystem, string sourceFileName)
        {
            return new SourceFile(fileSystem, sourceFileName);
        }

        public virtual Template Template(IProject project, string projectUniqueId, ITextNode textNode, string databaseName, string itemName, string itemIdOrPath)
        {
            return new Template(project, projectUniqueId, textNode, databaseName, itemName, itemIdOrPath);
        }

        public virtual TemplateField TemplateField(Template template)
        {
            return new TemplateField(template);
        }

        public virtual TemplateSection TemplateSection(ITextNode templateSectionTextNode)
        {
            return new TemplateSection(templateSectionTextNode);
        }

        public virtual TextNode TextNode(ISnapshot snapshot, TextPosition position, string name, string value, ITextNode parent)
        {
            return new TextNode(snapshot, position, name, value, parent);
        }
    }
}
