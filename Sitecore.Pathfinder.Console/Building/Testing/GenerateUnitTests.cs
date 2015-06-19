// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

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

            var directory = Path.Combine(context.SolutionDirectory, context.Configuration.Get(Constants.Configuration.LocalTestDirectory));
            context.FileSystem.CreateDirectory(directory);

            GenerateUnitTestFile(context, directory);
            GenerateLocalTestRunnerFile(context, directory);
        }

        private void GenerateBinFileTests([NotNull] IBuildContext context, [NotNull] StreamWriter stream, ref int index)
        {
            foreach (var binFile in context.Project.Items.OfType<BinFile>())
            {
                var testName = GetTestName(binFile.FilePath, index, "File");

                stream.WriteLine("        [Test]");
                stream.WriteLine("        public void " + testName + "()");
                stream.WriteLine("        {");
                stream.WriteLine("            Assert.IsTrue(File.Exists(FileUtil.MapPath(\"" + binFile.FilePath + "\")));");
                stream.WriteLine("        }");
                stream.WriteLine();

                index++;
            }
        }

        private void GenerateContentFileTests([NotNull] IBuildContext context, [NotNull] StreamWriter stream, ref int index)
        {
            foreach (var contentFile in context.Project.Items.OfType<ContentFile>())
            {
                var testName = GetTestName(contentFile.FilePath, index, "File");

                stream.WriteLine("        [Test]");
                stream.WriteLine("        public void " + testName + "()");
                stream.WriteLine("        {");
                stream.WriteLine("            Assert.IsTrue(File.Exists(FileUtil.MapPath(\"" + contentFile.FilePath + "\")));");
                stream.WriteLine("        }");
                stream.WriteLine();

                index++;
            }
        }

        private void GenerateTemplateTests([NotNull] IBuildContext context, [NotNull] StreamWriter stream, ref int index)
        {
            foreach (var template in context.Project.Items.OfType<Template>())
            {
                var testName = GetTestName(template.ItemIdOrPath, index, "Template");

                stream.WriteLine("        [Test]");
                stream.WriteLine("        public void " + testName + "()");
                stream.WriteLine("        {");
                stream.WriteLine("            var database = Factory.GetDatabase(\"" + template.DatabaseName + "\");");
                stream.WriteLine("            var item = database.GetItem(\"" + template.ItemIdOrPath + "\");");
                stream.WriteLine("            Assert.IsNotNull(item);");
                stream.WriteLine("            var template = TemplateManager.GetTemplate(item.ID, database);");
                stream.WriteLine("            Assert.IsNotNull(template);");
                stream.WriteLine("            Assert.AreEqual(\"" + template.ItemName.Value + "\", template.Name);");

                if (!string.IsNullOrEmpty(template.Icon.Value))
                {
                    stream.WriteLine("            Assert.AreEqual(\"" + template.Icon.Value + "\", template.Icon);");
                }

                stream.WriteLine("        }");
                stream.WriteLine();

                index++;
            }
        }

        private void GenerateItemTests([NotNull] IBuildContext context, [NotNull] StreamWriter stream, ref int index)
        {
            foreach (var item in context.Project.Items.OfType<Item>())
            {
                if (!item.IsEmittable)
                {
                    continue;
                }

                var testName = GetTestName(item.ItemIdOrPath, index, "Item");

                stream.WriteLine("        [Test]");
                stream.WriteLine("        public void " + testName + "()");
                stream.WriteLine("        {");
                stream.WriteLine("            var database = Factory.GetDatabase(\"" + item.DatabaseName + "\");");
                stream.WriteLine("            var item = database.GetItem(\"" + item.ItemIdOrPath + "\");");
                stream.WriteLine("            Assert.IsNotNull(item);");
                stream.WriteLine("            Assert.AreEqual(\"" + item.ItemName.Value + "\", item.Name);");

                if (!string.IsNullOrEmpty(item.TemplateIdOrPath.Value))
                {
                    Guid guid;
                    if (Guid.TryParse(item.TemplateIdOrPath.Value, out guid))
                    {
                        stream.WriteLine("            Assert.AreEqual(\"" + item.TemplateIdOrPath.Value + "\", item.TemplateID.ToString());");
                    }
                    else
                    {
                        stream.WriteLine("            Assert.AreEqual(\"" + item.TemplateIdOrPath.Value + "\", database.GetItem(item.TemplateID).Paths.Path);");
                    }
                }

                if (!string.IsNullOrEmpty(item.Icon.Value))
                {
                    stream.WriteLine("            Assert.AreEqual(\"" + item.Icon.Value + "\", item.Appearance.Icon);");
                }

                var sharedFields = item.Fields.Where(f => f.IsTestable && string.IsNullOrEmpty(f.Language.Value) && f.Version.Value == 0).ToList();
                var versionedFields = item.Fields.Where(f => f.IsTestable && (!string.IsNullOrEmpty(f.Language.Value) || f.Version.Value != 0)).ToList();

                foreach (var field in sharedFields)
                {
                    stream.WriteLine("            Assert.AreEqual(\"" + field.Value.Value + "\", item[\"" + field.FieldName.Value + "\"]);");
                }

                if (versionedFields.Any())
                {
                    stream.WriteLine("            var versions = item.Versions.GetVersions(true);");

                    foreach (var field in versionedFields)
                    {
                        stream.Write("            Assert.AreEqual(\"" + field.Value.Value + "\", versions.First(v => ");

                        if (!string.IsNullOrEmpty(field.Language.Value) && field.Version.Value != 0)
                        {
                            stream.Write("v.Language == \"" + field.Language.Value + "\" && v.Version == " + field.Version.Value);
                        }
                        else if (!string.IsNullOrEmpty(field.Language.Value))
                        {
                            stream.Write("v.Language == \"" + field.Language.Value + "\"");
                        }
                        else if (field.Version.Value != 0)
                        {
                            stream.Write("v.Version == " + field.Version.Value);
                        }

                        stream.WriteLine(")[\"" + field.FieldName.Value + "\"]);");
                    }
                }

                stream.WriteLine("        }");
                stream.WriteLine();

                index++;
            }
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
                    var testName = GetTestName(item.ItemIdOrPath, index, "Item");

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

        private void GenerateUnitTestFile([NotNull] IBuildContext context, [NotNull] string directory)
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
                stream.WriteLine("using Sitecore.IO;");
                stream.WriteLine();

                stream.WriteLine("namespace Sitecore.Pathfinder.Tests");
                stream.WriteLine("{");
                stream.WriteLine("    [TestFixture]");
                stream.WriteLine("    public class PathfinderTests");
                stream.WriteLine("    {");

                var index = 0;

                GenerateItemTests(context, stream, ref index);
                GenerateTemplateTests(context, stream, ref index);
                GenerateContentFileTests(context, stream, ref index);
                GenerateBinFileTests(context, stream, ref index);

                stream.WriteLine("    }");
                stream.WriteLine("}");
            }
        }

        [NotNull]
        private string GetTestName([NotNull] string itemIdOrPath, int index, [NotNull] string type)
        {
            return "Test" + index.ToString("000") + itemIdOrPath.Replace("/", "_").GetSafeCodeIdentifier() + "_" + type;
        }
    }
}
