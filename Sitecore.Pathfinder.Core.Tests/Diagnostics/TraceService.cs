namespace Sitecore.Pathfinder.Diagnostics
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

  [Export(typeof(ITraceService))]
  public class TraceService : TraceServiceBase
  {
    protected override string GetMessage(int text)
    {
      string s;
      return Texts.Messages.TryGetValue(text, out s) ? s : "SCC" + text;
    }

    protected override void Write(int text, [NotNull] string textType, [NotNull] string fileName, int line, int column, [NotNull] params object[] args)
    {
      var message = this.GetMessage(text);

      message = string.Format(message, args);
      var fileInfo = !string.IsNullOrEmpty(fileName) ? fileName : "scc.exe";

      var projectDirectory = this.ProjectDirectory;
      if (!string.IsNullOrEmpty(projectDirectory))
      {
        if (fileInfo.StartsWith(projectDirectory))
        {
          fileInfo = fileInfo.Mid(projectDirectory.Length + 1);
        }
      }

      Console.WriteLine($"{fileInfo}({line},{column}): {textType} SCC{text}: {message}");
    }
  }
}
