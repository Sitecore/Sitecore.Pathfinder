// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class LayoutCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> PlaceholdersShouldHaveAPlaceholderSettingsName([NotNull] ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                from placeholder in rendering.Placeholders
                where context.Project.Items.FirstOrDefault(i => i.ItemName == placeholder && i.TemplateName == "Placeholder") == null
                select Warning(Msg.C1105, "Placeholders should have a Placeholder Settings item", new StringTextNode("Placeholder(\""+ placeholder + "\")", rendering.Snapshot), $"To fix, create a '/sitecore/layout/Placeholder Settings/{placeholder}' item");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> RenderingNameAndFileNameShouldMatch([NotNull] ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                where rendering.ItemName != PathHelper.GetFileNameWithoutExtensions(rendering.FilePath)
                select Warning(Msg.C1106, "Rendering item name should match file name", rendering.Snapshot.SourceFile, "To fix, rename the rendering file or rename the rendering item");
        }
    }
}
