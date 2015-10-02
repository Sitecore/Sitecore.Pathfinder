// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
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
    public interface IFactoryService
    {
        [NotNull]
        BinFile BinFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        IProjectItem ContentFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        DeviceReference DeviceReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> deviceNameSourceProperty);

        [NotNull]
        Diagnostic Diagnostic([NotNull] string fileName, TextSpan span, Severity severity, [NotNull] string text);

        [NotNull]
        Field Field([NotNull] Item item, [NotNull] ITextNode textNode);

        [NotNull]
        Field Field([NotNull] Item item, [NotNull] ITextNode textNode, [NotNull] string fieldName, [NotNull] string fieldValue);

        [NotNull]
        FileReference FileReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceSourceProperty);

        [NotNull]
        Item Item([NotNull] IProject project, Guid guid, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath);

        [NotNull]
        ItemBuilder ItemBuilder();

        [NotNull]
        FieldBuilder FieldBuilder();

        [NotNull]
        ItemParseContext ItemParseContext([NotNull] IParseContext context, [NotNull] ItemParser itemParser, [NotNull] string databaseName, [NotNull] string parentItemPath);

        [NotNull]
        LayoutReference LayoutReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> layoutSourceProperty);

        [NotNull]
        LayoutRenderingReference LayoutRenderingReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> layoutTextNode);

        [NotNull]
        MediaFile MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath);

        [NotNull]
        IProject Project([NotNull] ProjectOptions projectOptions, [NotNull] [ItemNotNull] List<string> sourceFileNames);

        [NotNull]
        ProjectOptions ProjectOptions([NotNull] string projectDirectory, [NotNull] string databaseName);

        [NotNull]
        IReference Reference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> sourceSourceProperty);

        [NotNull]
        IReference Reference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> sourceSourceProperty, [NotNull] string referenceText);

        [NotNull]
        Rendering Rendering([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemPath, [NotNull] string itemName, [NotNull] string filePath, [NotNull] string templateIdOrPath);

        [NotNull]
        SerializationFile SerializationFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        ISnapshot Snapshot([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISourceFile SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string sourceFileName, [NotNull] string projectFileName);

        [NotNull]
        Template Template([NotNull] IProject project, Guid guid, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath);

        [NotNull]
        TemplateField TemplateField([NotNull] Template template, [NotNull] ITextNode templateFieldTextNode);

        [NotNull]
        TemplateSection TemplateSection([NotNull] ITextNode templateSectionTextNode);

        [NotNull]
        TextNode TextNode([NotNull] ISnapshot snapshot, TextSpan span, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent);
    }
}
