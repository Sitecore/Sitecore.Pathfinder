namespace Sitecore.Pathfinder.Models
{
  using Sitecore.Pathfinder.Diagnostics;

  public abstract class ItemModelBase : ModelBase
  {
    protected ItemModelBase([NotNull] string sourceFileName) : base(sourceFileName)
    {
      this.Name = string.Empty;
      this.DatabaseName = string.Empty;
      this.ItemIdOrPath = string.Empty;
      this.Icon = string.Empty;
    }

    [NotNull]
    public string DatabaseName { get; set; }

    [NotNull]
    public string Icon { get; set; }

    [NotNull]
    public string ItemIdOrPath { get; set; }

    [NotNull]
    public string Name { get; set; }
  }
}
