// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Building.Testing.UnitTests.Pipelines;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Testing.UnitTests
{
    public class GenerateUnitTest : TaskBase
    {
        public GenerateUnitTest() : base("generate-unittests")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Generating_unit_tests___);

            var directory = Path.Combine(context.ProjectDirectory, context.Configuration.Get(Constants.Configuration.LocalTestDirectory));
            context.FileSystem.CreateDirectory(directory);

            var tests = new List<string>();

            GenerateUnitTestFile(context, directory, tests);
            GenerateLocalTestRunnerFile(context, directory, tests);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Generates basic unit tests for the project.");
        }

        private void GenerateLocalTestRunnerFile([NotNull] IBuildContext context, [NotNull] string directory, [NotNull][ItemNotNull] List<string> tests)
        {
            var localDirectory = Path.Combine(directory, "local");
            context.FileSystem.CreateDirectory(localDirectory);

            var fileName = Path.Combine(localDirectory, "PathfinderTest.Local.g.cs");

            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("using System;");
                stream.WriteLine("using System.IO;");
                stream.WriteLine("using System.Net;");
                stream.WriteLine("using System.Linq;");
                stream.WriteLine("using System.Web;");
                stream.WriteLine("using NUnit.Framework;");
                stream.WriteLine("using Sitecore.Pathfinder.Diagnostics;");
                stream.WriteLine("using Sitecore.Pathfinder.Documents;");
                stream.WriteLine("using Sitecore.Pathfinder.Documents.Xml;");
                stream.WriteLine("using Sitecore.Pathfinder.Projects.Items;");
                stream.WriteLine("using Sitecore.Pathfinder.Projects.Templates;");
                stream.WriteLine();

                stream.WriteLine("namespace Sitecore.Pathfinder.Tests");
                stream.WriteLine("{");
                stream.WriteLine("    [TestFixture]");
                stream.WriteLine("    public class PathfinderLocalTests");
                stream.WriteLine("    {");

                foreach (var testName in tests)
                {
                    stream.WriteLine("        [Test]");
                    stream.WriteLine("        public void " + testName + "()");
                    stream.WriteLine("        {");
                    stream.WriteLine("            DoTest(\"" + testName + "\");");
                    stream.WriteLine("        }");
                    stream.WriteLine();
                }

                var url = context.Configuration.Get(Constants.Configuration.HostName) + "/sitecore/shell/client/Applications/Pathfinder/Tests/WebTestRunner.ashx?test=";

                stream.WriteLine("        private void DoTest(string testName)");
                stream.WriteLine("        {");
                stream.WriteLine("            try");
                stream.WriteLine("            {");

                stream.WriteLine("                var output = new WebClient().DownloadString(\"" + url + "\" + HttpUtility.UrlEncode(testName));");
                stream.WriteLine("                if (!string.IsNullOrEmpty(output))");
                stream.WriteLine("                {");
                stream.WriteLine("                    output = HttpUtility.HtmlDecode(output).Trim();");
                stream.WriteLine("                }");
                stream.WriteLine();
                stream.WriteLine("                if (!string.IsNullOrEmpty(output))");
                stream.WriteLine("                {");
                stream.WriteLine("                    Console.WriteLine(output);");
                stream.WriteLine("                }");

                stream.WriteLine("            }");
                stream.WriteLine("            catch (WebException ex)");
                stream.WriteLine("            {");
                stream.WriteLine("                var message = ex.Message;");
                stream.WriteLine();
                stream.WriteLine("                var stream = ex.Response?.GetResponseStream();");
                stream.WriteLine("                if (stream != null)");
                stream.WriteLine("                {");
                stream.WriteLine("                    message = HttpUtility.HtmlDecode(new StreamReader(stream).ReadToEnd()) ?? string.Empty;");
                stream.WriteLine("                }");
                stream.WriteLine();
                stream.WriteLine("                Console.WriteLine(message);");
                stream.WriteLine("            }");
                stream.WriteLine("            catch (Exception ex)");
                stream.WriteLine("            {");
                stream.WriteLine("                Console.WriteLine(ex.Message);");
                stream.WriteLine("            }");

                stream.WriteLine("        }");
                stream.WriteLine("    }");
                stream.WriteLine("}");
            }
        }

        private void GenerateUnitTestFile([NotNull] IBuildContext context, [NotNull] string directory, [NotNull][ItemNotNull] List<string> tests)
        {
            var serverDirectory = Path.Combine(directory, "server");
            context.FileSystem.CreateDirectory(serverDirectory);

            var fileName = Path.Combine(serverDirectory, "PathfinderTest.Server.g.cs");

            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("using System;");
                stream.WriteLine("using System.IO;");
                stream.WriteLine("using NUnit.Framework;");
                stream.WriteLine("using Sitecore;");
                stream.WriteLine("using Sitecore.Configuration;");
                stream.WriteLine("using Sitecore.Data.Managers;");
                stream.WriteLine("using Sitecore.Data.Items;");
                stream.WriteLine("using Sitecore.IO;");
                stream.WriteLine();

                stream.WriteLine("namespace Sitecore.Pathfinder.Tests");
                stream.WriteLine("{");
                stream.WriteLine("    [TestFixture]");
                stream.WriteLine("    public class PathfinderTests");
                stream.WriteLine("    {");

                var pipeline = context.PipelineService.Resolve<GenerateUnitTestsPipeline>().Execute(context, stream);
                tests.AddRange(pipeline.Tests);

                stream.WriteLine("    }");
                stream.WriteLine("}");
            }
        }
    }
}
