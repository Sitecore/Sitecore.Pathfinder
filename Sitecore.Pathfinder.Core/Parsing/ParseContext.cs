namespace Sitecore.Pathfinder.Parsing
{
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IParseContext))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ParseContext : IParseContext
  {
    private string itemName;

    private string itemPath;

    [ImportingConstructor]
    public ParseContext([NotNull] IConfiguration configuration)
    {
      this.Configuration = configuration;
      this.Document = TextDocuments.Document.Empty;
    }

    public IConfiguration Configuration { get; }

    public virtual string DatabaseName => this.Project.DatabaseName;

    public IDocument Document { get; private set; }

    public virtual string ItemName => this.itemName ?? (this.itemName = PathHelper.GetItemName(this.Document.SourceFile));

    public virtual string ItemPath => this.itemPath ?? (this.itemPath = PathHelper.GetItemPath(this.Project, this.Document.SourceFile));

    public IProject Project { get; private set; }

    public ITraceService Trace { get; private set; }

    public IParseContext With(IProject project, IDocument document)
    {
      this.Project = project;
      this.Document = document;
      this.Trace = new ProjectTraceService(this.Configuration).With(this.Project);

      return this;
    }
  }
}
