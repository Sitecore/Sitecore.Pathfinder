namespace Sitecore.Pathfinder.Building.Builders.ItemFiles
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Data.FieldHandlers;

  public interface IItemFileBuildContext
  {
    [NotNull]
    IEmitContext BuildContext { get; }

    [NotNull]
    string DatabaseName { get; }

    [NotNull]
    IEnumerable<IFieldHandler> FieldHandlers { get; }

    [NotNull]
    string FileName { get; }

    [NotNull]
    string ItemPath { get; }
  }
}
