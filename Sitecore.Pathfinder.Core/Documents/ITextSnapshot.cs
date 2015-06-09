// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Documents
{
    public interface ITextSnapshot : ISnapshot
    {
        bool IsEditable { get; }

        bool IsEditing { get; }

        [NotNull]
        ITextNode Root { get; }

        void BeginEdit();

        void EndEdit();

        void EnsureIsEditing();

        [CanBeNull]
        ITextNode GetJsonChildTextNode([NotNull] ITextNode textNode, [NotNull] string name);

        void ValidateSchema([NotNull] IParseContext context);
    }
}
