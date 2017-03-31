// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NuGet;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.IO;
using Sitecore.Pathfinder.Emitting.Parsing;
using Sitecore.Pathfinder.Emitting.Writers;
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

        protected virtual void InstallContent([NotNull] string contentFileName)
        {
            var root = ReadXmlFile(contentFileName);
            if (root == null)
            {
                return;
            }

            var templatesElement = root.Element("templates");
            if (templatesElement != null)
            {
                foreach (var templateElement in templatesElement.Elements())
                {
                    InstallTemplate(templateElement);
                }
            }

            var itemsElement = root.Element("items");
            if (itemsElement != null)
            {
                foreach (var itemElement in itemsElement.Elements())
                {
                    InstallItem(itemElement);
                }
            }
        }

        protected virtual void InstallItem(XElement itemElement)
        {
            var item = Item.Parse(itemElement);

            new ItemWriter(item).Write();
        }

        protected virtual void InstallPackage(object sender, PackageOperationEventArgs e)
        {
            var resetFileName = Path.Combine(e.InstallPath, "reset.xml");
            if (File.Exists(resetFileName))
            {
                ResetContent(resetFileName);
            }

            var contentFileName = Path.Combine(e.InstallPath, "content.xml");
            if (File.Exists(contentFileName))
            {
                InstallContent(contentFileName);
            }
        }

        protected virtual void InstallTemplate(XElement templateElement)
        {
            var template = Template.Parse(templateElement);

            new TemplateWriter(template).Write();
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

        protected virtual void ResetContent([NotNull] string resetFileName)
        {
            var root = ReadXmlFile(resetFileName);
            if (root == null)
            {
                return;
            }

            foreach (var itemElement in root.Elements())
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
    }
}
