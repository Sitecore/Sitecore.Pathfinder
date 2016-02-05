using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using System;

namespace Sitecore.Pathfinder.Markdown.Compiling.FieldCompilers
{
    public class MarkdownFieldCompiler : FieldCompilerBase
    {
        public MarkdownFieldCompiler() : base(Pathfinder.Constants.FieldCompilers.High)
        {
        }

        public override bool CanCompile([NotNull] IFieldCompileContext context, [NotNull] Field field)
        {
            if(string.IsNullOrEmpty(field.Value))
            {
                return false;
            }

            var fieldTypeCheck = string.Equals(field.TemplateField.Type, "rich text", StringComparison.OrdinalIgnoreCase);

            var indicator = GetIndicatorToken(context.Configuration);
            var indicatorCheck = indicator == null || field.Value.StartsWith(indicator);

            return fieldTypeCheck && indicatorCheck;
        }

        [NotNull]
        public override string Compile([NotNull] IFieldCompileContext context, [NotNull] Field field)
        {
            var indicator = GetIndicatorToken(context.Configuration);

            // Trim indicator
            var wip = field.Value.Substring(indicator.Length);

            var transformer = new MarkdownSharp.Markdown();
            wip = transformer.Transform(wip);

            return wip;
        }

        [NotNull]
        protected string GetIndicatorToken([NotNull] IConfiguration config)
        {
            return config.GetString(Constants.Configuration.IndicatorToken);
        }
    }
}
