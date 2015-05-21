namespace Sitecore.Pathfinder.Projects
{
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class ProjectTraceService : TraceService
  {
    public ProjectTraceService([NotNull] IConfiguration configuration) : base(configuration)
    {
    }

    [NotNull]
    protected IProject Project { get; private set; }

    [NotNull]
    public ITraceService With([NotNull] IProject project)
    {
      this.Project = project;
      return this;
    }

    protected override void Write(string text, MessageType messageType, string fileName, TextPosition position, string details)
    {
      if (!string.IsNullOrEmpty(details))
      {
        text += ": " + details;
      }

      var message = new ProjectMessage(fileName, position, messageType, text);

      this.Project.Messages.Add(message);
    }
  }
}
