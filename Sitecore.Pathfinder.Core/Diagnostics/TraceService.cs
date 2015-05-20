namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.TextDocuments;

  public enum MessageType
  {
    Information,
    Warning,
    Error
  }

  [Export(typeof(ITraceService))]
  public class TraceService : ITraceService
  {
    [ImportingConstructor]
    public TraceService([NotNull] IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    [NotNull]
    protected IConfiguration Configuration { get; }

    public void TraceError(string text, string details = "")
    {
      this.Write(text, MessageType.Error, string.Empty, TextPosition.Empty, details);
    }

    public void TraceError(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, MessageType.Error, fileName, position, details);
    }

    public void TraceInformation(string text, string details = "")
    {
      this.Write(text, MessageType.Information, string.Empty, TextPosition.Empty, details);
    }

    public void TraceInformation(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, MessageType.Information, fileName, position, details);
    }

    public void TraceWarning(string text, string details = "")
    {
      this.Write(text, MessageType.Warning, string.Empty, TextPosition.Empty, details);
    }

    public void TraceWarning(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, MessageType.Warning, fileName, position, details);
    }

    public void Writeline(string text, string details = "")
    {
      if (!string.IsNullOrEmpty(details))
      {
        text += ": " + details;
      }

      Console.WriteLine(text);
    }

    protected virtual void Write([NotNull] string text, MessageType messageType, [NotNull] string fileName, TextPosition position, [NotNull] string details)
    {
      if (!string.IsNullOrEmpty(details))
      {
        text += ": " + details;
      }

      var fileInfo = !string.IsNullOrEmpty(fileName) ? fileName : "scc.cmd";

      var solutionDirectory = this.Configuration.Get(Pathfinder.Constants.Configuration.SolutionDirectory);
      if (!string.IsNullOrEmpty(solutionDirectory))
      {
        if (fileInfo.StartsWith(solutionDirectory, StringComparison.OrdinalIgnoreCase))
        {
          fileInfo = fileInfo.Mid(solutionDirectory.Length + 1);
        }
      }

      var lineInfo = position.LineLength == 0 ? $"({position.LineNumber},{position.LinePosition})" : $"({position.LineNumber},{position.LinePosition},{position.LineNumber},{position.LinePosition + position.LineLength})";

      Console.WriteLine($"{fileInfo}{lineInfo}: {messageType.ToString().ToLowerInvariant()} SCC0000: {text}");
    }
  }
}
