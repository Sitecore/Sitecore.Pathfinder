// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.Zip;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using File = Sitecore.Pathfinder.Projects.Files.File;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class UpdatePackageProjectEmitter : ProjectEmitterBase
    {
        [ItemNotNull, NotNull]
        private readonly List<string> _files = new List<string>();

        [ItemNotNull, NotNull]
        private readonly List<Item> _items = new List<Item>();

        [ImportingConstructor]
        public UpdatePackageProjectEmitter([NotNull] IConfiguration configuration, [NotNull] IFactory factory, [NotNull] ITraceService trace, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, trace, emitters)
        {
            Factory = factory;
            FileSystem = fileSystem;
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        public ZipWriter Zip { get; private set; }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanEmit(string format) => string.Equals(format, "update", StringComparison.OrdinalIgnoreCase);

        public override void Emit(IEmitContext context, IProject project)
        {
            var packageFileName = Configuration.GetString(Constants.Configuration.Output.Update.FileName, "package");
            if (!packageFileName.EndsWith(".update"))
            {
                packageFileName += ".update";
            }

            var outputDirectory = PathHelper.Combine(project.ProjectDirectory, Configuration.GetString(Constants.Configuration.Output.Directory));
            var fileName = Path.Combine(outputDirectory, packageFileName);

            FileSystem.CreateDirectoryFromFileName(fileName);

            using (Zip = new ZipWriter(fileName))
            {
                base.Emit(context, project);

                EmitVersion();
                EmitMetaData();
            }

            context.OutputFiles.Add(Factory.OutputFile(fileName));
        }

        public virtual void EmitFile([NotNull] IEmitContext context, [NotNull] File file)
        {
            var sourceFileAbsoluteFileName = file.Snapshot.SourceFile.AbsoluteFileName;

            var fileName = PathHelper.NormalizeFilePath(file.FilePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            _files.Add('/' + fileName);

            Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            Zip.AddEntry("addedfiles/" + NormalizeZipPath(fileName), sourceFileAbsoluteFileName);

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    var sb = new StringBuilder();
                    var settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };
                    using (var output = XmlWriter.Create(sb, settings))
                    {
                        output.WriteProcessingInstruction("xml", "version=\"1.0\"");

                        output.WriteStartElement("addFile");
                        output.WriteFullElementString("collisionbehavior", string.Empty);
                        output.WriteFullElementString("path", PathHelper.NormalizeFilePath("\\" + fileName));
                        output.WriteFullElementString("id", PathHelper.NormalizeFilePath("\\" + fileName));
                        output.WriteFullElementString("parent", string.Empty);
                        output.WriteFullElementString("file", string.Empty);
                        output.WriteFullElementString("hashCode", string.Empty);
                        output.WriteEndElement();
                    }

                    writer.WriteLine("version=2.0");
                    writer.WriteLine("xml=" + sb);
                }

                Zip.AddEntry("properties/addedfiles/" + NormalizeZipPath(fileName), stream.ToArray());
            }
        }

        public virtual void EmitItem([NotNull] IEmitContext context, [NotNull] Item item)
        {
            _items.Add(item);

            Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            var fileName = item.DatabaseName + PathHelper.NormalizeFilePath(item.ItemIdOrPath) + "_" + item.Uri.Guid.ToString("B");

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    item.WriteAsUpdatePackageXml(writer);
                }

                Zip.AddEntry("addeditems/" + NormalizeZipPath(fileName), stream.ToArray());
            }
        }

        protected virtual void EmitMediaFile([NotNull] IEmitContext context, [NotNull] MediaFile mediaFile)
        {
            if (!mediaFile.UploadMedia)
            {
                EmitFile(context, mediaFile);
                return;
            }

            var item = context.Project.Indexes.FindQualifiedItem<Item>(mediaFile.MediaItemUri);
            if (item == null)
            {
                Trace.TraceInformation(Msg.E1047, "No media item - skipping", mediaFile.Snapshot.SourceFile);
                return;
            }

            EmitFile(context, mediaFile);
            EmitItem(context, item);
        }

        protected virtual void EmitMetaData()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(Configuration.GetString(Constants.Configuration.Author, string.Empty));
                }

                Zip.AddEntry("metadata/sc_author.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_comment.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(Configuration.GetString(Constants.Configuration.License, string.Empty));
                }

                Zip.AddEntry("metadata/sc_license.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    var name = Configuration.GetString(Constants.Configuration.Name, string.Empty);
                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Sitecore Pathfinder Package";
                    }

                    writer.Write(name);
                }

                Zip.AddEntry("metadata/sc_name.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_poststep.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(Configuration.GetString(Constants.Configuration.Publisher, string.Empty));
                }

                Zip.AddEntry("metadata/sc_publisher.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(Configuration.GetString(Constants.Configuration.Description, string.Empty));
                }

                Zip.AddEntry("metadata/sc_readme.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(Configuration.GetString(Constants.Configuration.Version, string.Empty));
                }

                Zip.AddEntry("metadata/sc_version.txt", stream.ToArray());
            }
        }

        protected override void EmitProjectItems(IEmitContext context, IEnumerable<IProjectItem> projectItems, List<IEmitter> emitters, ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            var unemittedItems = new List<IProjectItem>(projectItems);

            foreach (var projectItem in projectItems)
            {
                if (projectItem is MediaFile mediaFile)
                {
                    EmitMediaFile(context, mediaFile);
                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is File file)
                {
                    EmitFile(context, file);
                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is Item item)
                {
                    var sourcePropertyBag = (ISourcePropertyBag) item;
                    if (item.IsEmittable || sourcePropertyBag.GetValue<string>("__origin_reason") == nameof(CreateItemsFromTemplates))
                    {
                        EmitItem(context, item);
                    }

                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is Template)
                {
                    unemittedItems.Remove(projectItem);
                }
            }

            base.EmitProjectItems(context, unemittedItems, emitters, retries);
        }

        protected virtual void EmitVersion()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write("10.00.000000.000000");
                }

                Zip.AddEntry("installer/version", stream.ToArray());
            }
        }

        [NotNull]
        private string NormalizeZipPath([NotNull] string fileName)
        {
            return fileName.Replace("\\", "/").TrimEnd('/');
        }
    }
}
