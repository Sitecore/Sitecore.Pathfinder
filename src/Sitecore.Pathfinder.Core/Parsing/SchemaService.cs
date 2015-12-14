// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(ISchemaService))]
    public class SchemaService : ISchemaService
    {
        [NotNull]
        private readonly Dictionary<string, HashSet<string>> _schema = new Dictionary<string, HashSet<string>>();

        [ImportingConstructor]
        public SchemaService([NotNull] IConfiguration configuration, [NotNull] ITraceService trace)
        {
            Configuration = configuration;
            Trace = trace;

            foreach (var pair in Configuration.GetSubKeys("build-project:schema"))
            {
                var attributes = configuration.GetCommaSeparatedStringList("build-project:schema:" + pair.Key);
                _schema[pair.Key] = new HashSet<string>(attributes);
            }
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public bool ValidateSnapshotSchema(IParseContext context, ITextSnapshot textSnapshot)
        {
            return textSnapshot.ValidateSchema(context);
        }

        public bool ValidateTextNodeSchema(ITextNode textNode)
        {
            return ValidateTextNodeSchema(textNode, textNode.Key);
        }

        public bool ValidateTextNodeSchema(ITextNode textNode, string textNodeName)
        {
            if (string.IsNullOrEmpty(textNode.Key))
            {
                return true;
            }

            var result = true;

            HashSet<string> hashSet;
            if (!_schema.TryGetValue(textNodeName, out hashSet))
            {
                _schema.TryGetValue(textNodeName + "-attributes", out hashSet);
            }

            if (hashSet != null)
            {
                foreach (var attribute in textNode.Attributes)
                {
                    if (!hashSet.Contains(attribute.Key))
                    {
                        Trace.TraceError("'" + textNode.Key + "' element has unexpected attribute", attribute, attribute.Key);
                        result = false;
                    }
                }
            }

            if (_schema.TryGetValue(textNodeName + "-childnodes", out hashSet))
            {
                foreach (var childNode in textNode.ChildNodes)
                {
                    if (!hashSet.Contains(childNode.Key))
                    {
                        Trace.TraceError("'" + textNode.Key + "' element has unexpected child node", childNode, childNode.Key);
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}
