namespace Sitecore.Pathfinder.Projects.References
{
  using System.Collections;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

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
        if (!this.ProjectItem.IsBindComplete)
        {
          this.ProjectItem.Bind();
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
    public ProjectItemReference AddFieldReference([NotNull] string fieldValue)
    {
      var reference = new ProjectItemReference(this.ProjectItem.Project, "field reference", fieldValue);
      this.Add(reference);
      return reference;
    }

    [NotNull]
    public Reference AddTemplateReference([NotNull] string templateIdOrPath)
    {
      var reference = new ProjectItemReference(this.ProjectItem.Project, "template", templateIdOrPath);
      this.Add(reference);
      return reference;
    }

    public void Clear()
    {
      this.references.Clear();
    }

    public bool Contains([NotNull] Reference item)
    {
      if (!this.ProjectItem.IsBindComplete)
      {
        this.ProjectItem.Bind();
      }

      return this.references.Contains(item);
    }

    public void CopyTo(Reference[] array, int arrayIndex)
    {
      if (!this.ProjectItem.IsBindComplete)
      {
        this.ProjectItem.Bind();
      }

      this.references.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Reference> GetEnumerator()
    {
      if (!this.ProjectItem.IsBindComplete)
      {
        this.ProjectItem.Bind();
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
