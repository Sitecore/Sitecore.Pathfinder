namespace Sitecore.Pathfinder.Models
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ModelBase
  {
    protected ModelBase([NotNull] string sourceFileName)
    {
      this.SourceFileName = sourceFileName;
    }

    [NotNull]
    public ICollection<LinkBase> Links { get; } = new List<LinkBase>();

    [NotNull]
    public string SourceFileName { get; }
  }
}
