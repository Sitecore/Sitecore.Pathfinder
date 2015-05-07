namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders
{
  public interface IItemFileBuilder
  {
    double Priority { get; }

    void Build([NotNull] IItemFileBuildContext context);

    bool CanBuild([NotNull] IItemFileBuildContext context);
  }
}
