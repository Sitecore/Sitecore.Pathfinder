// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Documents
{
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        public TextSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile)
        {
            Contents = contents;

            Root = new SnapshotTextNode(this);
        }

        public bool IsEditable { get; protected set; }

        public bool IsEditing { get; protected set; }

        public virtual ITextNode Root { get; }

        [NotNull]
        protected string Contents { get; }

        public virtual void BeginEdit()
        {
            throw new InvalidOperationException("Document is not editable");
        }

        public virtual void EndEdit()
        {
            throw new InvalidOperationException("Document is not editable");
        }

        public void EnsureIsEditing()
        {
            if (!IsEditing)
            {
                throw new InvalidOperationException("Document is not in edit mode");
            }
        }

        public virtual ITextNode GetJsonChildTextNode(ITextNode textNode, string name)
        {
            // overwritten in JsonTextSnapshot to find the appropriate text node
            return textNode;
        }

        public virtual void ValidateSchema(IParseContext context)
        {
        }
    }
}
