// © 2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
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
            var json = context.Configuration.ToJson();

            Console.WriteLine(json);
        }
    }
}
