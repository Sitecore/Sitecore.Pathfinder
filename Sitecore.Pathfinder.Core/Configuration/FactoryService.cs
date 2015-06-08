namespace Sitecore.Pathfinder.Configuration
{
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

  [Export(typeof(IFactoryService))]
  public class FactoryService : IFactoryService
  {
    [ImportingConstructor]
    public FactoryService([NotNull] ICompositionService compositionService)
    {
      this.CompositionService = compositionService;
    }

    [NotNull]
    protected ICompositionService CompositionService { get; }

    public BinFile BinFile(IProject project, ISnapshot snapshot)
    {
      return new BinFile(project, snapshot);
    }

    public IProjectItem ContentFile(IProject project, ISnapshot snapshot)
    {
      return new ContentFile(project, snapshot);
    }

    public DeviceReference DeviceReference(IProjectItem projectItem, ITextNode deviceNameTextNode, string targetQualifiedName)
    {
      return new DeviceReference(projectItem, deviceNameTextNode, targetQualifiedName);
    }

    public Diagnostic Diagnostic(string fileName, TextPosition position, Severity severity, string text)
    {
      return new Diagnostic(fileName, position, severity, text);
    }

    public ExternalReferenceItem ExternalReferenceItem(IProject project, string externalReference, ISnapshot snapshot)
    {
      return new ExternalReferenceItem(project, externalReference, snapshot);
    }

    public Field Field(Item item, string fieldName, string language, int version, ITextNode nameTextNode, ITextNode valueTextNode, string valueHint = "")
    {
      return new Field(item, fieldName, language, version, nameTextNode, valueTextNode, valueHint);
    }

    public FileReference FileReference(IProjectItem owner, ITextNode sourceTextNode, string targetQualifiedName)
    {
      return new FileReference(owner, sourceTextNode, targetQualifiedName);
    }

    public Item Item(IProject project, string itemPath, ISnapshot snapshot)
    {
      return new Item(project, itemPath, snapshot);
    }

    public Item Item(IProject project, string itemPath, ITextNode textNode)
    {
      return new Item(project, itemPath, textNode);
    }

    public ItemParseContext ItemParseContext(IParseContext context, ItemParser itemParser, string parentItemPath)
    {
      return new ItemParseContext(context, itemParser, parentItemPath);
    }

    public LayoutReference LayoutReference(IProjectItem projectItem, ITextNode layoutTextNode, string targetQualifiedName)
    {
      return new LayoutReference(projectItem, layoutTextNode, targetQualifiedName);
    }

    public LayoutRenderingReference LayoutRenderingReference(IProjectItem projectItem, ITextNode renderingTextNode, string targetQualifiedName)
    {
      return new LayoutRenderingReference(projectItem, renderingTextNode, targetQualifiedName);
    }

    public MediaFile MediaFile(IProject project, ISnapshot snapshot, Item mediaItem)
    {
      return new MediaFile(project, snapshot, mediaItem);
    }

    public IProject Project(ProjectOptions projectOptions, List<string> sourceFileNames)
    {
      return this.CompositionService.Resolve<IProject>().Load(projectOptions, sourceFileNames);
    }

    public ProjectOptions ProjectOptions(string projectDirectory, string databaseName)
    {
      return new ProjectOptions(projectDirectory, databaseName);
    }

    public IReference Reference(IProjectItem projectItem, ITextNode sourceTextNode, string targetQualifiedName)
    {
      return new Reference(projectItem, sourceTextNode, targetQualifiedName);
    }

    public Rendering Rendering(IProject project, ISnapshot snapshot, Item item)
    {
      return new Rendering(project, snapshot, item);
    }

    public SerializationFile SerializationFile(IProject project, ISnapshot snapshot)
    {
      return new SerializationFile(project, snapshot);
    }

    public ISourceFile SourceFile(IFileSystemService fileSystem, string sourceFileName)
    {
      return new SourceFile(fileSystem, sourceFileName);
    }

    public Template Template(IProject project, string projectUniqueId, ITextNode textNode)
    {
      return new Template(project, projectUniqueId, textNode);
    }

    public TemplateField TemplateField(Template template)
    {
      return new TemplateField(template);
    }

    public TemplateSection TemplateSection()
    {
      return new TemplateSection();
    }

    public ITextNode TextNode(ISnapshot snapshot, string name, string value, ITextNode parent)
    {
      return new TextNode(snapshot, name, value, parent);
    }

    public ITextNode TextNode(ISnapshot snapshot, TextPosition position, string name, string value, ITextNode parent)
    {
      return new TextNode(snapshot, position, name, value, parent);
    }
  }
}
