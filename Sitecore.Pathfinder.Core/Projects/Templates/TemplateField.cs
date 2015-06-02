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

    public int SortOrder { get; set; }

    public void Merge([NotNull] TemplateField newField)
    {
      if (!string.IsNullOrEmpty(newField.Type))
      {
        this.Type = newField.Type;
      }

      if (!string.IsNullOrEmpty(newField.Source))
      {
        this.Source = newField.Source;
      }

      if (newField.Shared)
      {
        this.Shared = true;
      }

      if (newField.Unversioned)
      {
        this.Unversioned = true;
      }

      if (!string.IsNullOrEmpty(newField.StandardValue))
      {
        this.StandardValue = newField.StandardValue;
      }

      if (!string.IsNullOrEmpty(newField.ShortHelp))
      {
        this.ShortHelp = newField.ShortHelp;
      }

      if (!string.IsNullOrEmpty(newField.LongHelp))
      {
        this.LongHelp = newField.LongHelp;
      }

      if (newField.SortOrder != 0)
      {
        this.SortOrder = newField.SortOrder;
      }
    }
  }
}