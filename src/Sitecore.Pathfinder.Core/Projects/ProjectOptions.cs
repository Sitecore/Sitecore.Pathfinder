// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectOptions
    {
        [NotNull]
        public static readonly ProjectOptions Empty = new ProjectOptions(string.Empty, string.Empty);

        public ProjectOptions([NotNull] string projectDirectory, [NotNull] string databaseName)
        {
            ProjectDirectory = projectDirectory;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [NotNull, ItemNotNull]
        public ICollection<string> StandardTemplateFields { get; } = new List<string>();

        [NotNull]
        public IDictionary<string, string> Tokens { get; } = new Dictionary<string, string>();

        public virtual void LoadStandardTemplateFields([NotNull] IConfiguration configuration)
        {
            foreach (var pair in configuration.GetSubKeys(Constants.Configuration.StandardTemplateFields))
            {
                StandardTemplateFields.Add(pair.Key);

                var value = configuration.GetString(Constants.Configuration.StandardTemplateFields + ":" + pair.Key);
                if (!string.IsNullOrEmpty(value))
                {
                    StandardTemplateFields.Add(value);
                }
            }
        }

        public virtual void LoadTokens([NotNull] IConfiguration configuration)
        {
            foreach (var pair in configuration.GetSubKeys(Constants.Configuration.SearchAndReplaceTokens))
            {
                var value = configuration.GetString(Constants.Configuration.SearchAndReplaceTokens + ":" + pair.Key);
                Tokens[pair.Key] = value;
            }
        }
    }
}
