using System.IO;
using System.Linq;
using Sitecore.IO;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Languages.ConfigFiles;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters.Files
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

            var destinationFileName = FileUtil.MapPath(configFile.FilePath);

            context.FileSystem.CreateDirectoryFromFileName(destinationFileName);
            context.FileSystem.Copy(projectItem.Snapshots.First().SourceFile.AbsoluteFileName, destinationFileName, context.ForceUpdate);
        }
    }
}