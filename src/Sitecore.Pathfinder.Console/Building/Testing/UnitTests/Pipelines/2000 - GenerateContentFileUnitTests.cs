// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Building.Testing.UnitTests.Pipelines
{
    public class GenerateContentFileUnitTests : PipelineProcessorBase<GenerateUnitTestsPipeline>
    {
        public GenerateContentFileUnitTests() : base(2000)
        {
        }

        protected override void Process(GenerateUnitTestsPipeline pipeline)
        {
            foreach (var contentFile in pipeline.Context.Project.Items.OfType<ContentFile>())
            {
                var testName = pipeline.GetTestName(contentFile.FilePath, pipeline.Index, "File");
                pipeline.Tests.Add(testName);

                pipeline.Stream.WriteLine("        [Test]");
                pipeline.Stream.WriteLine("        public void " + testName + "()");
                pipeline.Stream.WriteLine("        {");
                pipeline.Stream.WriteLine("            Assert.IsTrue(File.Exists(FileUtil.MapPath(\"" + contentFile.FilePath + "\")));");
                pipeline.Stream.WriteLine("        }");
                pipeline.Stream.WriteLine();

                pipeline.Index++;
            }
        }
    }
}
