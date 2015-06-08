namespace Sitecore.Pathfinder.Configuration
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Parsing.Items;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Layouts;
  using Sitecore.Pathfinder.Projects.References;
  using Sitecore.Pathfinder.Projects.Templates;

  public interface IFactoryService
  {
    [NotNull]
    BinFile BinFile([NotNull] IProject project, [NotNull] ISnapshot snapshot);

    [NotNull]
    IProjectItem ContentFile([NotNull] IProject project, [NotNull] ISnapshot snapshot);

    [NotNull]
    DeviceReference DeviceReference([NotNull] IProjectItem projectItem, [NotNull] ITextNode deviceNameTextNode, [NotNull] string targetQualifiedName);

    [NotNull]
    Diagnostic Diagnostic([NotNull] string fileName, TextPosition position, Severity severity, [NotNull] string text);

    [NotNull]
    ExternalReferenceItem ExternalReferenceItem([NotNull] IProject project, [NotNull] string externalReference, [NotNull] ISnapshot snapshot);

    [NotNull]
    Field Field([NotNull] Item item, [NotNull] string fieldName, [NotNull] string language, int version, [NotNull] ITextNode nameTextNode, [NotNull] ITextNode valueTextNode, [NotNull] string valueHint = "");

    [NotNull]
    FileReference FileReference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName);

    [NotNull]
    Item Item([NotNull] IProject project, [NotNull] string itemPath, [NotNull] ISnapshot snapshot);

    [NotNull]
    Item Item([NotNull] IProject project, [NotNull] string itemPath, [NotNull] ITextNode textNode);

    [NotNull]
    ItemParseContext ItemParseContext([NotNull] IParseContext context, [NotNull] ItemParser itemParser, [NotNull] string parentItemPath);

    [NotNull]
    LayoutReference LayoutReference([NotNull] IProjectItem projectItem, [NotNull] ITextNode layoutTextNode, [NotNull] string targetQualifiedName);

    [NotNull]
    LayoutRenderingReference LayoutRenderingReference([NotNull] IProjectItem projectItem, [NotNull] ITextNode renderingTextNode, [NotNull] string targetQualifiedName);

    [NotNull]
    MediaFile MediaFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] Item mediaItem);

    [NotNull]
    IProject Project([NotNull] ProjectOptions projectOptions, [NotNull] List<string> sourceFileNames);

    [NotNull]
    ProjectOptions ProjectOptions([NotNull] string projectDirectory, [NotNull] string databaseName);

    [NotNull]
    IReference Reference([NotNull] IProjectItem projectItem, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName);

    [NotNull]
    Rendering Rendering([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] Item item);

    [NotNull]
    SerializationFile SerializationFile([NotNull] IProject project, [NotNull] ISnapshot snapshot);

    [NotNull]
    ISourceFile SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string sourceFileName);

    [NotNull]
    Template Template([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ITextNode textNode);

    [NotNull]
    TemplateField TemplateField([NotNull] Template template);

    [NotNull]
    TemplateSection TemplateSection();

    [NotNull]
    ITextNode TextNode([NotNull] ISnapshot snapshot, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent);

    [NotNull]
    ITextNode TextNode([NotNull] ISnapshot snapshot, TextPosition position, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent);
  }
}
