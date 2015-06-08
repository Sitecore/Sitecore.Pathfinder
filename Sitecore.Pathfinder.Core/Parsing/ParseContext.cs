namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseContext))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ParseContext : IParseContext
  {
    private string filePath;

    private string itemName;

    private string itemPath;

    [ImportingConstructor]
    public ParseContext([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory)
    {
      this.Configuration = configuration;
      this.Factory = factory;
      this.Snapshot = Documents.Snapshot.Empty;
    }

    public IConfiguration Configuration { get; }

    public virtual string DatabaseName => this.Project.Options.DatabaseName;

    public IFactoryService Factory { get; }

    public virtual string FilePath => this.filePath ?? (this.filePath = PathHelper.GetFilePath(this.Project, this.Snapshot.SourceFile));

    public virtual string ItemName => this.itemName ?? (this.itemName = PathHelper.GetItemName(this.Snapshot.SourceFile));

    public virtual string ItemPath => this.itemPath ?? (this.itemPath = PathHelper.GetItemPath(this.Project, this.Snapshot.SourceFile));

    public IProject Project { get; private set; }

    public ISnapshot Snapshot { get; private set; }

    public ITraceService Trace { get; private set; }

    public IParseContext With(IProject project, ISnapshot snapshot)
    {
      this.Project = project;
      this.Snapshot = snapshot;
      this.Trace = new DiagnosticTraceService(this.Configuration, this.Factory).With(this.Project);

      return this;
    }
  }
}
