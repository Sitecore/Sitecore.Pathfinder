// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Compiling.FieldCompilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Markdown.Compiling.FieldCompilers
{
    public class MarkdownFieldCompiler : FieldCompilerBase
    {
        public MarkdownFieldCompiler() : base(Pathfinder.Constants.FieldCompilers.High)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                return false;
            }

            var fieldTypeCheck = string.Equals(field.TemplateField.Type, "rich text", StringComparison.OrdinalIgnoreCase);

            var indicator = GetIndicatorToken(context.Configuration);
            var indicatorCheck = field.Value.StartsWith(indicator);

            return fieldTypeCheck && indicatorCheck;
        }

        public override string Compile(IFieldCompileContext context, Field field)
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
            return config.GetString(Constants.Configuration.IndicatorToken, "!md\r\n");
        }
    }
}
