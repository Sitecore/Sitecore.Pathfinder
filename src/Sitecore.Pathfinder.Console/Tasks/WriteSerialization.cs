// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Serialization;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class WriteSerialization : BuildTaskBase
    {
        [NotNull]
        public IFileSystemService FileSystem { get; }

        [ImportingConstructor]
        public WriteSerialization([NotNull] IFileSystemService fileSystem) : base("write-serialization")
        {
            FileSystem = fileSystem;
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1021, Texts.Writing_serialization___);

            var directory = PathHelper.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.WriteSerialization.Directory));
            if (FileSystem.DirectoryExists(directory))
            {
                FileSystem.DeleteDirectory(directory);
            }

            var flat = context.Configuration.GetBool(Constants.Configuration.WriteSerialization.Flat);

            var project = context.LoadProject();

            foreach (var item in project.Items)
            {
                string fileName;
                if (flat)
                {
                    fileName = Path.Combine(directory, item.DatabaseName + "\\" + item.ItemName + ".item");
                }
                else
                {
                    fileName = Path.Combine(directory, item.DatabaseName + "\\" + PathHelper.NormalizeFilePath(item.ItemIdOrPath).TrimStart('\\') + ".item");
                }

                fileName = FileSystem.GetUniqueFileName(fileName);

                FileSystem.CreateDirectoryFromFileName(fileName);

                using (var stream = FileSystem.OpenStreamWriter(fileName))
                {
                    item.WriteAsSerialization(stream, WriteAsSerializationOptions.WriteCompiledFieldValues);
                }
            }
        }
    }
}
