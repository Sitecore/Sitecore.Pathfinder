namespace Sitecore.Pathfinder.Diagnostics
{
  using System;

  public abstract class TraceServiceBase : ITraceService
  {
    public string ProjectDirectory { get; set; }

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
    protected abstract string GetMessage(int text);

    protected abstract void Write(int text, [NotNull] string textType, [NotNull] string fileName, int line, int column, [NotNull] params object[] args);
  }
}
