// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.Zip;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class ExtractItems : BuildTaskBase
    {
        [ImportingConstructor]
        public ExtractItems([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem, [NotNull] IFactory factory, [NotNull] IProjectService projectService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IProjectEmitter> projectEmitters) : base("extract-items")
        {
            CompositionService = compositionService;
            Configuration = configuration;
            FileSystem = fileSystem;
            Factory = factory;
            ProjectService = projectService;
            ProjectEmitters = projectEmitters;
        }

        [NotNull, Option("format", Alias = "f", IsRequired = true, PromptText = "Select output format", HelpText = "Output format", PositionalArg = 2, HasOptions = true, DefaultValue = "unicorn")]
        public string Format { get; set; } = "unicorn";

        [NotNull, Option("package", Alias = "p", PositionalArg = 1, PromptText = "Package (.zip)")]
        public string PackageFileName { get; set; }

        [ItemNotNull, NotNull]
        public IEnumerable<IProjectEmitter> ProjectEmitters { get; }

        [NotNull]
        public IProjectService ProjectService { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var fileName = PathHelper.Combine(Directory.GetCurrentDirectory(), PackageFileName);
            if (!FileSystem.FileExists(fileName))
            {
                context.Trace.TraceError("Package not found", fileName);
                return;
            }

            var options = Factory.ProjectOptions(Configuration.GetString(Constants.Configuration.Database));
            var project = ProjectService.LoadProject(options, Enumerable.Empty<string>());

            LoadPackage(context, project, fileName);

            project.Compile();

            PublishProject(context, project);
        }

        protected virtual void ExtractItem([NotNull] IProject project, [NotNull] ZipEntry zipEntry)
        {
            using (var stream = zipEntry.GetStream())
            {
                var doc = XDocument.Load(stream);

                var root = doc.Root;
                if (root == null)
                {
                    return;
                }

                ExtractItem(project, zipEntry.Name, root);
            }
        }

        protected virtual void ExtractItem([NotNull] IProject project, [NotNull] string entryName, [NotNull] XElement element)
        {
            var parts = entryName.Split('/');
            var databaseName = parts[1];
            var itemIdOrPath = '/' + string.Join("/", parts.Take(parts.Length - 4).Skip(2));

            var itemName = element.GetAttributeValue("name");
            var guid = Guid.Parse(element.GetAttributeValue("id"));
            var templateIdOrPath = element.GetAttributeValue("tid");

            var database = project.GetDatabase(databaseName);
            var item = Factory.Item(database, guid, itemName, itemIdOrPath, templateIdOrPath);
            item.IsEmittable = true;

            item.Sortorder = int.Parse(element.GetAttributeValue("sortorder"));

            var language = Factory.Language(element.GetAttributeValue("language"));
            var version = Factory.Version(int.Parse(element.GetAttributeValue("version")));

            var fieldsElement = element.Element("fields");
            if (fieldsElement != null)
            {
                foreach (var fieldElement in fieldsElement.Elements())
                {
                    var field = Factory.Field(item, fieldElement.GetAttributeValue("key"), fieldElement.GetElementValue("content"));
                    field.FieldId = Guid.Parse(fieldElement.GetAttributeValue("tfid"));
                    field.Language = language;
                    field.Version = version;

                    item.Fields.Add(field);
                }
            }

            project.AddOrMerge(item);
        }

        [NotNull, OptionValues("Format")]
        protected IEnumerable<(string Name, string Value)> GetFormatOptions([NotNull] ITaskContext context)
        {
            // remember to update PublishProject.cs as well
            yield return ("Package", "package");
            yield return ("Nuget", "nuget");
            yield return ("Unicorn", "unicorn");
            yield return ("Update", "update");
            yield return ("Yaml", "yaml");
            yield return ("Json", "json");
            yield return ("Xml", "xml");
            yield return ("Serialization", "serialization");
        }

        protected virtual void LoadPackage([NotNull] IBuildContext context, [NotNull] IProject project, [NotNull] string fileName)
        {
            context.Trace.TraceInformation(Msg.D1029, "Loading package...");

            using (var zipReader = OpenPackageFile(fileName))
            {
                foreach (var zipEntry in zipReader.Entries)
                {
                    if (zipEntry.Name.StartsWith("items/"))
                    {
                        ExtractItem(project, zipEntry);
                    }
                }
            }
        }

        [NotNull]
        protected virtual ZipReader OpenPackageFile([NotNull] string fileName)
        {
            var zipReader = new ZipReader(fileName, Encoding.UTF8);

            var entry = zipReader.GetEntry("package.zip");
            if (entry == null)
            {
                return zipReader;
            }

            var tempFileName = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFileName))
            {
                entry.GetStream().CopyTo(fileStream);
            }

            zipReader.Dispose();

            return new ZipReader(tempFileName, Encoding.UTF8);
        }

        protected virtual void PublishProject([NotNull] IBuildContext context, [NotNull] IProject project)
        {
            context.Trace.TraceInformation(Msg.D1029, "Publishing project...");

            var format = Format;
            if (string.IsNullOrEmpty(format))
            {
                format = context.Configuration.GetString(Constants.Configuration.Output.Format, "package");
            }

            var projectEmitters = ProjectEmitters.Where(p => p.CanEmit(format)).ToArray();
            if (!projectEmitters.Any())
            {
                context.Trace.TraceError(Msg.E1043, "No project emitters found");
                return;
            }

            foreach (var projectEmitter in projectEmitters)
            {
                var emitContext = CompositionService.Resolve<IEmitContext>().With(projectEmitter, project);

                projectEmitter.Emit(emitContext, project);

                context.OutputFiles.AddRange(emitContext.OutputFiles);
            }
        }
    }
}
