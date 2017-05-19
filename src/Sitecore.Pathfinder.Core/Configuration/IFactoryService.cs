// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles;
using Sitecore.Pathfinder.Languages.ConfigFiles;
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
    public interface IFactoryService
    {
        [NotNull]
        BinFile BinFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        ConfigFile ConfigFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        IProjectItem ContentFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        DeviceReference DeviceReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> deviceNameSourceProperty, [NotNull] string databaseName);

        [NotNull]
        Diagnostic Diagnostic(int msg, [NotNull] string fileName, TextSpan span, Severity severity, [NotNull] string text);

        [NotNull]
        Field Field([NotNull] Item item);

        [NotNull]
        Field Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string fieldValue);

        [NotNull]
        FieldBuilder FieldBuilder();

        [NotNull]
        FileReference FileReference([NotNull] IProjectItem owner, [NotNull] ITextNode textNode, [NotNull] string referenceText);

        [NotNull]
        FileReference FileReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceSourceProperty, [NotNull] string referenceText);

        [NotNull]
        Item Item([NotNull] Database database, Guid guid, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath);

        [NotNull]
        ItemBuilder ItemBuilder();

        [NotNull]
        ItemParseContext ItemParseContext([NotNull] IParseContext context, [NotNull] ItemParser itemParser, [NotNull] string databaseName, [NotNull] string parentItemPath, bool isImport);

        [NotNull]
        LayoutReference LayoutReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> layoutSourceProperty, [NotNull] string databaseName);

        [NotNull]
        LayoutRenderingReference LayoutRenderingReference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> layoutTextNode, [NotNull] string databaseName);

        [NotNull]
        MediaFile MediaFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemPath, [NotNull] string filePath);

        [NotNull]
        ProjectOptions ProjectOptions([NotNull] string databaseName);

        [NotNull]
        IReference Reference([NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode, [NotNull] string referenceText, [NotNull] string databaseName);

        [NotNull]
        IReference Reference([NotNull] IProjectItem projectItem, [NotNull] SourceProperty<string> sourceSourceProperty, [NotNull] string referenceText, [NotNull] string databaseName);

        [NotNull]
        Rendering Rendering([NotNull] Database database, [NotNull] ISnapshot snapshot, [NotNull] string itemPath, [NotNull] string itemName, [NotNull] string filePath, [NotNull] string templateIdOrPath);

        [NotNull]
        SerializationFile SerializationFile([NotNull] IProjectBase project, [NotNull] ISnapshot snapshot, [NotNull] string filePath);

        [NotNull]
        ISnapshot Snapshot([NotNull] ISourceFile sourceFile);

        [NotNull]
        ISourceFile SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string absoluteFileName);

        // todo: swap guid and textnode parameters
        [NotNull]
        Template Template([NotNull] Database database, Guid guid, [NotNull] string itemName, [NotNull] string itemIdOrPath);

        [NotNull]
        TemplateField TemplateField([NotNull] Template template, Guid guid);

        [NotNull]
        TemplateSection TemplateSection([NotNull] Template template, Guid guid);

        [NotNull]
        TextNode TextNode([NotNull] ISnapshot snapshot, TextSpan span, [NotNull] string name, [NotNull] string value);
    }
}
