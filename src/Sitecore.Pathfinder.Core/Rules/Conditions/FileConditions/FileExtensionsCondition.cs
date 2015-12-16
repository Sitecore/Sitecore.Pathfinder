using System.Collections.Generic;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.FileConditions
{
    public class FileExtensionsCondition : StringConditionBase
    {
        public FileExtensionsCondition() : base("file-name-extensions")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var file = ruleContext.Object as Projects.Files.File;
            return file == null ? string.Empty : PathHelper.GetExtension(file.FilePath);
        }
    }
}