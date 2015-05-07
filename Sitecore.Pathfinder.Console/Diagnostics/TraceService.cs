namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(ITraceService))]
  public class TraceService : TraceServiceBase
  {
    private readonly IConfiguration configuration;

    private IConfiguration texts;

    [ImportingConstructor]
    public TraceService([NotNull] IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    protected override string GetMessage(int text)
    {
      var key = "SCC" + text.ToString("0000");

      if (this.texts == null)
      {
        // todo: make texts pluggable
        var language = this.configuration.Get("error-messages-language");
        if (string.IsNullOrEmpty(language))
        {
          throw new TraceException("Configuration element 'error-messages-language' not found");
        }

        var fileName = PathHelper.Combine(this.configuration.Get(Building.Constants.ToolsPath), "texts." + language + ".json");
        if (!File.Exists(fileName))
        {
          throw new TraceException("Error text file not found: " + fileName);
        }

        var config = new Configuration();
        config.AddJsonFile(fileName);

        this.texts = config;
      }

      return this.texts.Get(key);
    }

    protected override void Write(int text, string textType, string fileName, int line, int column, params object[] args)
    {
      var message = this.GetMessage(text);
      if (string.IsNullOrEmpty(message))
      {
        if (this.configuration.Get<bool>("ignore-blank-error-messages"))
        {
          // error text not found - ignore the error
          return;
        }

        throw new TraceException($"Error message SCC'{text}' not found");
      }

      var solutionDirectory = this.configuration.Get(Building.Constants.SolutionDirectory);
      if (fileName.StartsWith(solutionDirectory, StringComparison.OrdinalIgnoreCase))
      {
        fileName = fileName.Mid(solutionDirectory.Length + 1);
      }

      message = string.Format(message, args);
      var fileInfo = !string.IsNullOrEmpty(fileName) ? fileName : "scc.exe";

      Console.WriteLine($"{fileInfo}({line},{column}): {textType} SCC{text}: {message}");
    }
  }
}
