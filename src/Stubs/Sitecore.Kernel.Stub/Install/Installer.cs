// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Install.Framework;

namespace Sitecore.Install
{
    public class Installer
    {
        [NotNull]
        public static IProcessingContext CreatePreviewContext()
        {
            throw new NotImplementedException();
        }

        public void ExecutePostStep([NotNull] string postStep, [NotNull] IProcessingContext previewContext)
        {
            throw new NotImplementedException();
        }

        public void InstallPackage([NotNull] string fileName, [NotNull] SimpleProcessingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
