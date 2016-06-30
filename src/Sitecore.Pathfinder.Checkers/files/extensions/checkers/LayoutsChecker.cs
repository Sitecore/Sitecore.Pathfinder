// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checkers
{
    public class LayoutsChecker : Checker
    {
        [Export("Check")]
        public IEnumerable<Diagnostic> PlaceholdersShouldHaveAPlaceholderSettingsName(ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                from placeholder in rendering.Placeholders
                where context.Project.Items.FirstOrDefault(i => i.ItemName == placeholder && i.TemplateName == "Placeholder") == null
                select Warning(Msg.C1000, "Placeholders should have a Placeholder Settings item", new StringTextNode("Placeholder(\""+ placeholder + "\")", rendering.Snapshots.First()), $"To fix, create a '/sitecore/layout/Placeholder Settings/{placeholder}' item");
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> RenderingNameAndFileNameShouldMatch(ICheckerContext context)
        {
            return from rendering in context.Project.ProjectItems.OfType<Rendering>()
                where rendering.ItemName != PathHelper.GetFileNameWithoutExtensions(rendering.FilePath)
                select Warning(Msg.C1000, "Rendering item name should match file name", rendering.Snapshots.First().SourceFile, $"To fix, rename the rendering file or rename the rendering item");
        }
    }
}
