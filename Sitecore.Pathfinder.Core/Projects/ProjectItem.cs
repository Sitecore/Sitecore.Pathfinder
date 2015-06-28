// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.References;

namespace Sitecore.Pathfinder.Projects
{
    [DebuggerDisplay("{GetType().Name,nq}: {QualifiedName}")]
    public abstract class ProjectItem : IProjectItem
    {
        private static readonly MD5 Md5Hash = MD5.Create();

        protected ProjectItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot)
        {
            Project = project;
            ProjectUniqueId = projectUniqueId;
            Snapshots.Add(snapshot);

            References = new ReferenceCollection(this);

            SetGuid();
        }

        public Guid Guid { get; private set; }

        public IProject Project { get; }

        // item format: /sitecore/content/Home/Welcome
        // guid format: {25FCB32E-4D31-40C9-A409-4D936345F0DB}
        // file format: /sitecore/content/Home/Welcome.jpg (includes extensions)
        public string ProjectUniqueId { get; private set; }

        public abstract string QualifiedName { get; }

        public ReferenceCollection References { get; }

        public abstract string ShortName { get; }

        public ICollection<ISnapshot> Snapshots { get; } = new List<ISnapshot>();

        public abstract void Rename(string newShortName);

        protected virtual void Merge([NotNull] IParseContext context, [NotNull] IProjectItem newProjectItem, bool overwrite)
        {
            foreach (var snapshot in newProjectItem.Snapshots)
            {
                Snapshots.Add(snapshot);
            }

            if (!overwrite)
            {
                return;
            }

            ProjectUniqueId = newProjectItem.ProjectUniqueId;
            SetGuid();
        }

        private void SetGuid()
        {
            Guid guid;
            if (!Guid.TryParse(ProjectUniqueId, out guid))
            {
                // calculate guid from project unique id and project id
                var text = Project.ProjectUniqueId + "/" + ProjectUniqueId;
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = Md5Hash.ComputeHash(bytes);
                guid = new Guid(hash);
            }

            Guid = guid;
        }
    }
}
