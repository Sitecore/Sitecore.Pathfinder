namespace Sitecore.Pathfinder.Projects.Templates
{
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateField
  {
    [NotNull]
    public string LongHelp { get; set; } = string.Empty;

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public bool Shared { get; set; }

    [NotNull]
    public string ShortHelp { get; set; } = string.Empty;

    [NotNull]
    public string Source { get; set; } = string.Empty;

    [NotNull]
    public string StandardValue { get; set; } = string.Empty;

    [NotNull]
    public string Type { get; set; } = string.Empty;

    public bool Unversioned { get; set; }
  }
}
