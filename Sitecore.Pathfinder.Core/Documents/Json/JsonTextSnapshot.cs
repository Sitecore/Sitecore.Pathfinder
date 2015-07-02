// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents.Json
{
    public class JsonTextSnapshot : TextSnapshot
    {
        private ITextNode _root;

        public JsonTextSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile)
        {
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
        }

        public override ITextNode Root => _root ?? (_root = (RootToken != null ? Parse() : TextNode.Empty));

        [CanBeNull]
        protected JToken RootToken { get; }

        public override ITextNode GetJsonChildTextNode(ITextNode textNode, string name)
        {
            return textNode.ChildNodes.FirstOrDefault(n => n.Name == name);
        }

        public override void SaveChanges()
        {
            if (RootToken == null)
            {
                return;
            }

            using (var stream = new StreamWriter(SourceFile.FileName))
            {
                using (var writer = new JsonTextWriter(stream))
                {
                    writer.Formatting = Formatting.Indented;

                    RootToken.WriteTo(writer);
                }
            }
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

                _root = Parse(property.Name, value, null);
            }

            var jarray = RootToken as JArray;
            if (jarray != null)
            {
                var jsonTextNode = new JsonTextNode(this, string.Empty, jarray);
                _root = jsonTextNode;

                foreach (var jobj in jarray.OfType<JObject>())
                {
                    Parse(string.Empty, jobj, jsonTextNode);
                }
            }

            return _root;
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
