namespace Sitecore.Pathfinder.Models
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IProject
  {
    [NotNull]
    ICollection<ModelBase> Models { get; }

    [NotNull]
    string ProjectDirectory { get; }

    void Parse();
  }
}
