// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    [Export]
    public class JsonTextSnapshot : TextSnapshot
    {
        [CanBeNull]
        private ITextNode _root;

        [ImportingConstructor]
        public JsonTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        public override ITextNode Root => _root ?? (_root = RootToken != null ? Parse() : TextNode.Empty);

        [CanBeNull, ItemNotNull]
        protected JToken RootToken { get; private set; }

        [NotNull]
        public virtual JsonTextSnapshot With([NotNull] ISourceFile sourceFile, [NotNull] string contents)
        {
            base.With(sourceFile);

            try
            {
                RootToken = JToken.Parse(contents);
            }
            catch (JsonException ex)
            {
                ParseError = ex.Message;
                RootToken = null;
            }
            catch (Exception ex)
            {
                ParseError = ex.Message;
                RootToken = null;
            }

            return this;
        }

        [NotNull]
        protected ITextNode Parse()
        {
            var jobject = RootToken as JObject;
            if (jobject != null)
            {
                var property = jobject.Properties().FirstOrDefault(p => p.Name != "$schema");
                if (property == null)
                {
                    return TextNode.Empty;
                }

                var value = property.Value as JObject;
                if (value == null)
                {
                    return TextNode.Empty;
                }

                _root = Parse(property.Name, value);
            }

            var jarray = RootToken as JArray;
            if (jarray != null)
            {
                var jsonTextNode = new JsonTextNode(this, string.Empty, jarray);
                _root = jsonTextNode;

                foreach (var jobj in jarray.OfType<JObject>())
                {
                    var textNode = Parse(string.Empty, jobj);
                    ((ICollection<ITextNode>)jsonTextNode.ChildNodes).Add(textNode);
                }
            }

            return _root;
        }

        [NotNull]
        protected virtual ITextNode Parse([NotNull] string name, [NotNull, ItemNotNull] JObject jobject)
        {
            var treeNode = new JsonTextNode(this, name, jobject);

            var childNodes = (ICollection<ITextNode>)treeNode.ChildNodes;
            var attributes = (ICollection<ITextNode>)treeNode.Attributes;

            foreach (var property in jobject.Properties())
            {
                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        var objectTextNode = Parse(property.Name, property.Value.Value<JObject>());
                        childNodes.Add(objectTextNode);
                        break;

                    case JTokenType.Array:
                        var array = property.Value.Value<JArray>();
                        foreach (var element in array.OfType<JObject>())
                        {
                            var arrayTextNode = Parse(property.Name, element);
                            childNodes.Add(arrayTextNode);
                        }

                        break;

                    case JTokenType.Boolean:
                    case JTokenType.Date:
                    case JTokenType.Float:
                    case JTokenType.Integer:
                    case JTokenType.String:
                        var propertyTreeNode = new JsonTextNode(this, property.Name, property);
                        attributes.Add(propertyTreeNode);
                        break;
                }
            }

            return treeNode;
        }
    }
}
