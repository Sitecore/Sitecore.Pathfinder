namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class Reference
  {
    protected Reference([NotNull] string type)
    {
      this.Type = type;
    }

    public bool IsResolved { get; set; }

    public bool IsValid { get; set; }

    [NotNull]
    public string Type { get; }
  }
}
