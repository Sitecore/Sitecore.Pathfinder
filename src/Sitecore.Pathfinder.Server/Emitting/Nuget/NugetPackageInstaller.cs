// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NuGet;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.IO;
using Sitecore.Pathfinder.Emitting.Parsing;
using Sitecore.Pathfinder.Emitting.Writers;
using Sitecore.Publishing;
using Sitecore.Xml;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Emitting.Nuget
{
    public class NugetPackageInstaller
    {
        private string _fileName;

        public void Install([NotNull] string fileName)
        {
            _fileName = fileName + ".log";

            try
            {
                File.Delete(_fileName);

                string packageId;
                string version;
                ExtractPackageId(fileName, out packageId, out version);
                if (string.IsNullOrEmpty(packageId))
                {
                    TraceError("Failed to extract package ID");
                    return;
                }

                var directory = FileUtil.MapDataFilePath("pathfinder") + "\\.nuget";
                var localRepository = GetLocalRepository(directory);

                var repositories = GetRepositoriesFromConfig().ToList();
                repositories.Insert(0, Path.GetDirectoryName(fileName));

                var remoteRepository = GetRemoteRepository(repositories);

                var package = remoteRepository.FindPackage(packageId);
                if (package == null)
                {
                    TraceError("Package not found", packageId);
                    return;
                }

                var packageManager = new PackageManager(remoteRepository, localRepository.Source);
                packageManager.PackageInstalled += InstallPackage;

                packageManager.InstallPackage(package, false, true);

                TraceInformation("OK");
            }
            catch (Exception ex)
            {
                TraceError(ex.Message, ex.StackTrace);
            }
        }

        protected virtual bool CanCopyBinFile([NotNull] string sourceFileName, [NotNull] string destinationFileName)
        {
            if (!File.Exists(destinationFileName))
            {
                return true;
            }

            var destinationVersion = GetFileVersion(destinationFileName);
            var sourceVersion = GetFileVersion(sourceFileName);

            if (sourceVersion <= destinationVersion)
            {
                return false;
            }

            var destinationFileInfo = new FileInfo(destinationFileName);
            var sourceFileInfo = new FileInfo(sourceFileName);

            if (sourceFileInfo.Length == destinationFileInfo.Length && sourceFileInfo.LastWriteTimeUtc <= destinationFileInfo.LastWriteTimeUtc)
            {
                return false;
            }

            return true;
        }

        protected virtual void ExtractPackageId([NotNull] string fileName, out string packageId, out string version)
        {
            packageId = string.Empty;
            version = string.Empty;

            if (!File.Exists(fileName))
            {
                return;
            }

            using (var zip = new ZipReader(fileName))
            {
                foreach (var entry in zip.Entries)
                {
                    if (!entry.Name.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    using (var reader = new StreamReader(entry.GetStream()))
                    {
                        var xml = reader.ReadToEnd();

                        var doc = XDocument.Parse(xml);
                        var root = doc.Root;
                        if (root == null)
                        {
                            return;
                        }

                        var metadata = root.Element(XName.Get("metadata", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"));
                        if (metadata == null)
                        {
                            return;
                        }

                        packageId = metadata.Element(XName.Get("id", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"))?.Value ?? string.Empty;
                        version = metadata.Element(XName.Get("version", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"))?.Value ?? string.Empty;

                        return;
                    }
                }
            }
        }

        [NotNull]
        protected virtual Version GetFileVersion([NotNull] string fileName)
        {
            var info = FileVersionInfo.GetVersionInfo(fileName);
            return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
        }

        protected virtual IPackageRepository GetLocalRepository([NotNull] string directory)
        {
            Directory.CreateDirectory(directory);
            return PackageRepositoryFactory.Default.CreateRepository(directory);
        }

        protected virtual IPackageRepository GetRemoteRepository([NotNull] IEnumerable<string> feeds)
        {
            var repositories = feeds.Select(feed => PackageRepositoryFactory.Default.CreateRepository(feed));

            return new AggregateRepository(repositories);
        }

        protected virtual IEnumerable<string> GetRepositoriesFromConfig()
        {
            foreach (XmlNode configNode in Factory.GetConfigNodes("pathfinder/nuget-repositories/repository"))
            {
                yield return XmlUtil.GetAttribute("url", configNode);
            }
        }

        protected virtual void InstallItems([NotNull] XElement root)
        {
            var itemsElement = root.Element("items");
            if (itemsElement != null)
            {
                foreach (var itemElement in itemsElement.Elements())
                {
                    var item = Item.Parse(itemElement);
                    new ItemWriter(item).Write();
                }
            }
        }

        protected virtual void InstallPackage(object sender, PackageOperationEventArgs e)
        {
            ProcessItems(e.InstallPath);
            ProcessFiles(e.InstallPath);
        }

        protected virtual void InstallTemplates([NotNull] XElement root)
        {
            var templatesElement = root.Element("templates");
            if (templatesElement != null)
            {
                foreach (var templateElement in templatesElement.Elements())
                {
                    var template = Template.Parse(templateElement);
                    new TemplateWriter(template).Write();
                }
            }
        }

        protected virtual void ProcessItems([NotNull] string installPath)
        {
            var contentFileName = Path.Combine(installPath, "content.xml");
            if (!File.Exists(contentFileName))
            {
                return;
            }

            var root = ReadXmlFile(contentFileName);
            if (root == null)
            {
                return;
            }

            ResetContent(root);

            InstallTemplates(root);

            InstallItems(root);

            PublishDatabases(root);
        }

        protected virtual void ProcessFiles([NotNull] string rootDirectory, [NotNull] string sourceDirectory)
        {
            foreach (var sourceFileName in Directory.GetFiles(sourceDirectory))
            {
                var destination = FileUtil.MapPath(sourceFileName.Mid(rootDirectory.Length + 1));

                if (string.Equals(Path.GetExtension(sourceFileName), ".dll", StringComparison.OrdinalIgnoreCase) && !CanCopyBinFile(sourceFileName, destination))
                {
                    continue;
                }

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? string.Empty);
                    File.Copy(sourceFileName, destination, true);
                }
                catch (Exception ex)
                {
                    TraceError("Could not copy", ex.Message + ": " + sourceDirectory + " => " + destination);
                }
            }

            foreach (var subdirectory in Directory.GetDirectories(sourceDirectory))
            {
                ProcessFiles(rootDirectory, subdirectory);
            }
        }

        protected virtual void ProcessFiles([NotNull] string path)
        {
            var sourceDirectory = path + "\\content";
            if (Directory.Exists(sourceDirectory))
            {
                ProcessFiles(sourceDirectory, sourceDirectory);
            }
        }

        protected virtual XElement ReadXmlFile([NotNull] string fileName)
        {
            XElement root;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    var doc = XDocument.Load(stream);

                    root = doc.Root;
                    if (root == null)
                    {
                        TraceError("Cannot read Xml file", fileName);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    TraceError("Cannot load Xml file", fileName + ": " + ex.Message);
                    return null;
                }
            }

            return root;
        }

        protected virtual void ResetContent([NotNull] XElement root)
        {
            var resetElement = root.Element("reset");
            if (resetElement == null)
            {
                return;
            }

            foreach (var itemElement in resetElement.Elements())
            {
                var databaseName = itemElement.GetAttributeValue("database");
                var itemId = itemElement.GetAttributeValue("id");

                var database = Factory.GetDatabase(databaseName);

                var item = database.GetItem(itemId);
                if (item != null)
                {
                    item.Delete();
                }
            }
        }

        protected virtual void TraceError(string text, string details = "")
        {
            var s = text + (string.IsNullOrEmpty(details) ? string.Empty : ": " + details);

            Log.Error(s, GetType());

            File.AppendAllText(_fileName, @"Error: " + s);
        }

        protected virtual void TraceInformation(string text, string details = "")
        {
            var s = text + (string.IsNullOrEmpty(details) ? string.Empty : ": " + details);

            File.AppendAllText(_fileName, s);
        }

        private void PublishDatabases(XElement root)
        {
            var publishElement = root.Element("publish");
            if (publishElement == null)
            {
                return;
            }

            foreach (var attribute in publishElement.Attributes())
            {
                var databaseName = attribute.Name.LocalName;
                var mode = attribute.Value;

                var database = Factory.GetDatabase(databaseName);

                var publishingTargets = PublishManager.GetPublishingTargets(database);

                var targetDatabases = publishingTargets.Select(target => Factory.GetDatabase(target["Target database"])).ToArray();
                if (!targetDatabases.Any())
                {
                    continue;
                }

                var languages = LanguageManager.GetLanguages(database).ToArray();

                switch (mode)
                {
                    case "republish":
                        PublishManager.Republish(database, targetDatabases, languages);
                        break;

                    case "incremental":
                        PublishManager.PublishIncremental(database, targetDatabases, languages);
                        break;

                    case "smart":
                        PublishManager.PublishSmart(database, targetDatabases, languages);
                        break;

                    case "rebuild":
                        PublishManager.RebuildDatabase(database, targetDatabases);
                        break;
                }
            }
        }
    }
}
