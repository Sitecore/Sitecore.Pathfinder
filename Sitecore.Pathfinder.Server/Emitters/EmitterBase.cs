namespace Sitecore.Pathfinder.Emitters
{
  using Sitecore.Pathfinder.Projects;

  public abstract class EmitterBase : IEmitter
  {
    protected const double BinFiles = 9999;

    protected const double ContentFiles = 4000;

    protected const double Items = 2000;

    protected const double MediaFiles = 3000;

    protected const double Templates = 1000;

    protected EmitterBase(double sortorder)
    {
      this.Sortorder = sortorder;
    }

    public double Sortorder { get; }

    public abstract bool CanEmit(IEmitContext context, IProjectItem projectItem);

    public abstract void Emit(IEmitContext context, IProjectItem projectItem);
  }
}
