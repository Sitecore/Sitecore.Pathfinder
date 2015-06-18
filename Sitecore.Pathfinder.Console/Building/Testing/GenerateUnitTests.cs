using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Building.Testing
{
    [Export(typeof(ITask))]
    public class GenerateUnitTests : TaskBase
    {
        public GenerateUnitTests() : base("generate-unittests")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Generating_unit_tests___);

            var directory = Path.Combine(context.SolutionDirectory, ".tests");
            context.FileSystem.CreateDirectory(directory);

            GenerateUnitTestFile(context, directory);
            // GenerateWebTestRunnerFile(context, baseFileName);
            GenerateLocalTestRunnerFile(context, directory);
        }

        private void GenerateLocalTestRunnerFile([NotNull] IBuildContext context, [NotNull] string directory)
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

                var index = 0;
                foreach (var item in context.Project.Items.OfType<Item>())
                {
                    var testName = "Test" + index.ToString("#0") + "_" + item.ItemIdOrPath.GetSafeCodeIdentifier();

                    stream.WriteLine("        [Test]");
                    stream.WriteLine("        public void " + testName + "()");
                    stream.WriteLine("        {");
                    stream.WriteLine("            DoTest(\"" + testName + "\");");
                    stream.WriteLine("        }");
                    stream.WriteLine();

                    index++;
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

        protected void GenerateWebTestRunnerFile([NotNull] IBuildContext context, [NotNull] string baseFileName)
        {
            var fileName = baseFileName + ".ashx";

            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("<%@ WebHandler Language=\"C#\" Class=\"SitecorePathfinderWebTestRunnerHandler\" %>");
                stream.WriteLine();

                stream.WriteLine("using System;");
                stream.WriteLine("using System.Collections.Specialized;");
                stream.WriteLine("using System.Collections.Generic;");
                stream.WriteLine("using System.IO;");
                stream.WriteLine("using System.Linq;");
                stream.WriteLine("using System.Web;");
                stream.WriteLine();

                stream.WriteLine("public class SitecorePathfinderWebTestRunnerHandler : IHttpHandler");
                stream.WriteLine("{");
                stream.WriteLine("    public bool IsReusable");
                stream.WriteLine("    {");
                stream.WriteLine("        get { return false; }");
                stream.WriteLine("    }");
                stream.WriteLine();

                stream.WriteLine("    public void ProcessRequest(HttpContext context)");
                stream.WriteLine("    {");
                stream.WriteLine("       context.Response.ContentType = \"text/plain\";");
                stream.WriteLine("       var output = new StringWriter();");
                stream.WriteLine("       Console.SetOut(output);");
                stream.WriteLine("       var testName = context.Request.QueryString[\"test\"] ?? string.Empty;");
                stream.WriteLine();
                stream.WriteLine("       var compiler = new Sitecore.CodeDom.Compiler.CSharpCompiler();");
                stream.WriteLine("       var references = new StringCollection();");
                stream.WriteLine("       references.Add(Sitecore.IO.FileUtil.MapPath(\"/bin/nunit.framework.dll\"));");
                stream.WriteLine("       references.Add(\"System.Configuration.dll\");");
                stream.WriteLine();
                stream.WriteLine("       var assembly = compiler.FileToMemory(\"/sitecore/shell/client/Applications/Pathfinder/Tests/PathfinderTests.cs\", references);");
                stream.WriteLine("       var type = assembly.GetType(\"Sitecore.Pathfinder.Tests.PathfinderTests\");");
                stream.WriteLine("       var instance = Activator.CreateInstance(type);");
                stream.WriteLine();
                stream.WriteLine("       var methodInfo = type.GetMethod(testName);");
                stream.WriteLine("       methodInfo.Invoke(instance, null);");
                stream.WriteLine();
                stream.WriteLine("       Console.WriteLine(\"Passed: \" + testName);");
                stream.WriteLine("       context.Response.Write(HttpUtility.HtmlEncode(output.ToString()));");
                stream.WriteLine("    }");

                /*
                stream.WriteLine("       switch (testName)");
                stream.WriteLine("       {");

                var index = 0;
                foreach (var item in context.Project.Items.OfType<Item>())
                {
                    var testName = "Test" + index.ToString("#0") + "_" + item.ItemIdOrPath.GetSafeCodeIdentifier();

                    stream.WriteLine("           case \"" + testName + "\":");
                    stream.WriteLine("               " + testName + "();");
                    stream.WriteLine("               break;");

                    index++;
                }

                stream.WriteLine("             case \"all\":");
                index = 0;
                foreach (var item in context.Project.Items.OfType<Item>())
                {
                    var testName = "Test" + index.ToString("#0") + "_" + item.ItemIdOrPath.GetSafeCodeIdentifier();

                    stream.WriteLine("               " + testName + "();");

                    index++;
                }

                stream.WriteLine("               break;");
                stream.WriteLine("           default:");
                stream.WriteLine("               throw new InvalidOperationException(\"Test not found\");");
                stream.WriteLine("       }");
                stream.WriteLine("    }");

                index = 0;
                foreach (var item in context.Project.Items.OfType<Item>())
                {
                    var testName = "Test" + index.ToString("#0") + "_" + item.ItemIdOrPath.GetSafeCodeIdentifier();

                    stream.WriteLine();
                    stream.WriteLine("    public void " + testName + "()");
                    stream.WriteLine("    {");
                    stream.WriteLine("        var tests = new Sitecore.Pathfinder.Tests.PathfinderTests();");
                    stream.WriteLine("        tests." + testName + "();");
                    stream.WriteLine("    }");

                    index++;
                }
                */

                stream.WriteLine("}");
            }
        }

        private void GenerateUnitTestFile([NotNull] IBuildContext context, [NotNull] string directory)
        {
            var serverDirectory = Path.Combine(directory, "server");
            context.FileSystem.CreateDirectory(serverDirectory);

            var fileName = Path.Combine(serverDirectory, "PathfinderTest.Server.g.cs");

            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine("using System;");
                stream.WriteLine("using NUnit.Framework;");
                stream.WriteLine("using Sitecore;");
                stream.WriteLine("using Sitecore.Configuration;");
                stream.WriteLine();

                stream.WriteLine("namespace Sitecore.Pathfinder.Tests");
                stream.WriteLine("{");
                stream.WriteLine("    [TestFixture]");
                stream.WriteLine("    public class PathfinderTests");
                stream.WriteLine("    {");

                var index = 0;
                foreach (var item in context.Project.Items.OfType<Item>())
                {
                    var testName = "Test" + index.ToString("#0") + "_" + item.ItemIdOrPath.GetSafeCodeIdentifier();

                    stream.WriteLine("        [Test]");
                    stream.WriteLine("        public void " + testName + "()");
                    stream.WriteLine("        {");
                    stream.WriteLine("            var item = Factory.GetDatabase(\"" + item.DatabaseName + "\").GetItem(\"" + item.ItemIdOrPath + "\");");
                    stream.WriteLine("            Assert.IsNotNull(item);");
                    stream.WriteLine("        }");
                    stream.WriteLine();

                    index++;
                }

                stream.WriteLine("    }");
                stream.WriteLine("}");

            }
        }
    }
}