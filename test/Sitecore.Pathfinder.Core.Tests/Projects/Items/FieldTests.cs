// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using System;
using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Projects.Items
{
    [TestFixture]
    public class FieldTests
    {
        [Test]
        public void Compile_NullCompilers()
        {
            var context = CreateContext(null);
            var field = new Field(Item.Empty, null) {Value = "Lorem Ipsum"};

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.False);
        }

        [Test]
        public void Compile_EmptyCompilers()
        {
            var context = CreateContext(new IFieldCompiler[0]);
            var field = new Field(Item.Empty, null) {Value = "Lorem Ipsum"};

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.False);
        }

        [Test]
        public void Compile_NoMatchingCompiler()
        {
            var compilers = new IFieldCompiler[] { new CheckboxFieldCompiler() };
            var project = new Project(null, null, null, null, null, null);
            var template = CreateTemplate(project);
            var context = CreateContext(compilers);

            var item = new Item(project, TextNode.Empty, Guid.NewGuid(), "master", "item", "/sitecore/item", template.ItemIdOrPath);
            project.AddOrMerge(item);

            var field = new Field(item, null)
            {
                FieldName = "Text",
                Value = "Lorem Ipsum"
            };

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.False);
        }

        [Test]
        public void Compile_MatchingCompiler()
        {
            var compilers = new IFieldCompiler[] { new CheckboxFieldCompiler() };
            var project = new Project(null, null, null, null, null, null);
            var template = CreateTemplate(project);
            var context = CreateContext(compilers);

            var item = new Item(project, TextNode.Empty, Guid.NewGuid(), "master", "item", "/sitecore/item", template.ItemName);
            project.AddOrMerge(item);

            var field = new Field(item, null)
            {
                FieldName = "Checkbox",
                Value = "True"
            };

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.True);
            Assert.That(field.CompiledValue, Is.EqualTo("1"));
        }

        [Test]
        public void Compile_ExclusiveCompiler()
        {
            var compilers = new IFieldCompiler[] { new CheckboxFieldCompiler(), new ReplaceCompiler("alpha")  };
            var project = new Project(null, null, null, null, null, null);
            var template = CreateTemplate(project);
            var context = CreateContext(compilers);

            var item = new Item(project, TextNode.Empty, Guid.NewGuid(), "master", "item", "/sitecore/item", template.ItemName);
            project.AddOrMerge(item);

            var field = new Field(item, null)
            {
                FieldName = "Checkbox",
                Value = "True"
            };

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.True);
            Assert.That(field.CompiledValue, Is.EqualTo("1"));
        }

        [Test]
        public void Compile_NonExclusiveCompiler()
        {
            var compilers = new IFieldCompiler[] { new ReplaceCompiler("alpha"), new ReplaceCompiler("beta"),  };
            var project = new Project(null, null, null, null, null, null);
            var template = CreateTemplate(project);
            var context = CreateContext(compilers);

            var item = new Item(project, TextNode.Empty, Guid.NewGuid(), "master", "item", "/sitecore/item", template.ItemName);
            project.AddOrMerge(item);

            var field = new Field(item, null)
            {
                FieldName = "Text",
                Value = "True"
            };

            field.Compile(context);

            Assert.That(field.IsCompiled, Is.True);
            Assert.That(field.CompiledValue, Is.EqualTo("beta"));
        }

        [NotNull]
        private IFieldCompileContext CreateContext([CanBeNull] IFieldCompiler[] compilers)
        {
            var config = new Microsoft.Framework.ConfigurationModel.Configuration();
            return new FieldCompileContext(config, null, null, null, compilers);
        }

        [NotNull]
        private Template CreateTemplate([NotNull] IProject project)
        {
            var template = new Template(project, TextNode.Empty, Guid.NewGuid(), "master", "dummy template", Guid.NewGuid().ToString());
            var stringField = new TemplateField(template, Guid.NewGuid(), null)
            {
                Type = "Single-Line Text",
                FieldName = "Text"
            };

            var checkboxField = new TemplateField(template, Guid.NewGuid(), null)
            {
                Type = "Checkbox",
                FieldName = "Checkbox"
            };

            var section = new TemplateSection(template, Guid.NewGuid(), null);
            section.Fields.Add(stringField);
            section.Fields.Add(checkboxField);

            template.Sections.Add(section);

            project.AddOrMerge(template);

            return template;
        }

        private class ReplaceCompiler : FieldCompilerBase
        {
            public string Value { get; set; }

            [ImportingConstructor]
            public ReplaceCompiler(string value) : base(Constants.FieldCompilers.Low)
            {
                Value = value;
            }

            public override bool CanCompile(IFieldCompileContext context, Field field)
            {
                return true;
            }

            public override string Compile(IFieldCompileContext context, Field field)
            {
                return Value;
            }
        }
    }
}
