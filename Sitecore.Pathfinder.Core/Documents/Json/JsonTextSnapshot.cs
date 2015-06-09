// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents.Json
{
    public class JsonTextSnapshot : TextSnapshot
    {
        private ITextNode _root;

        public JsonTextSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile, contents)
        {
        }

        public override ITextNode Root
        {
            get
            {
                if (_root == null)
                {
                    JToken token;
                    try
                    {
                        token = JToken.Parse(Contents);
                    }
                    catch
                    {
                        return TextNode.Empty;
                    }

                    var jobject = token as JObject;
                    if (jobject != null)
                    {
                        var r = jobject.Properties().FirstOrDefault(p => p.Name != "$schema");
                        if (r == null)
                        {
                            return TextNode.Empty;
                        }

                        var value = r.Value as JObject;
                        if (value == null)
                        {
                            return TextNode.Empty;
                        }

                        _root = Parse(r.Name, value, null);
                    }

                    var jarray = token as JArray;
                    if (jarray != null)
                    {
                        var r = new JsonTextNode(this, string.Empty, jarray);
                        _root = r;

                        foreach (var o in jarray.OfType<JObject>())
                        {
                            Parse(string.Empty, o, r);
                        }
                    }
                }

                return _root ?? TextNode.Empty;
            }
        }

        public override ITextNode GetJsonChildTextNode(ITextNode textNode, string name)
        {
            return textNode.ChildNodes.FirstOrDefault(n => n.Name == name);
        }

        [NotNull]
        protected virtual ITextNode Parse([NotNull] string name, [NotNull] JObject jobject, [CanBeNull] JsonTextNode parent)
        {
            var treeNode = new JsonTextNode(this, name, jobject, parent);
            (parent?.ChildNodes as ICollection<ITextNode>)?.Add(treeNode);

            var childNodes = (ICollection<ITextNode>)treeNode.ChildNodes;
            var attributes = (ICollection<ITextNode>)treeNode.Attributes;

            foreach (var property in jobject.Properties())
            {
                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        Parse(property.Name, property.Value.Value<JObject>(), treeNode);
                        break;

                    case JTokenType.Array:
                        var array = property.Value.Value<JArray>();
                        var arrayTreeNode = new JsonTextNode(this, property.Name, array, parent);

                        foreach (var o in array.OfType<JObject>())
                        {
                            Parse(string.Empty, o, arrayTreeNode);
                        }

                        childNodes.Add(arrayTreeNode);
                        break;

                    case JTokenType.Boolean:
                    case JTokenType.Date:
                    case JTokenType.Float:
                    case JTokenType.Integer:
                    case JTokenType.String:
                        var propertyTreeNode = new JsonTextNode(this, property.Name, property, treeNode);
                        attributes.Add(propertyTreeNode);
                        break;
                }
            }

            return treeNode;
        }
    }
}
