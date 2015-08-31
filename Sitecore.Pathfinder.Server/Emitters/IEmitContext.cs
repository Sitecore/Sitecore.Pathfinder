// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Data;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Emitters
{
    public interface IEmitContext
    {
        [Diagnostics.NotNull]
        ICollection<string> AddedFiles { get; }

        [Diagnostics.NotNull]
        ICollection<string> AddedItems { get; }

        [Diagnostics.NotNull]
        IConfiguration Configuration { get; }

        [Diagnostics.NotNull]
        IDataService DataService { get; }

        [Diagnostics.NotNull]
        ICollection<string> DeletedFiles { get; }

        [Diagnostics.NotNull]
        ICollection<string> DeletedItems { get; }

        [Diagnostics.NotNull]
        IFileSystemService FileSystem { get; }

        [Diagnostics.NotNull]
        IProject Project { get; }

        [Diagnostics.NotNull]
        ITraceService Trace { get; }

        [Diagnostics.NotNull]
        string UninstallDirectory { get; }

        [Diagnostics.NotNull]
        ICollection<string> UpdatedFiles { get; }

        [Diagnostics.NotNull]
        ICollection<string> UpdatedItems { get; }

        void RegisterAddedFile([Diagnostics.NotNull] File projectItem, [Diagnostics.NotNull] string destinationFileName);

        void RegisterAddedItem([Diagnostics.NotNull] Item newItem);

        void RegisterDeletedFile([Diagnostics.NotNull] File projectItem, [Diagnostics.NotNull] string destinationFileName);

        void RegisterDeletedItem([Diagnostics.NotNull] Item deletedItem);

        void RegisterUpdatedFile([Diagnostics.NotNull] File projectItem, [Diagnostics.NotNull] string destinationFileName);

        void RegisterUpdatedItem([Diagnostics.NotNull] Item item);

        [Diagnostics.NotNull]
        IEmitContext With([Diagnostics.NotNull] IProject project);
    }
}
