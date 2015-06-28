// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Building.Testing.UnitTests.Pipelines
{
    public class GenerateTemplateUnitTests : PipelineProcessorBase<GenerateUnitTestsPipeline>
    {
        public GenerateTemplateUnitTests() : base(3000)
        {
        }

        protected override void Process(GenerateUnitTestsPipeline pipeline)
        {
            foreach (var template in pipeline.Context.Project.Items.OfType<Template>())
            {
                var testName = pipeline.GetTestName(template.ItemIdOrPath, pipeline.Index, "Template");
                pipeline.Tests.Add(testName);

                pipeline.Stream.WriteLine("        [Test]");
                pipeline.Stream.WriteLine("        public void " + testName + "()");
                pipeline.Stream.WriteLine("        {");
                pipeline.Stream.WriteLine("            var database = Factory.GetDatabase(\"" + template.DatabaseName + "\");");
                pipeline.Stream.WriteLine("            var item = database.GetItem(\"" + template.ItemIdOrPath + "\");");
                pipeline.Stream.WriteLine("            Assert.IsNotNull(item);");
                pipeline.Stream.WriteLine("            var template = TemplateManager.GetTemplate(item.ID, database);");
                pipeline.Stream.WriteLine("            Assert.IsNotNull(template);");
                pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + template.ItemName.Value + "\", template.Name);");

                if (!string.IsNullOrEmpty(template.Icon.Value))
                {
                    pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + template.Icon.Value + "\", template.Icon);");
                }

                if (!string.IsNullOrEmpty(template.BaseTemplates))
                {
                    pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + template.BaseTemplates + "\", item[FieldIDs.BaseTemplate]);");
                }

                if (!string.IsNullOrEmpty(template.ShortHelp))
                {
                    pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + template.ShortHelp + "\", item.Help.ToolTip);");
                }

                if (!string.IsNullOrEmpty(template.LongHelp))
                {
                    pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + template.LongHelp + "\", item.Help.Text);");
                }

                pipeline.Stream.WriteLine();
                pipeline.Stream.WriteLine("            var standardValuesItem = database.GetItem(item[FieldIDs.BaseTemplate]);");
                pipeline.Stream.WriteLine("            Assert.IsNotNull(standardValuesItem);");
                pipeline.Stream.WriteLine();
                pipeline.Stream.WriteLine("            Item sectionItem;");
                pipeline.Stream.WriteLine("            Item fieldItem;");

                foreach (var templateSection in template.Sections)
                {
                    pipeline.Stream.WriteLine();
                    pipeline.Stream.WriteLine("            sectionItem = item.Children[\"" + templateSection.SectionName.Value + "\"];");
                    pipeline.Stream.WriteLine("            Assert.IsNotNull(sectionItem);");
                    pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateSection.SectionName.Value + "\", sectionItem.Name);");

                    if (!string.IsNullOrEmpty(templateSection.Icon))
                    {
                        pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateSection.Icon + "\", sectionItem.Appearance.Icon);");
                    }

                    foreach (var templateField in templateSection.Fields)
                    {
                        pipeline.Stream.WriteLine();
                        pipeline.Stream.WriteLine("            fieldItem = sectionItem.Children[\"" + templateField.FieldName.Value + "\"];");
                        pipeline.Stream.WriteLine("            Assert.IsNotNull(fieldItem);");
                        pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.FieldName.Value + "\", fieldItem.Name);");

                        if (!string.IsNullOrEmpty(templateField.ShortHelp))
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.ShortHelp + "\", fieldItem.Help.ToolTip);");
                        }

                        if (!string.IsNullOrEmpty(templateField.LongHelp))
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.LongHelp + "\", fieldItem.Help.Text);");
                        }

                        if (templateField.Shared)
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"1\", fieldItem[TemplateFieldIDs.Shared]);");
                        }

                        if (templateField.Unversioned)
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"1\", fieldItem[TemplateFieldIDs.Unversioned]);");
                        }

                        if (templateField.SortOrder != 0)
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.SortOrder + "\", fieldItem[FieldIDs.Sortorder]);");
                        }

                        if (!string.IsNullOrEmpty(templateField.Source))
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.Source + "\", fieldItem[TemplateFieldIDs.Source]);");
                        }

                        if (!string.IsNullOrEmpty(templateField.Type))
                        {
                            pipeline.Stream.WriteLine("            Assert.AreEqual(\"" + templateField.Type + "\", fieldItem[TemplateFieldIDs.Type]);");
                        }
                    }
                }

                pipeline.Stream.WriteLine("        }");
                pipeline.Stream.WriteLine();

                pipeline.Index++;
            }
        }
    }
}
