using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    public class ConfigFileEmitter : EmitterBase
    {
        public ConfigFileEmitter() : base(Constants.Emitters.ContentFiles)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is ConfigFile;
        }


        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var configFile = (ConfigFile)projectItem;

            var filePath = PathHelper.NormalizeFilePath(configFile.FilePath);
            if (filePath.StartsWith("~\\"))
            {
                filePath = filePath.Mid(2);
            }

            var destinationFileName = PathHelper.Combine(context.Configuration.GetWebsiteDirectory(), filePath);

            context.Trace.TraceInformation(Msg.I1011, "Installing config file", filePath);

            context.FileSystem.CreateDirectoryFromFileName(destinationFileName);
            context.FileSystem.Copy(projectItem.Snapshot.SourceFile.AbsoluteFileName, destinationFileName, context.ForceUpdate);
        }
    }
}