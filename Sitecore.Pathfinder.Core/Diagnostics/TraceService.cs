namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel.Composition;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

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

    public void TraceError(int text)
    {
      this.Write(text, "error", string.Empty, 0, 0);
    }

    public void TraceError(int text, params object[] args)
    {
      this.Write(text, "error", string.Empty, 0, 0, args);
    }

    public void TraceError(int text, string fileName, int line = 0, int column = 0, params object[] args)
    {
      this.Write(text, "error", fileName, line, column, args);
    }

    public void TraceInformation(int text)
    {
      this.Write(text, "information", string.Empty, 0, 0);
    }

    public void TraceInformation(int text, params object[] args)
    {
      this.Write(text, "information", string.Empty, 0, 0, args);
    }

    public void TraceInformation(int text, string fileName, int line = 0, int column = 0, params object[] args)
    {
      this.Write(text, "information", fileName, line, column, args);
    }

    public void TraceWarning(int text)
    {
      this.Write(text, "warning", string.Empty, 0, 0);
    }

    public void TraceWarning(int text, params object[] args)
    {
      this.Write(text, "warning", string.Empty, 0, 0, args);
    }

    public void TraceWarning(int text, string fileName, int line = 0, int column = 0, params object[] args)
    {
      this.Write(text, "warning", fileName, line, column, args);
    }

    public void Writeline(string message)
    {
      Console.WriteLine(message);
    }

    public void Writeline(int text)
    {
      var message = this.GetMessage(text);
      Console.WriteLine(message);
    }

    public void Writeline(int text, params object[] args)
    {
      var message = this.GetMessage(text);
      Console.WriteLine(message, args);
    }

    [NotNull]
    protected virtual string GetMessage(int text)
    {
      return Texts.Messages[text];
    }

    protected virtual void Write(int text, [NotNull] string textType, [NotNull] string fileName, int line, int column, [NotNull] params object[] args)
    {
      var message = this.GetMessage(text);
      if (string.IsNullOrEmpty(message))
      {
        throw new TraceException($"Error message SCC'{text}' not found");
      }

      message = string.Format(message, args);
      var fileInfo = !string.IsNullOrEmpty(fileName) ? fileName : "scc.exe";

      var projectDirectory = this.Configuration.Get(Pathfinder.Constants.Configuration.SolutionDirectory);
      if (!string.IsNullOrEmpty(projectDirectory))
      {
        if (fileInfo.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
        {
          fileInfo = fileInfo.Mid(projectDirectory.Length + 1);
        }
      }

      Console.WriteLine($"{fileInfo}({line},{column}): {textType} SCC{text}: {message}");
    }
  }
}
