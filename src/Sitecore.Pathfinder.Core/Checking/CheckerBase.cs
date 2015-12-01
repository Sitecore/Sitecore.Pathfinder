// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Checking
{
    public abstract class CheckerBase : IChecker
    {
        protected const string Fields = "Fields,";

        protected const string Items = "Items,";

        protected const string Media = "Media,";

        protected const string TemplateFields = "TemplateFields,";

        protected const string Templates = "Templates,";

        protected const string All = Fields + Items + Media + TemplateFields + Templates;

        protected CheckerBase([NotNull] string name, [NotNull] string categories)
        {
            Name = name;
            Categories = categories;
        }

        public string Name { get; }

        public string Categories { get; }

        public abstract void Check(ICheckerContext context);
    }
}
