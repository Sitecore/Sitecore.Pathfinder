// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NuGet;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Pathfinder.Install.Emitting;
using Sitecore.Pathfinder.Install.Parsing;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Install
{
    public class NugetPackageInstaller
    {
        public void Install([NotNull] string fileName)
        {
            try
            {
                string packageId;
                string version;
                ExtractPackageId(fileName, out packageId, out version);
                if (string.IsNullOrEmpty(packageId))
                {
                    File.WriteAllText(fileName + ".log", @"Error" + Environment.NewLine + "Failed to extract package ID");
                    return;
                }

                var directory = FileUtil.MapDataFilePath("pathfinder") + "\\.nuget";
                var localRepository = GetLocalRepository(directory);
                var remoteRepository = GetRemoteRepository(new[]
                {
                    Path.GetDirectoryName(fileName),
                    "https://api.nuget.org/v3/index.json"
                });

                var package = remoteRepository.FindPackage(packageId);
                if (package == null)
                {
                    return;
                }

                var packageManager = new PackageManager(remoteRepository, localRepository.Source);
                packageManager.PackageInstalled += InstallPackage;

                packageManager.InstallPackage(package, false, true);

                File.WriteAllText(fileName + ".log", @"OK");
            }
            catch (Exception ex)
            {
                File.WriteAllText(fileName + ".log", @"Error" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
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

        protected virtual void InstallPackage(object sender, PackageOperationEventArgs e)
        {
            var contentFileName = Path.Combine(e.InstallPath, "content.xml");
            if (!File.Exists(contentFileName))
            {
                return;
            }

            InstallContent(contentFileName);
        }

        private void InstallContent([NotNull] string contentFileName)
        {
            XElement root;
            using (var stream = new FileStream(contentFileName, FileMode.Open))
            {
                var doc = XDocument.Load(stream);
                root = doc.Root;
                if (root == null)
                {
                    Log.Error("Cannot read Xml file", GetType());
                    return;
                }
            }

            var templatesElement = root.Element("templates");
            if (templatesElement != null)
            {
                foreach (var templateElement in templatesElement.Elements())
                {
                    InstallTemplate(templateElement);
                }
            }
        }

        private void InstallTemplate(XElement templateElement)
        {
            var template = Template.Parse(templateElement);

            var writer = new TemplateWriter(template);
            writer.Write();
        }

        private void ExtractPackageId([NotNull] string fileName, out string packageId, out string version)
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
    }
}
