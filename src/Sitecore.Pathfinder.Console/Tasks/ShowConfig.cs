// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ShowConfig : BuildTaskBase
    {
        [ImportingConstructor]
        public ShowConfig([NotNull] IConsoleService console) : base("show-config")
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public override void Run(IBuildContext context)
        {
            context.DisplayDoneMessage = false;

            var json = context.Configuration.ToJson();

            Console.WriteLine(json);
        }
    }
}
