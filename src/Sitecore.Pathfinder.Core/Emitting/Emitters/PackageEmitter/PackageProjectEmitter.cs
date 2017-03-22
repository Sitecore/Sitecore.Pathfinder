// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.Zip;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Emitting.Emitters.PackageEmitter
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class PackageProjectEmitter : ProjectEmitterBase
    {
        [ItemNotNull, NotNull]
        private readonly List<string> _files = new List<string>();

        [ItemNotNull, NotNull]
        private readonly List<Item> _items = new List<Item>();

        [ImportingConstructor]
        public PackageProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystemService fileSystem) : base(configuration, compositionService, traceService, emitters)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        public ZipWriter Zip { get; protected set; }

        public virtual void AddFile([NotNull] IEmitContext context, [NotNull] string sourceFileAbsoluteFileName, [NotNull] string filePath)
        {
            var fileName = PathHelper.NormalizeFilePath(filePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            _files.Add('/' + fileName);

            context.Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            Zip.AddEntry("files/" + NormalizeZipPath(fileName), sourceFileAbsoluteFileName);

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("type=file");
                }

                Zip.AddEntry("properties/files/" + NormalizeZipPath(fileName), stream.ToArray());
            }
        }

        [NotNull]
        private string NormalizeZipPath([NotNull] string fileName)
        {
            return fileName.Replace("\\", "/").TrimEnd('/');
        }

        public virtual void AddItem([NotNull] IEmitContext context, [NotNull] Item item)
        {
            _items.Add(item);

            context.Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            var languages = item.GetLanguages().ToList();
            if (!languages.Any())
            {
                // todo: only shared fields - replace language with something else
                languages = new List<Language>
                {
                    new Language("en")
                };
            }

            foreach (var language in languages)
            {
                var versions = item.GetVersions(language).ToList();
                if (!versions.Any())
                {
                    versions = new List<Projects.Items.Version>
                    {
                        new Projects.Items.Version(1)
                    };
                }

                foreach (var version in versions)
                {
                    var fileName = item.DatabaseName + PathHelper.NormalizeFilePath(item.ItemIdOrPath) + "\\" + item.Uri.Guid.Format() + "\\" + language.LanguageName + "\\" + version.Number + "\\xml";
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            var settings = new XmlWriterSettings
                            {
                                Encoding = Encoding.UTF8,
                                Indent = true
                            };

                            using (var output = XmlWriter.Create(writer, settings))
                            {
                                output.WriteStartElement("item");
                                output.WriteAttributeString("name", item.ItemName);
                                output.WriteAttributeString("key", item.ItemName.ToLowerInvariant());
                                output.WriteAttributeString("id", item.Uri.Guid.Format());
                                output.WriteAttributeString("tid", item.Template.Uri.Guid.Format());
                                output.WriteAttributeString("mid", "{00000000-0000-0000-0000-000000000000}");
                                output.WriteAttributeString("sortorder", item.Sortorder.ToString());
                                output.WriteAttributeString("language", language.LanguageName);
                                output.WriteAttributeString("version", version.ToString());
                                output.WriteAttributeString("template", item.TemplateName.ToLowerInvariant());
                                output.WriteAttributeString("parentid", item.GetParent().Uri.Guid.Format());

                                output.WriteStartElement("fields");

                                foreach (var field in item.Fields.GetFields(language, version))
                                {
                                    output.WriteStartElement("field");
                                    output.WriteAttributeString("tfid", field.FieldId.Format());
                                    output.WriteAttributeString("key", field.FieldName.ToLowerInvariant());
                                    output.WriteAttributeString("type", field.TemplateField.Type);

                                    output.WriteStartElement("content");
                                    output.WriteValue(field.CompiledValue);
                                    output.WriteEndElement();
                                    output.WriteEndElement();
                                }

                                output.WriteEndElement();
                                output.WriteEndElement();
                            }

                            Zip.AddEntry("items/" + NormalizeZipPath(fileName), stream.ToArray());
                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            writer.WriteLine("database=" + item.DatabaseName);
                            writer.WriteLine("id=" + item.Uri.Guid.Format());
                            writer.WriteLine("language=" + language.LanguageName);
                            writer.WriteLine("version=" + version.Number);
                            writer.WriteLine("revision=" + Guid.NewGuid().ToString("D"));
                            writer.WriteLine("fieldproperties=" + string.Join("|", item.Fields.GetFields(language, version).Select(f => f.FieldId.Format() + ":" + (f.TemplateField.Shared ? "Shared" : f.TemplateField.Unversioned ? "Unversioned" : "Versioned"))));
                        }

                        Zip.AddEntry("properties/items/" + NormalizeZipPath(fileName), stream.ToArray());
                    }
                }
            }
        }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "package", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IProject project)
        {
            var outputDirectory = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory));
            var fileName = Path.Combine(outputDirectory, "sitecore.package.zip");

            FileSystem.CreateDirectoryFromFileName(fileName);

            using (Zip = new ZipWriter(fileName))
            {
                base.Emit(project);

                AddProject();
                AddVersion();
                AddMetaData();
            }
        }

        protected virtual void AddMetaData()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
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
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_license.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
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
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_publisher.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_readme.txt", stream.ToArray());
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.Write(string.Empty);
                }

                Zip.AddEntry("metadata/sc_version.txt", stream.ToArray());
            }
        }

        protected virtual void AddProject()
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var stream = new MemoryStream())
            {
                using (var output = XmlWriter.Create(stream, settings))
                {
                    output.WriteStartElement("project");

                    output.WriteStartElement("Metadata");
                    output.WriteStartElement("metadata");
                    output.WriteElementString("PackageName", "Pathfinder");
                    output.WriteElementString("Author", "Author");
                    output.WriteElementString("Version", "1.0.0");
                    output.WriteElementString("Revision", string.Empty);
                    output.WriteElementString("License", string.Empty);
                    output.WriteElementString("Comment", string.Empty);
                    output.WriteElementString("Attributes", string.Empty);
                    output.WriteElementString("Readme", string.Empty);
                    output.WriteElementString("Publisher", string.Empty);
                    output.WriteElementString("PostStep", string.Empty);
                    output.WriteElementString("PackageID", string.Empty);
                    output.WriteEndElement();
                    output.WriteEndElement();

                    output.WriteElementString("SaveProject", "True");

                    output.WriteStartElement("Sources");

                    // output.WriteStartElement("x");
                    // output.WriteRaw("    <>");
                    // output.WriteStartElement("Sources");

                    output.WriteStartElement("xitems");
                    output.WriteStartElement("Entries");
                    foreach (var item in _items)
                    {
                        output.WriteElementString("x-item", "/" + item.DatabaseName + item.ItemIdOrPath + "/" + item.Uri.Guid.Format() + "/invariant/0");
                    }

                    output.WriteEndElement();
                    output.WriteElementString("SkipVersions", "False");
                    output.WriteStartElement("Converter");
                    output.WriteStartElement("ItemToEntryConverter");
                    output.WriteStartElement("Transforms");
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteElementString("Include", string.Empty);
                    output.WriteElementString("Exclude", string.Empty);
                    output.WriteElementString("Name", "Items");
                    output.WriteEndElement();

                    output.WriteStartElement("xfiles");
                    output.WriteStartElement("Entries");
                    foreach (var file in _files)
                    {
                        output.WriteElementString("x-item", file);
                    }

                    output.WriteEndElement();
                    output.WriteStartElement("Converter");
                    output.WriteStartElement("FileToEntryConverter");
                    output.WriteStartElement("Root");
                    output.WriteEndElement();
                    output.WriteStartElement("Transforms");
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteElementString("Include", string.Empty);
                    output.WriteElementString("Exclude", string.Empty);
                    output.WriteElementString("Name", "Files");

                    output.WriteEndElement();

                    // output.WriteEndElement();
                    // output.WriteRaw("    </>");
                    output.WriteEndElement();

                    output.WriteStartElement("Converter");
                    output.WriteStartElement("TrivialConverter");
                    output.WriteStartElement("Transforms");
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteEndElement();
                    output.WriteElementString("Include", string.Empty);
                    output.WriteElementString("Exclude", string.Empty);
                    output.WriteElementString("Name", string.Empty);

                    output.WriteEndElement();
                }

                Zip.AddEntry("installer/project", stream.ToArray());
            }
        }

        protected virtual void AddVersion()
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
    }
}
