namespace Sitecore.Pathfinder.Projects.References
{
  using System.Collections;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Locations;

  public class ReferenceCollection : ICollection<Reference>
  {
    private readonly List<Reference> references = new List<Reference>();

    public ReferenceCollection([NotNull] ProjectItem projectItem)
    {
      this.ProjectItem = projectItem;
    }

    public int Count
    {
      get
      {
        if (!this.ProjectItem.IsAnalyzed)
        {
          this.ProjectItem.Analyze();
        }

        return this.references.Count;
      }
    }

    public bool IsReadOnly => false;

    [NotNull]
    public ProjectItem ProjectItem { get; }

    public void Add([NotNull] Reference item)
    {
      this.references.Add(item);
    }

    [NotNull]
    public ProjectItemReference AddFieldReference(Field field)
    {
      var reference = new ProjectItemReference(this.ProjectItem.Project, field.Location, "field reference", field.Value);
      this.Add(reference);
      return reference;
    }

    [NotNull]
    public Reference AddTemplateReference(Location sourceLocation, [NotNull] string templateIdOrPath)
    {
      var reference = new ProjectItemReference(this.ProjectItem.Project, sourceLocation, "template", templateIdOrPath);
      this.Add(reference);
      return reference;
    }

    public void Clear()
    {
      this.references.Clear();
    }

    public bool Contains([NotNull] Reference item)
    {
      if (!this.ProjectItem.IsAnalyzed)
      {
        this.ProjectItem.Analyze();
      }

      return this.references.Contains(item);
    }

    public void CopyTo(Reference[] array, int arrayIndex)
    {
      if (!this.ProjectItem.IsAnalyzed)
      {
        this.ProjectItem.Analyze();
      }

      this.references.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Reference> GetEnumerator()
    {
      if (!this.ProjectItem.IsAnalyzed)
      {
        this.ProjectItem.Analyze();
      }

      return this.references.GetEnumerator();
    }

    public bool Remove([NotNull] Reference item)
    {
      return this.references.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
