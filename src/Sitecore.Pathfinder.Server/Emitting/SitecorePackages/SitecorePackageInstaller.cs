// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Sitecore.Data.Engines;
using Sitecore.Data.Proxies;
using Sitecore.Install;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Metadata;
using Sitecore.Install.Utils;
using Sitecore.Install.Zip;
using Sitecore.IO;
using Sitecore.SecurityModel;

namespace Sitecore.Pathfinder.Emitting.SitecorePackages
{
    public class SitecorePackageInstaller
    {
        public void Install([NotNull] string fileName)
        {
            try
            {
                Context.SetActiveSite("shell");
                using (new SecurityDisabler())
                {
                    using (new ProxyDisabler())
                    {
                        using (new SyncOperationContext())
                        {
                            IProcessingContext context = new SimpleProcessingContext();

                            IItemInstallerEvents itemInstallerEvents = new DefaultItemInstallerEvents(new BehaviourOptions(InstallMode.Overwrite, MergeMode.Clear));
                            context.AddAspect(itemInstallerEvents);

                            IFileInstallerEvents fileInstallerEvents = new DefaultFileInstallerEvents(true);
                            context.AddAspect(fileInstallerEvents);

                            var installer = new Installer();

                            installer.InstallPackage(FileUtil.MapPath(fileName), context);

                            ISource<PackageEntry> source = new PackageReader(FileUtil.MapPath(fileName));
                            var previewContext = Installer.CreatePreviewContext();
                            var view = new MetadataView(previewContext);
                            var sink = new MetadataSink(view);
                            sink.Initialize(previewContext);
                            source.Populate(sink);
                            installer.ExecutePostStep(view.PostStep, previewContext);
                        }
                    }
                }

                File.WriteAllText(fileName + ".log", @"OK");
            }
            catch (Exception ex)
            {
                File.WriteAllText(fileName + ".log", @"Error" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
