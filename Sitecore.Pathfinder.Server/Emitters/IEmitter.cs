namespace Sitecore.Pathfinder.Emitters
{
  using Sitecore.Pathfinder.Models;

  public interface IEmitter
  {
    double Sortorder { get; }

    bool CanEmit([NotNull] IEmitContext context, [NotNull] ModelBase model);

    void Emit([NotNull] IEmitContext context, [NotNull] ModelBase model);
  }
}
