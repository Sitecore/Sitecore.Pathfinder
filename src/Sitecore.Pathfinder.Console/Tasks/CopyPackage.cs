// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CopyPackage : BuildTaskBase
    {
        [ImportingConstructor]
        public CopyPackage([NotNull] IFileSystemService fileSystem) : base("copy-package")
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var dictionary = context.Configuration.GetDictionary(Constants.Configuration.CopyPackage);
            if (!dictionary.Any())
            {
                return;
            }

            context.Trace.TraceInformation(Msg.D1005, Texts.Copying_package___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            foreach (var pair in dictionary)
            {
                foreach (var fileName in context.OutputFiles)
                {
                    if (!PathHelper.MatchesPattern(fileName, pair.Key))
                    {
                        continue;
                    }

                    var destinationDirectory = PathHelper.Combine(context.ProjectDirectory, pair.Value);
                    var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(fileName));

                    FileSystem.CreateDirectoryFromFileName(destinationFileName);
                    FileSystem.Copy(fileName, destinationFileName);
                }
            }
        }
    }
}
