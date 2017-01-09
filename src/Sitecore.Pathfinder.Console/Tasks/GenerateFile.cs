// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class GenerateFile : BuildTaskBase, IOptionPicker
    {
        [ImportingConstructor]
        public GenerateFile([NotNull] IPathMapperService pathMapper) : base("generate-file")
        {
            PathMapper = pathMapper;

            Alias = "generate";
            Shortcut = "g";
        }

        [NotNull, Option("generator", Alias = "g", PositionalArg = 1, HasOptions = true, PromptText = "Pick generator")]
        public string GeneratorDirectory { get; set; } = string.Empty;

        [NotNull, Option("name", Alias = "n", PositionalArg = 2, DefaultValue = "file")]
        public string Name { get; set; } = string.Empty;

        [NotNull]
        protected IPathMapperService PathMapper { get; }


        public override void Run(IBuildContext context)
        {
            var generatorsDirectory = Path.Combine(context.Configuration.GetToolsDirectory(), "files\\generators");

            context.Trace.TraceInformation(Msg.G1009, Texts.Generating_files___);

            if (!context.FileSystem.DirectoryExists(GeneratorDirectory))
            {
                if (GeneratorDirectory.IndexOf('/') < 0 && GeneratorDirectory.IndexOf('\\') < 0)
                {
                    GeneratorDirectory = Path.Combine(generatorsDirectory, GeneratorDirectory);
                }
            }

            if (!context.FileSystem.DirectoryExists(GeneratorDirectory))
            {
                var dir = string.Empty;
                foreach (var directory in context.FileSystem.GetDirectories(generatorsDirectory).OrderBy(d => d))
                {
                    if (directory.StartsWith(GeneratorDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(dir))
                        {
                            context.Trace.TraceError(Msg.G1019, "Ambigious generator");
                            return;
                        }

                        dir = directory;
                    }
                }

                GeneratorDirectory = dir;
            }

            if (!context.FileSystem.DirectoryExists(GeneratorDirectory))
            {
                context.Trace.TraceError(Msg.G1018, Texts.Generator_not_found, GeneratorDirectory);
                return;
            }

            var macros = new Dictionary<string, string>();
            macros["name"] = Name;

            var textFileExtensions = context.Configuration.GetStringList(Constants.Configuration.GenerateFile.TextFileExtensions);

            Copy(context, GeneratorDirectory, Directory.GetCurrentDirectory(), macros, textFileExtensions);
        }

        protected virtual void Copy([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory, [NotNull] Dictionary<string, string> macros, [ItemNotNull, NotNull] IEnumerable<string> textFileExtensions)
        {
            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));

                destinationFileName = ReplaceMacros(destinationFileName, macros, "__", "__");

                var extension = Path.GetExtension(sourceFileName);
                if (textFileExtensions.Any(t => string.Equals(t, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    var projectFileName = PathHelper.UnmapPath(context.Configuration.GetProjectDirectory(), destinationFileName);

                    string itemPath;
                    string databaseName;
                    bool isImport;
                    bool uploadMedia;
                    PathMapper.TryGetWebsiteItemPath(projectFileName, out databaseName, out itemPath, out isImport, out uploadMedia);
                    macros["itemPath"] = itemPath;
                    macros["database"] = databaseName;

                    string websiteFileName;
                    PathMapper.TryGetWebsiteFileName(projectFileName, out websiteFileName);
                    macros["fileName"] = websiteFileName;

                    var content = context.FileSystem.ReadAllText(sourceFileName);

                    content = ReplaceMacros(content, macros, "<%= ", " %>");

                    context.FileSystem.CreateDirectoryFromFileName(destinationFileName);
                    context.FileSystem.WriteAllText(destinationFileName, content);
                }
                else
                {
                    context.FileSystem.CreateDirectoryFromFileName(destinationFileName);
                    context.FileSystem.Copy(sourceFileName, destinationFileName);
                }
            }

            foreach (var sourceSubdirectory in context.FileSystem.GetDirectories(sourceDirectory))
            {
                var destinationSubdirectory = Path.Combine(destinationDirectory, Path.GetFileName(sourceSubdirectory));
                Copy(context, sourceSubdirectory, destinationSubdirectory, macros, textFileExtensions);
            }
        }

        [NotNull]
        protected virtual string ReplaceMacros([NotNull] string text, [NotNull] Dictionary<string, string> macros, [NotNull] string prefix, [NotNull] string postfix)
        {
            foreach (var keyValue in macros)
            {
                text = text.Replace(prefix + keyValue.Key + postfix, keyValue.Value);
            }

            return text;
        }

        Dictionary<string, string> IOptionPicker.GetOptions(string optionName, ITaskContext context)
        {
            var options = new Dictionary<string, string>();

            var generatorsDirectory = Path.Combine(context.Configuration.GetToolsDirectory(), "files\\generators");
            if (!context.FileSystem.DirectoryExists(generatorsDirectory))
            {
                return options;
            }

            foreach (var directory in context.FileSystem.GetDirectories(generatorsDirectory))
            {
                options[Path.GetFileName(directory).Replace("-", " ")] = directory;
            }

            return options;
        }
    }
}
