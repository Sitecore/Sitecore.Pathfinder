namespace Sitecore.Pathfinder.Building.Preprocessing.Data
{
  using Sitecore.Pathfinder.Diagnostics;

  public class ProjectField
  {
    public ProjectField([NotNull] ProjectItem item)
    {
      this.Item = item;
      this.Id = string.Empty;
      this.Name = string.Empty;
      this.Language = string.Empty;
      this.Version = 0;
      this.Type = string.Empty;
      this.Value = string.Empty;
    }

    public ProjectField([NotNull] ProjectItem item, [NotNull] string name, [NotNull] string value)
    {
      this.Item = item;
      this.Id = string.Empty;
      this.Name = name;
      this.Language = string.Empty;
      this.Version = 0;
      this.Type = string.Empty;
      this.Value = value;
    }

    public ProjectField([NotNull] ProjectItem item, [NotNull] string id, [NotNull] string name, [NotNull] string language, int version, [NotNull] string type, [NotNull] string value)
    {
      this.Item = item;
      this.Id = id;
      this.Name = name;
      this.Language = language;
      this.Version = version;
      this.Type = type;
      this.Value = value;
    }

    public string Id { get; set; }

    [NotNull]
    public ProjectItem Item { get; set; }

    [NotNull]
    public string Language { get; set; }

    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public string Type { get; set; }

    [NotNull]
    public string Value { get; set; }

    public int Version { get; set; }
  }
}
