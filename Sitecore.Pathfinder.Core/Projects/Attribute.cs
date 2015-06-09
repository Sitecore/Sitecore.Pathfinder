// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {Name,nq} = {Value}")]
    public class Attribute<T>
    {
        public Attribute([NotNull] string name, [NotNull] T value)
        {
            Name = name;
            Value = value;
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public ITextNode Source { get; set; }

        [NotNull]
        public T Value { get; private set; }

        public void SetValue([NotNull] T value)
        {
            Value = value;
        }

        public bool SetValue([NotNull] T value, [CanBeNull] ITextNode source)
        {
            Value = value;
            Source = source;
            return true;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
