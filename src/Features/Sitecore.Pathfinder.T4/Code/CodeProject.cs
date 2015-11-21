// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeProject
    {
        [NotNull]
        private readonly Dictionary<string, CodeDatabase> _databases = new Dictionary<string, CodeDatabase>();

        public CodeProject([NotNull] IProject innerProject)
        {
            InnerProject = innerProject;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<File> Files => InnerProject.ProjectItems.OfType<File>();

        [NotNull]
        public IProject InnerProject { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CodeItem> Items => InnerProject.ProjectItems.OfType<Item>().Where(i => !i.IsImport).Select(i => new CodeItem(this, i));

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CodeTemplate> Templates => InnerProject.ProjectItems.OfType<Template>().Where(t => !t.IsImport).Select(t => new CodeTemplate(this, t));

        [NotNull]
        public CodeDatabase GetDatabase([NotNull] string databaseName)
        {
            var key = databaseName.ToLowerInvariant();

            CodeDatabase database;
            if (!_databases.TryGetValue(key, out database))
            {
                database = new CodeDatabase(this, databaseName);
                _databases[key] = database;
            }

            return database;
        }
    }
}
