namespace Sitecore.Pathfinder.Emitters
{
  using Sitecore.Pathfinder.Projects;

  public abstract class EmitterBase : IEmitter
  {
    protected EmitterBase(double sortorder)
    {
      this.Sortorder = sortorder;
    }

    public double Sortorder { get; }

    public abstract bool CanEmit(IEmitContext context, IProjectItem projectItem);

    public abstract void Emit(IEmitContext context, IProjectItem projectItem);
  }
}
