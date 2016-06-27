// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ListOutput : BuildTaskBase
    {
        [ImportingConstructor]
        public ListOutput() : base("list-output")
        {
        }

        public override void Run(IBuildContext context)
        {
            foreach (var item in context.Project.Items.Where(i => !i.IsImport).OrderBy(i => i.ItemIdOrPath))
            {
                var text = item.ItemIdOrPath;
                if (string.IsNullOrEmpty(text))
                {
                    text = item.ItemName;
                }

                text += " [" + item.TemplateName + "]";

                if (string.IsNullOrEmpty(text))
                {
                    text = "? [Item]";
                }

                Console.WriteLine(text);
            }

            foreach (var filePath in context.Project.Files.OrderBy(i => i.FilePath))
            {
                var fileName = filePath.FilePath;
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                Console.WriteLine(fileName);
            }
        }
    }
}
