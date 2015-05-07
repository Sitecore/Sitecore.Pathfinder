namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.MediaItemFileBuilders
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Resources.Media;

  [Export(typeof(IItemFileBuilder))]
  public class MediaItemFileBuilder : ItemFileBuilderBase
  {
    private static readonly string[] FileExtensions = 
    {
      ".png", 
      ".gif", 
      ".bmp", 
      ".jpg", 
      ".jpeg", 
      ".docx", 
      ".doc", 
      ".pdf", 
      ".zip", 
    };

    public MediaItemFileBuilder() : base(Media)
    {
    }

    public override void Build(IItemFileBuildContext context)
    {
      var database = Factory.GetDatabase(context.DatabaseName);
      var name = Path.GetFileNameWithoutExtension(context.FileName);

      var options = new MediaCreatorOptions
      {
        AlternateText = name, 
        Database = database, 
        FileBased = false, 
        IncludeExtensionInItemName = false, 
        KeepExisting = false, 
        Language = LanguageManager.DefaultLanguage, 
        Versioned = false, 
        Destination = context.ItemPath
      };

      // create parent path of media folders before uploading
      var parentPath = (Path.GetDirectoryName(context.ItemPath) ?? string.Empty).Replace("\\", "/");
      var mediaFolderTemplate = new TemplateItem(database.GetItem(TemplateIDs.MediaFolder));
      database.CreateItemPath(parentPath, mediaFolderTemplate);

      using (var stream = new FileStream(context.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(context.FileName), options);
        if (item == null)
        {
          throw new BuildException(Texts.Text2013, context.FileName);
        }
      }
    }

    public override bool CanBuild(IItemFileBuildContext context)
    {
      var fileExtension = Path.GetExtension(context.FileName);
      return FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }
  }
}
