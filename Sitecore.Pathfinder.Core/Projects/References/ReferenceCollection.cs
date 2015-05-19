namespace Sitecore.Pathfinder.Projects.References
{
  using System.Collections;
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class ReferenceCollection : ICollection<IReference>
  {
    private readonly List<IReference> references = new List<IReference>();

    public ReferenceCollection([NotNull] ProjectItem projectItem)
    {
      this.ProjectItem = projectItem;
    }

    public int Count => this.references.Count;

    public bool IsReadOnly => false;

    [NotNull]
    public ProjectItem ProjectItem { get; }

    public void Add([NotNull] IReference item)
    {
      this.references.Add(item);
    }

    public void AddRange([NotNull] IEnumerable<IReference> items)
    {
      this.references.AddRange(items);
    }

    public void Clear()
    {
      this.references.Clear();
    }

    public bool Contains([NotNull] IReference item)
    {
      return this.references.Contains(item);
    }

    public void CopyTo(IReference[] array, int arrayIndex)
    {
      this.references.CopyTo(array, arrayIndex);
    }

    public IEnumerator<IReference> GetEnumerator()
    {
      return this.references.GetEnumerator();
    }

    public bool Remove([NotNull] IReference item)
    {
      return this.references.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
