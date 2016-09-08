// © 2016 Sitecore Corporation A/S. All rights reserved.

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
        [CanBeNull]
        private string _indicator;

        public MarkdownFieldCompiler() : base(Pathfinder.Constants.FieldCompilers.High)
        {
        }

        public override bool CanCompile(IFieldCompileContext context, Field field)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                return false;
            }

            if (!string.Equals(field.TemplateField.Type, "rich text", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var indicator = GetIndicatorToken(context.Configuration);

            return field.Value.StartsWith(indicator);
        }

        public override string Compile(IFieldCompileContext context, Field field)
        {
            var indicator = GetIndicatorToken(context.Configuration);

            // trim indicator
            var wip = field.Value.Mid(indicator.Length);

            var transformer = new HeyRed.MarkdownSharp.Markdown();
            wip = transformer.Transform(wip);

            return wip;
        }

        [NotNull]
        protected string GetIndicatorToken([NotNull] IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(_indicator))
            {
                _indicator = configuration.GetString(Constants.Configuration.IndicatorToken, "!md\r\n");
            }

            return _indicator;
        }
    }
}
