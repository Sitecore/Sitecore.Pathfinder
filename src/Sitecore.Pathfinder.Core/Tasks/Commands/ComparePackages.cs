// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
    public class ComparePackages : BuildTaskBase
    {
        [ImportingConstructor]
        public ComparePackages([NotNull] IFileSystem fileSystem, [NotNull] IProjectService projectService) : base("compare-projects")
        {
            FileSystem = fileSystem;
            ProjectService = projectService;
        }

        [NotNull]
        public string Header { get; set; } = string.Empty;

        [NotNull]
        protected IFileSystem FileSystem { get; }

        [NotNull]
        protected IProjectService ProjectService { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.Configuration.GetCommandLineArg(1);
            if (string.IsNullOrEmpty(projectDirectory))
            {
                context.Trace.WriteLine(Texts.Missing_project_directory_to_compare_with);
                return;
            }

            projectDirectory = PathHelper.Combine(Directory.GetCurrentDirectory(), projectDirectory);
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                context.Trace.WriteLine(Texts.Project_directory_not_found__ + projectDirectory);
                return;
            }

            var project1 = context.LoadProject();

            var project2 = ProjectService.LoadProjectFromNewHost(projectDirectory);
            if (project2 == null)
            {
                context.Trace.WriteLine(Texts.Could_not_load_project__ + projectDirectory);
                return;
            }

            Compare(context, project1, project2);
        }

        protected virtual void Compare([NotNull] IBuildContext context, [NotNull] Template template1, [NotNull] Template template2)
        {
            if (template1.IsImport && template2.IsImport)
            {
                return;
            }

            Header = template1.QualifiedName;

            WriteCompare(context, "Name [Template]", template1.ItemName, template2.ItemName);
            WriteCompare(context, "Path [Template]", template1.ItemIdOrPath, template2.ItemIdOrPath);
            WriteCompare(context, "BaseTemplate [Template]", template1.BaseTemplates, template2.BaseTemplates);
            WriteCompare(context, "ShortHelp [Template]", template1.ShortHelp, template2.ShortHelp);
            WriteCompare(context, "LongHelp [Template]", template1.LongHelp, template2.LongHelp);

            foreach (var section1 in template1.Sections)
            {
                Header = template1.QualifiedName;

                var section2 = template2.Sections.FirstOrDefault(s => s.Uri == section1.Uri);
                if (section2 == null)
                {
                    WriteHeader(context);
                    context.Trace.WriteLine($"+ {section1.SectionName} [Template Section]");
                    continue;
                }

                Header = template1.QualifiedName + "/" + section1.SectionName;

                WriteCompare(context, "Name [Template Section]", section1.SectionName, section2.SectionName);
                WriteCompare(context, "Icon [Template Section]", section1.Icon, section2.Icon);

                foreach (var field1 in section1.Fields)
                {
                    Header = template1.QualifiedName + "/" + section1.SectionName;

                    var field2 = section2.Fields.FirstOrDefault(f => f.Uri == field1.Uri);
                    if (field2 == null)
                    {
                        WriteHeader(context);
                        context.Trace.WriteLine($@"+ {field1.FieldName} [Templare Field]");
                        continue;
                    }

                    Header = template1.QualifiedName + "/" + section1.SectionName + "/" + field1.FieldName;

                    WriteCompare(context, "Name [Template Field]", field1.FieldName, field2.FieldName);
                    WriteCompare(context, "ShortHelp [Template Field]", field1.ShortHelp, field2.ShortHelp);
                    WriteCompare(context, "LongHelp [Template Field]", field1.LongHelp, field2.LongHelp);
                    WriteCompare(context, "Source [Template Field]", field1.Source, field2.Source);
                    WriteCompare(context, "Type [Template Field]", field1.Type, field2.Type);
                    WriteCompare(context, "Shared [Template Field]", field1.Shared.ToString(), field2.Shared.ToString());
                    WriteCompare(context, "Unversioned [Template Field]", field1.Unversioned.ToString(), field2.Unversioned.ToString());
                    WriteCompare(context, "SortOrder [Template Field]", field1.Sortorder.ToString(), field2.Sortorder.ToString());
                }

                foreach (var field2 in section2.Fields)
                {
                    var field1 = section1.Fields.FirstOrDefault(s => s.Uri == field2.Uri);
                    if (field1 == null)
                    {
                        context.Trace.WriteLine($"- {template2.QualifiedName}/{section2.SectionName}/{field2.FieldName} [Template Field]");
                    }
                }

            }

            foreach (var section2 in template2.Sections)
            {
                var section1 = template1.Sections.FirstOrDefault(s => s.Uri == section2.Uri);
                if (section1 == null)
                {
                    context.Trace.WriteLine($"- {template2.QualifiedName}/{section2.SectionName} [Template Section]");
                }
            }
        }

        protected virtual void Compare([NotNull] IBuildContext context, [NotNull] IProjectBase project1, [NotNull] IProjectBase project2)
        {
            foreach (var i in project1.ProjectItems.OrderBy(p => p.QualifiedName))
            {
                var projectItem1 = i;
                Header = projectItem1.QualifiedName;

                var projectItem2 = project2.Indexes.FindQualifiedItem<IProjectItem>(projectItem1.Uri);
                if (projectItem2 == null)
                {
                    var file = projectItem1 as Projects.Files.File;
                    var qualifiedName = file != null ? file.FilePath : projectItem1.QualifiedName;

                    context.Trace.WriteLine($"+ {qualifiedName} [{projectItem1.GetType().Name}]");
                    continue;
                }

                // if either project items are imported, treat them as if not existing
                var databaseProjectItem1 = projectItem1 as DatabaseProjectItem;
                if (databaseProjectItem1 != null && databaseProjectItem1.IsImport)
                {
                    projectItem1 = null;
                }

                var databaseProjectItem2 = projectItem2 as DatabaseProjectItem;
                if (databaseProjectItem2 != null && databaseProjectItem2.IsImport)
                {
                    projectItem2 = null;
                }

                if (projectItem1 == null && projectItem2 == null)
                {
                    continue;
                }

                // compare items
                var item1 = projectItem1 as Item;
                var item2 = projectItem2 as Item;
                if (item1 != null || item2 != null)
                {
                    if (item1 == null)
                    {
                        WriteHeader(context);
                        context.Trace.WriteLine($"- {projectItem2.QualifiedName} [Item]");
                        context.Trace.WriteLine($"+ {projectItem1.QualifiedName} [Item]");
                        continue;
                    }

                    if (item2 == null)
                    {
                        WriteHeader(context);
                        context.Trace.WriteLine($"- {projectItem2.QualifiedName} [Item]");
                        context.Trace.WriteLine($"+ {projectItem1.QualifiedName} [Item]");
                        continue;
                    }

                    Compare(context, item1, item2);

                    continue;
                }

                // compare templates
                var template1 = projectItem1 as Template;
                var template2 = projectItem2 as Template;
                if (template1 != null || template2 != null)
                {
                    if (template1 == null)
                    {
                        context.Trace.WriteLine($"- {projectItem2.QualifiedName} [Template]");
                        continue;
                    }

                    if (template2 == null)
                    {
                        context.Trace.WriteLine($"+ {projectItem1.QualifiedName} [Template]");
                        continue;
                    }

                    Compare(context, template1, template2);
                }

                var file1 = projectItem1 as Projects.Files.File;
                var file2 = projectItem2 as Projects.Files.File;
                if (file1 != null || file2 != null)
                {
                    if (file1 == null)
                    {
                        context.Trace.WriteLine($"- {file2.FilePath} [File]");
                        continue;
                    }

                    if (file2 == null)
                    {
                        context.Trace.WriteLine($"+ {file1.FilePath} [File]");
                    }
                }
            }

            foreach (var projectItem2 in project2.ProjectItems.OrderBy(p => p.QualifiedName))
            {
                var databaseProjectItem2 = projectItem2 as DatabaseProjectItem;
                if (databaseProjectItem2 != null && databaseProjectItem2.IsImport)
                {
                    continue;
                }

                Header = projectItem2.QualifiedName;

                var projectItem1 = project1.Indexes.FindQualifiedItem<IProjectItem>(projectItem2.Uri);
                if (projectItem1 == null)
                {
                    var file = projectItem2 as Projects.Files.File;
                    var qualifiedName = file != null ? file.FilePath : projectItem2.QualifiedName;

                    context.Trace.WriteLine($"- {qualifiedName} [{projectItem2.GetType().Name}]");
                }
            }
        }

        protected virtual void Compare([NotNull] IBuildContext context, [NotNull] Item item1, [NotNull] Item item2)
        {
            if (item1.IsImport && item2.IsImport)
            {
                return;
            }

            Header = item1.QualifiedName;

            WriteCompare(context, "Name [Item]", item1.ItemName, item2.ItemName);
            WriteCompare(context, "Path [Item]", item1.ItemIdOrPath, item2.ItemIdOrPath);
            WriteCompare(context, "Template [Item]", item1.TemplateName, item2.TemplateName);
            WriteCompare(context, "Icon [Item]", item1.Icon, item2.Icon);

            foreach (var field1 in item1.Fields.OrderBy(f => f.FieldName))
            {
                Header = item1.QualifiedName + "/" + field1.FieldName;

                var field2 = item2.Fields.FirstOrDefault(f => f.FieldId == field1.FieldId && f.Language == field1.Language && f.Version == field1.Version);
                if (field2 == null)
                {
                    context.Trace.WriteLine($@"+ {item1.QualifiedName}/{field1.FieldName} [Field]");
                    continue;
                }

                WriteCompare(context, "Value [Field]", field1.Value, field2.Value);
            }

            foreach (var field2 in item2.Fields.OrderBy(f => f.FieldName))
            {
                var field1 = item1.Fields.FirstOrDefault(f => f.FieldId == field2.FieldId && f.Language == field2.Language && f.Version == field2.Version);
                if (field1 == null)
                {
                    context.Trace.WriteLine($@"- {item2.QualifiedName}/{field2.FieldName} [Field]");
                }
            }
        }

        protected virtual void WriteCompare([NotNull] IBuildContext context, [NotNull] string text, [NotNull] string to, [NotNull] string from)
        {
            if (to == from)
            {
                return;
            }

            context.Trace.WriteLine(Header + @"/" + text);

            if (!string.IsNullOrEmpty(from))
            {
                context.Trace.WriteLine(@"- " + from);
            }

            if (!string.IsNullOrEmpty(to))
            {
                context.Trace.WriteLine(@"+ " + to);
            }
        }

        protected virtual void WriteHeader([NotNull] IBuildContext context)
        {
            if (string.IsNullOrEmpty(Header))
            {
                return;
            }

            context.Trace.WriteLine(string.Empty);
            context.Trace.WriteLine(Header);
            Header = string.Empty;
        }
    }
}
