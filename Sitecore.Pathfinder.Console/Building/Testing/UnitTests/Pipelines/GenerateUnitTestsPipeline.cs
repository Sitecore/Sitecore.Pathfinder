// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Testing.UnitTests.Pipelines
{
    public class GenerateUnitTestsPipeline : PipelineBase<GenerateUnitTestsPipeline>
    {
        [NotNull]
        public IBuildContext Context { get; private set; }

        public int Index { get; set; }

        [NotNull]
        public StreamWriter Stream { get; private set; }

        [NotNull]
        [ItemNotNull]
        public ICollection<string> Tests { get; } = new List<string>();

        [NotNull]
        public GenerateUnitTestsPipeline Execute([NotNull] IBuildContext context, [NotNull] StreamWriter stream)
        {
            Context = context;
            Stream = stream;

            Execute();

            return this;
        }

        [NotNull]
        public string GetTestName([NotNull] string itemIdOrPath, int index, [NotNull] string type)
        {
            return "Test" + index.ToString("000") + itemIdOrPath.Replace("/", "_").GetSafeCodeIdentifier() + "_" + type;
        }
    }
}
