// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Serialization;

namespace Sitecore.Pathfinder.Building.Assembling
{
    public class WriteSerialization : BuildTaskBase
    {
        public WriteSerialization() : base("write-serialization")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                return;
            }

            context.Trace.TraceInformation(Msg.D1021, Texts.Writing_serialization___);

            var directory = PathHelper.Combine(context.ProjectDirectory, context.Configuration.GetString(Constants.Configuration.WriteSerializationDirectory));
            if (context.FileSystem.DirectoryExists(directory))
            {
                context.FileSystem.DeleteDirectory(directory);
            }

            var flat = context.Configuration.GetBool(Constants.Configuration.WriteSerializationFlat);

            foreach (var item in context.Project.Items)
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

                fileName = context.FileSystem.GetUniqueFileName(fileName);

                context.FileSystem.CreateDirectoryFromFileName(fileName);

                using (var stream = new StreamWriter(fileName))
                {
                    item.WriteAsSerialization(stream, WriteAsSerializationOptions.WriteCompiledFieldValues);
                }
            }
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Writes all items to a serialization folder");

            helpWriter.Remarks.Write("Writes all items to a serialization folder.");
        }
    }
}
