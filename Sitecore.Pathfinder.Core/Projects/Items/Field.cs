namespace Sitecore.Pathfinder.Projects.Items
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Locations;

  // todo: consider basing this on ProjectElement
  public class Field
  {
    public Field([NotNull] Location location)
    {
      this.Location = location;
      this.Name = string.Empty;
      this.Value = string.Empty;
      this.Language = string.Empty;
    }

    public Field([NotNull] Location location, [NotNull] string name, [NotNull] string value)
    {
      this.Location = location;
      this.Name = name;
      this.Value = value;
      this.Language = string.Empty;
    }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public Location Location { get; set; }

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}