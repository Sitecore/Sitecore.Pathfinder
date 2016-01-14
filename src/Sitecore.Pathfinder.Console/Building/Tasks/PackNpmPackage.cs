// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Building.Tasks
{
    public class PackNpmPackage : BuildTaskBase
    {
        [ImportingConstructor]
        public PackNpmPackage([NotNull] IPathMapperService pathMapper) : base("pack-npm")
        {
            PathMapper = pathMapper;

            CanRunWithoutConfig = true;
        }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Msg.D1017, Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Msg.D1018, "Creating Npm module...");

            var npmFileName = Path.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.PackNpmOutputFile));

            using (var fileStream = File.Create(npmFileName))
            {
                using (var zipStream = new GZipOutputStream(fileStream))
                {
                    using (var tarArchive = TarArchive.CreateOutputTarArchive(zipStream))
                    {
                        tarArchive.RootPath = context.ProjectDirectory;

                        foreach (var sourceFile in context.Project.SourceFiles)
                        {
                            var pathMappingContext = new PathMappingContext(PathMapper);
                            pathMappingContext.Parse(context.Project, sourceFile);

                            if (!pathMappingContext.IsMapped)
                            {
                                continue;
                            }

                            var tarEntry = TarEntry.CreateEntryFromFile(sourceFile.AbsoluteFileName);
                            tarArchive.WriteEntry(tarEntry, true);
                        }

                        tarArchive.Close();
                    }
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Creates an Npm module from the project.");
        }
    }
}
