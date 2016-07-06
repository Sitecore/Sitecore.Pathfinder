using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking
{
    public class FieldTextNode : ITextNode
    {
        public FieldTextNode([NotNull] ISnapshot snapshot, [NotNull] Field databaseField)
        {
            Snapshot = snapshot;
            Key = databaseField.Name;
            Value = databaseField.Value;
        }

        public IEnumerable<ITextNode> Attributes { get; } = Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = Enumerable.Empty<ITextNode>();

        public string Key { get; }

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public string Value { get; }

        public ITextNode GetAttribute(string attributeName)
        {
            return null;
        }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            return string.Empty;
        }

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        public ITextNode GetSnapshotLanguageSpecificChildNode(string name)
        {
            return null;
        }

        public bool HasAttribute(string attributeName)
        {
            return false;
        }
    }
}