// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    [Export(typeof(IFieldCompiler)), Shared]
    public class ImageFieldCompiler : FieldCompilerBase
    {
        public ImageFieldCompiler() : base(Constants.FieldCompilers.Normal)
        {
        }

        public override bool IsExclusive { get; } = true;

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            return string.Equals(field.TemplateField.Type, "image", StringComparison.OrdinalIgnoreCase);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var qualifiedName = field.Value.Trim();
            if (string.IsNullOrEmpty(qualifiedName))
            {
                return string.Empty;
            }

            if (qualifiedName == "<image />")
            {
                return string.Empty;
            }

            var item = field.Item.Project.Indexes.FindQualifiedItem<IProjectItem>(qualifiedName);

            if (item == null)
            {
                var mediaFile = field.Item.Project.Files.FirstOrDefault(f => string.Equals(f.FilePath, qualifiedName, StringComparison.OrdinalIgnoreCase)) as MediaFile;
                if (mediaFile != null)
                {
                    item = field.Item.Project.Indexes.FindQualifiedItem<Item>(mediaFile.MediaItemUri);
                }
            }

            if (item == null)
            {
                context.Trace.TraceError(Msg.C1044, Texts.Image_reference_not_found, TraceHelper.GetTextNode(field.ValueProperty, field.FieldNameProperty), qualifiedName);
                return string.Empty;
            }

            return $"<image mediapath=\"\" alt=\"\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{item.Uri.Guid.Format()}\" />";
        }
    }
}
