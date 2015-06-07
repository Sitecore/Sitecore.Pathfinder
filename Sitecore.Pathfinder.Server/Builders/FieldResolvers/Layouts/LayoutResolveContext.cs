namespace Sitecore.Pathfinder.Builders.FieldResolvers.Layouts
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Emitters;

  public class LayoutResolveContext
  {
    public LayoutResolveContext([NotNull] IEmitContext emitContext, [NotNull] ITextSnapshot snapshot, [NotNull] string databaseName)
    {
      this.EmitContext = emitContext;
      this.Snapshot = snapshot;
      this.DatabaseName = databaseName;
    }

    [NotNull]
    public string DatabaseName { get; }

    [NotNull]
    public IEmitContext EmitContext { get; }

    [NotNull]
    public ITextSnapshot Snapshot { get; }
  }
}
