// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public class PackageWriter
    {
        [ItemNotNull, NotNull]
        private readonly List<string> databaseFiles = new List<string>();

        [NotNull]
        private readonly string outputPath;

        [NotNull]
        private readonly ISqlGenerator sqlGenerator;

        [NotNull]
        private readonly IFileSystem fileSystemProvider;

        public PackageWriter([NotNull] IFileSystem fileSystemProvider, [NotNull] ISqlGenerator sqlGenerator, [NotNull] string outputPath)
        {
            this.fileSystemProvider = fileSystemProvider;
            this.sqlGenerator = sqlGenerator;
            this.outputPath = outputPath;
        }

        public void Dispose()
        {
            var appendStatements = sqlGenerator.GenerateAppendStatements();
            foreach (var databaseFile in databaseFiles)
            {
                fileSystemProvider.AppendFile(databaseFile, appendStatements);
            }
        }

        public void WriteBlob([NotNull] Blob blob)
        {
            var addBlobStatements = sqlGenerator.GenerateAddBlobStatements(blob);
            fileSystemProvider.AppendFile(Path.Combine(outputPath, $"{blob.Database}.sql"), addBlobStatements);
        }

        public void WriteFile([NotNull] PackageFile file)
        {
            fileSystemProvider.WriteFile(Path.Combine(outputPath, "website", file.FileName), file.Content);
        }

        public void WriteItem([NotNull] VersionedItem item)
        {
            var databaseFile = Path.Combine(outputPath, $"{item.Database.DatabaseName}.sql");
            if (!databaseFiles.Contains(databaseFile))
            {
                databaseFiles.Add(databaseFile);

                if (fileSystemProvider.FileExists(databaseFile))
                {
                    fileSystemProvider.DeleteFile(databaseFile);
                }

                var prependStatements = sqlGenerator.GeneratePrependStatements();
                fileSystemProvider.AppendFile(databaseFile, prependStatements);
            }

            var addItemStatements = sqlGenerator.GenerateAddItemStatements(item);
            fileSystemProvider.AppendFile(databaseFile, addItemStatements);
        }

        public void WritePostStep([NotNull] string packageName, [NotNull] string postStep)
        {
            fileSystemProvider.WriteFile(Path.Combine(outputPath, $"website\\App_Data\\poststeps\\{(packageName ?? "poststep").Replace(' ', '_')}.poststep"), postStep);
        }

        public void WriteRole([NotNull] Role role)
        {
            var addRoleStatements = sqlGenerator.GenerateAddRoleStatements(role);
            fileSystemProvider.AppendFile(Path.Combine(outputPath, "core.sql"), addRoleStatements);
        }
    }
}
