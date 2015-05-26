namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [Export(typeof(IParseContext))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ParseContext : IParseContext
  {
    private string itemName;

    private string itemPath;

    private string filePath;

    [ImportingConstructor]
    public ParseContext([NotNull] IConfiguration configuration)
    {
      this.Configuration = configuration;
      this.DocumentSnapshot = Documents.DocumentSnapshot.Empty;
    }

    public IConfiguration Configuration { get; }

    public virtual string DatabaseName => this.Project.Options.DatabaseName;

    public IDocumentSnapshot DocumentSnapshot { get; private set; }

    public virtual string ItemName => this.itemName ?? (this.itemName = PathHelper.GetItemName(this.DocumentSnapshot.SourceFile));

    public virtual string ItemPath => this.itemPath ?? (this.itemPath = PathHelper.GetItemPath(this.Project, this.DocumentSnapshot.SourceFile));

    public virtual string FilePath => this.filePath ?? (this.filePath = PathHelper.GetFilePath(this.Project, this.DocumentSnapshot.SourceFile));

    public IProject Project { get; private set; }

    public ITraceService Trace { get; private set; }

    public IParseContext With(IProject project, IDocumentSnapshot documentSnapshot)
    {
      this.Project = project;
      this.DocumentSnapshot = documentSnapshot;
      this.Trace = new DiagnosticTraceService(this.Configuration).With(this.Project);

      return this;
    }
  }
}
