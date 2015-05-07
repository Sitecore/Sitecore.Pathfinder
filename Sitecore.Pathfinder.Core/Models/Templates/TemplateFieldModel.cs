namespace Sitecore.Pathfinder.Models.Templates
{
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateFieldModel
  {
    public TemplateFieldModel()
    {
      this.Name = string.Empty;
      this.Source = string.Empty;
    }

    [NotNull]
    public string Name { get; set; }

    public bool Shared { get; set; }

    [NotNull]
    public string Source { get; set; }

    [NotNull]
    public string Type { get; set; }

    public bool Unversioned { get; set; }
  }
}
