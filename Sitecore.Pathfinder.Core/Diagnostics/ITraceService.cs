namespace Sitecore.Pathfinder.Diagnostics
{
  public interface ITraceService
  {
    [CanBeNull]
    string ProjectDirectory { get; set; }

    void TraceError(int text);

    void TraceError(int text, [NotNull] params object[] args);

    void TraceError(int text, [NotNull] string fileName, int line, int column, [NotNull] params object[] args);

    void TraceInformation(int text);

    void TraceInformation(int text, [NotNull] params object[] args);

    void TraceInformation(int text, [NotNull] string fileName, int line, int column, [NotNull] params object[] args);

    void TraceWarning(int text);

    void TraceWarning(int text, [NotNull] params object[] args);

    void TraceWarning(int text, [NotNull] string fileName, int line, int column, [NotNull] params object[] args);

    void Writeline([NotNull] string message);

    void Writeline(int text);

    void Writeline(int text, [NotNull] params object[] args);
  }
}
