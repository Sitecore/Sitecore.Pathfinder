namespace Sitecore.Pathfinder.Projects.Templates
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class TemplateSection
  {
    public TemplateSection()
    {
      this.Name = string.Empty;

      this.Fields = new List<TemplateField>();
    }

    [NotNull]
    public IList<TemplateField> Fields { get; }

    [NotNull]
    public string Icon { get; set; }

    [NotNull]
    public string Name { get; set; }

    public void Merge([NotNull] TemplateSection newSection)
    {
      if (!string.IsNullOrEmpty(newSection.Icon))
      {
        this.Icon = newSection.Icon;
      }

      foreach (var newField in newSection.Fields)
      {
        var field = this.Fields.FirstOrDefault(f => string.Compare(f.Name, newField.Name, StringComparison.OrdinalIgnoreCase) == 0);
        if (field == null)
        {
          this.Fields.Add(newField);
          continue;
        }

        field.Merge(newField);
      }
    }
  }
}