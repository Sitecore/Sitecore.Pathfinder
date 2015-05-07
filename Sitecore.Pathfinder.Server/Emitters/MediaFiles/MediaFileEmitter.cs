namespace Sitecore.Pathfinder.Emitters.MediaFiles
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.MediaFiles;
  using Sitecore.Resources.Media;

  [Export(typeof(IEmitter))]
  public class MediaFileEmitter : EmitterBase
  {
    public MediaFileEmitter() : base(MediaFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectElementBase model)
    {
      return model is MediaFile;
    }

    public override void Emit(IEmitContext context, ProjectElementBase model)
    {
      var mediaFileModel = (MediaFile)model;

      var database = Factory.GetDatabase(mediaFileModel.DatabaseName);
      var name = Path.GetFileNameWithoutExtension(model.SourceFileName);

      var options = new MediaCreatorOptions
      {
        AlternateText = name, 
        Database = database, 
        FileBased = false, 
        IncludeExtensionInItemName = false, 
        KeepExisting = false, 
        Language = LanguageManager.DefaultLanguage, 
        Versioned = false, 
        Destination = mediaFileModel.ItemIdOrPath
      };

      // create parent path of media folders before uploading
      var parentPath = (Path.GetDirectoryName(mediaFileModel.ItemIdOrPath) ?? string.Empty).Replace("\\", "/");
      var mediaFolderTemplate = new TemplateItem(database.GetItem(TemplateIDs.MediaFolder));
      database.CreateItemPath(parentPath, mediaFolderTemplate);

      using (var stream = new FileStream(model.SourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(model.SourceFileName), options);
        if (item == null)
        {
          throw new BuildException(Texts.Text2013, model.SourceFileName);
        }
      }
    }
  }
}
