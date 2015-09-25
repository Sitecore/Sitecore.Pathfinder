using System.Linq;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Building.Testing.UnitTests.Pipelines
{
    public class GenerateBinFileUnitTests : PipelineProcessorBase<GenerateUnitTestsPipeline>
    {
        public GenerateBinFileUnitTests() : base(1000)
        {
        }

        protected override void Process(GenerateUnitTestsPipeline pipeline)
        {
            foreach (var binFile in pipeline.Context.Project.Items.OfType<BinFile>())
            {
                var testName = pipeline.GetTestName(binFile.FilePath, pipeline.Index, "File");
                pipeline.Tests.Add(testName);

                pipeline.Stream.WriteLine("        [Test]");
                pipeline.Stream.WriteLine("        public void " + testName + "()");
                pipeline.Stream.WriteLine("        {");
                pipeline.Stream.WriteLine("            Assert.IsTrue(File.Exists(FileUtil.MapPath(\"" + binFile.FilePath + "\")));");
                pipeline.Stream.WriteLine("        }");
                pipeline.Stream.WriteLine();

                pipeline.Index++;
            }
        }
    }
}