// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Projects
{
    [Export(typeof(IProjectService))]
    public class ProjectService : IProjectService
    {
        [ImportingConstructor]
        public ProjectService([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] ICheckerService checker)
        {
            CompositionService = compositionService;
            Configuration = configuration;
            Factory = factory;
            Checker = checker;
        }

        [NotNull]
        protected ICheckerService Checker { get; set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        public IProject LoadProjectFromConfiguration()
        {
            var projectOptions = CreateProjectOptions();

            LoadExternalReferences(projectOptions);
            LoadStandardTemplateFields(projectOptions);

            var sourceFileNames = new List<string>();
            LoadSourceFileNames(projectOptions, sourceFileNames);

            var project = Factory.Project(projectOptions, sourceFileNames);

            return project;
        }

        [NotNull]
        protected virtual ProjectOptions CreateProjectOptions()
        {
            var projectDirectory = PathHelper.Combine(Configuration.GetString(Constants.Configuration.SolutionDirectory), Configuration.GetString(Constants.Configuration.ProjectDirectory));
            var databaseName = Configuration.GetString(Constants.Configuration.Database);

            return Factory.ProjectOptions(projectDirectory, databaseName);
        }

        protected virtual void LoadExternalReferences([NotNull] ProjectOptions projectOptions)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.ExternalReferences))
            {
                Guid guid;
                if (!Guid.TryParse(pair.Key, out guid))
                {
                    throw new InvalidOperationException($"External reference Guid is not valid: {pair.Key}");
                }

                var value = Configuration.Get(Constants.Configuration.ExternalReferences + ":" + pair.Key);
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException($"External reference path is not valid: {pair.Key}");
                }

                projectOptions.ExternalReferences.Add(new Tuple<Guid, string>(guid, value));
            }
        }

        protected virtual void LoadStandardTemplateFields([NotNull] ProjectOptions projectOptions)
        {
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.StandardTemplateFields))
            {
                projectOptions.StandardTemplateFields.Add(pair.Key);

                var value = Configuration.Get(Constants.Configuration.StandardTemplateFields + ":" + pair.Key);
                if (!string.IsNullOrEmpty(value))
                {
                    projectOptions.StandardTemplateFields.Add(value);
                }
            }
        }

        protected virtual void LoadSourceFileNames([NotNull] ProjectOptions projectOptions, [NotNull] ICollection<string> sourceFileNames)
        {
            var ignoreFileNames = Configuration.GetList(Constants.Configuration.IgnoreFileNames).ToList();
            var ignoreDirectories = Configuration.GetList(Constants.Configuration.IgnoreDirectories).ToList();
            ignoreDirectories.Add(Path.GetFileName(Configuration.GetString(Constants.Configuration.ToolsDirectory)));

            var visitor = CompositionService.Resolve<ProjectDirectoryVisitor>().With(ignoreDirectories, ignoreFileNames);
            visitor.Visit(projectOptions, sourceFileNames);
        }
    }
}
