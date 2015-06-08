namespace Sitecore.Pathfinder.Projects
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  [DebuggerDisplay("{GetType().Name,nq}: {Name,nq} = {Value}")]
  public class Attribute<T>
  {
    public Attribute([NotNull] string name, [NotNull] T value)
    {
      this.Name = name;
      this.Value = value;
    }

    [NotNull]
    public string Name { get; }

    [CanBeNull]
    public ITextNode Source { get; set; }

    [NotNull]
    public T Value { get; private set; }

    public bool SetValue([NotNull] T value)
    {
      this.Value = value;
      return this.Source?.SetValue(value.ToString()) ?? false;
    }

    public bool SetValue([NotNull] T value, [CanBeNull] ITextNode source)
    {
      this.Value = value;
      this.Source = source;
      return true;
    }

    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
