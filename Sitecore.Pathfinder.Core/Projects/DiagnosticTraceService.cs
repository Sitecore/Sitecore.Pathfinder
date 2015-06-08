namespace Sitecore.Pathfinder.Projects
{
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class DiagnosticTraceService : TraceService
  {
    public DiagnosticTraceService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory) : base(configuration)
    {
      this.Factory = factory;
    }

    [NotNull]
    protected IFactoryService Factory { get; }

    [NotNull]
    protected IProject Project { get; private set; }

    [NotNull]
    public ITraceService With([NotNull] IProject project)
    {
      this.Project = project;
      return this;
    }

    protected override void Write(string text, Severity severity, string fileName, TextPosition position, string details)
    {
      if (!string.IsNullOrEmpty(details))
      {
        text += ": " + details;
      }

      var diagnostic = this.Factory.Diagnostic(fileName, position, severity, text);

      this.Project.Diagnostics.Add(diagnostic);
    }
  }
}
