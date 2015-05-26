namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

  public enum Severity
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
      this.Write(text, Severity.Error, string.Empty, TextPosition.Empty, details);
    }

    public void TraceError(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, Severity.Error, fileName, position, details);
    }

    public void TraceError(string text, ITextNode textNode, string details = "")
    {
      this.Write(text, Severity.Error, textNode.DocumentSnapshot.SourceFile.FileName, textNode.Position, details);
    }

    public void TraceInformation(string text, string details = "")
    {
      this.Write(text, Severity.Information, string.Empty, TextPosition.Empty, details);
    }

    public void TraceInformation(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, Severity.Information, fileName, position, details);
    }

    public void TraceInformation(string text, ITextNode textNode, string details = "")
    {
      this.Write(text, Severity.Information, textNode.DocumentSnapshot.SourceFile.FileName, textNode.Position, details);
    }

    public void TraceWarning(string text, string details = "")
    {
      this.Write(text, Severity.Warning, string.Empty, TextPosition.Empty, details);
    }

    public void TraceWarning(string text, string fileName, TextPosition position, string details = "")
    {
      this.Write(text, Severity.Warning, fileName, position, details);
    }

    public void TraceWarning(string text, ITextNode textNode, string details = "")
    {
      this.Write(text, Severity.Warning, textNode.DocumentSnapshot.SourceFile.FileName, textNode.Position, details);
    }

    public void Writeline(string text, string details = "")
    {
      if (!string.IsNullOrEmpty(details))
      {
        text += ": " + details;
      }

      Console.WriteLine(text);
    }

    protected virtual void Write([NotNull] string text, Severity severity, [NotNull] string fileName, TextPosition position, [NotNull] string details)
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

      Console.WriteLine($"{fileInfo}{lineInfo}: {severity.ToString().ToLowerInvariant()} SCC0000: {text}");
    }
  }
}
