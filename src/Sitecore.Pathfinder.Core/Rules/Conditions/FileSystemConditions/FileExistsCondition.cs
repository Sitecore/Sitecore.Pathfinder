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
    public class FileExistsCondition : ConditionBase
    {
        [ImportingConstructor]
        public FileExistsCondition([NotNull] IConfiguration configuration, [NotNull] IFileSystemService fileSystem) : base("file-exists")
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
            var fileName = GetParameterValue(parameters, "file-name", ruleContext.Object);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GetParameterValue(parameters, "value", ruleContext.Object);
            }

            Assert.IsNotNullOrEmpty(fileName);

            if (!fileName.StartsWith("~/"))
            {
                throw new ArgumentException("File name must start with '~/'");
            }

            var projectDirectory = Configuration.GetProjectDirectory();
            fileName = Path.Combine(projectDirectory, PathHelper.NormalizeFilePath(fileName.Mid(2)));

            return FileSystem.FileExists(fileName);
        }
    }
}
