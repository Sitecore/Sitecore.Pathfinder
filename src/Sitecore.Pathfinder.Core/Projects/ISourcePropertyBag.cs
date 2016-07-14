// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface ISourcePropertyBag
    {
        [NotNull]
        IDictionary<string, object> PropertyBag { get; }

        bool ContainsSourceProperty([NotNull] string name);

        [CanBeNull]
        SourceProperty<T> GetSourceProperty<T>([NotNull] string name);

        [CanBeNull]
        T GetValue<T>([NotNull] string name);

        [NotNull]
        SourceProperty<T> NewSourceProperty<T>([NotNull] string name, [NotNull] T defaultValue, SourcePropertyFlags flags = SourcePropertyFlags.None);

        void SetValue<T>([NotNull] string name, [NotNull] T value);
    }
}
