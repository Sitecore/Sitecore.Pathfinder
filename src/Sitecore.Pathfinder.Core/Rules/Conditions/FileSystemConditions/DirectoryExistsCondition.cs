// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.FileSystemConditions
{
    public class DirectoryExistsCondition : ConditionBase
    {
        [ImportingConstructor]
        public DirectoryExistsCondition([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem) : base("directory-exists")
        {
            Configuration = configuration;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var directory = GetParameterValue(parameters, "directory", ruleContext.Object);
            if (string.IsNullOrEmpty(directory))
            {
                directory = GetParameterValue(parameters, "value", ruleContext.Object);
            }

            Assert.IsNotNullOrEmpty(directory);

            if (!directory.StartsWith("~/"))
            {
                throw new ArgumentException("Directory must start with '~/'");
            }

            var projectDirectory = Configuration.GetString(Constants.Configuration.ProjectDirectory);
            directory = Path.Combine(projectDirectory, PathHelper.NormalizeFilePath(directory.Mid(2)));

            return FileSystem.DirectoryExists(directory);
        }
    }
}
