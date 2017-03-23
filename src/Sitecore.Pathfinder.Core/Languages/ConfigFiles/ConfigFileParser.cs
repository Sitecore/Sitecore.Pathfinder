// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.ConfigFiles
{
    [Export(typeof(IParser)), Shared]
    public class ConfigFileParser : ParserBase
    {
        private const string AssemblyConfigFileExtension = ".dll.config";

        private const string ConfigFileExtension = ".config";

        private const string DisabledConfigFileExtension = ".config.disabled";

        private const string ExampleConfigFileExtension = ".config.example";

        public ConfigFileParser() : base(Constants.Parsers.Templates)
        {
        }

        public override bool CanParse(IParseContext context)
        {
            if (string.IsNullOrEmpty(context.FilePath))
            {
                return false;
            }

            var fileName = context.Snapshot.SourceFile.AbsoluteFileName;

            if (string.Equals(Path.GetFileName(fileName), "app.config", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (fileName.EndsWith(AssemblyConfigFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (fileName.EndsWith(ConfigFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (fileName.EndsWith(DisabledConfigFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return fileName.EndsWith(ExampleConfigFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public override void Parse(IParseContext context)
        {
            var configFile = context.Factory.ConfigFile(context.Project, context.Snapshot, context.FilePath);
            context.Project.AddOrMerge(configFile);
        }
    }
}
