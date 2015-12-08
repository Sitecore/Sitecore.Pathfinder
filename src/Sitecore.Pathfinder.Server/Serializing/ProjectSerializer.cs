// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Importing.ItemImporters;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Serializing
{
    public class ProjectSerializer
    {
        public ProjectSerializer([Diagnostics.NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        [Diagnostics.NotNull]
        public string ProjectDirectory { get; private set; }

        [Diagnostics.NotNull]
        public string ToolsDirectory { get; private set; }

        [Diagnostics.NotNull]
        protected ICompositionService CompositionService { get; }

        [Diagnostics.NotNull, ItemNotNull]
        protected IEnumerable<string> ExcludedFields { get; private set; }

        [Diagnostics.NotNull]
        protected PathMapperService PathMapper { get; private set; }

        [Diagnostics.NotNull]
        protected IProject Project { get; private set; }

        public virtual void SerializeItem([Diagnostics.NotNull] IItemImporterService itemImporter, [Diagnostics.NotNull] ILanguageService languageService, [Diagnostics.NotNull] Item item)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectFileNames)
            {
                string projectFileName;
                string format;
                if (!mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
                {
                    continue;
                }

                var language = languageService.GetLanguageByExtension(format);
                if (language == null)
                {
                    throw new ConfigurationException(Texts.Format_not_found, format);
                }

                var i = itemImporter.ImportItem(Project, item, language, ExcludedFields);

                var fileName = Path.Combine(ProjectDirectory, projectFileName);
                using (var stream = new StreamWriter(fileName))
                {
                    language.WriteItem(stream, i);
                }

                // write media file
                if (!string.IsNullOrEmpty(item["Blob"]))
                {
                    var mediaItem = new MediaItem(item);
                    var mediaFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(fileName) + "." + mediaItem.Extension;

                    using (var stream = new FileStream(mediaFileName, FileMode.Create))
                    {
                        FileUtil.CopyStream(mediaItem.GetMediaStream(), stream);
                    }
                }
            }
        }

        [Diagnostics.NotNull]
        public ProjectSerializer With([Diagnostics.NotNull] string toolsDirectory, [Diagnostics.NotNull] string projectDirectory)
        {
            ToolsDirectory = toolsDirectory;
            ProjectDirectory = projectDirectory;

            var configuration = new Startup().WithToolsDirectory(ToolsDirectory).WithProjectDirectory(ProjectDirectory).AsNoninteractive().RegisterConfiguration();
            if (configuration == null)
            {
                throw new ConfigurationException("Failed to load configuration");
            }

            PathMapper = new PathMapperService(configuration);
            ExcludedFields = configuration.GetCommaSeparatedStringList(Constants.Configuration.ProjectWebsiteMappingsExcludedFields);
            Project = CompositionService.Resolve<IProject>();

            return this;
        }
    }
}
