// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Checking
{
    public abstract class CheckerBase : IChecker
    {
        protected const string Fields = " Fields";

        protected const string Items = " Items";

        protected const string Media = " Media";

        protected const string TemplateFields = " TemplateFields";

        protected const string Templates = " Templates";

        protected CheckerBase([NotNull] string name, [NotNull] string tags)
        {
            Name = name;
            Tags = tags;
        }

        public string Name { get; }

        public string Tags { get; }

        public abstract void Check(ICheckerContext context);
    }
}
