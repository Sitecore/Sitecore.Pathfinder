using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions.FileConditions
{
    public class AbsoluteFileNameCondition : StringConditionBase
    {
        public AbsoluteFileNameCondition() : base("absolute-file-name")
        {
        }

        protected override string GetValue(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var file = ruleContext.Object as Projects.Files.File;
            return file == null ? string.Empty : Path.GetFileName(file.Snapshots.First().SourceFile.AbsoluteFileName);
        }
    }
}