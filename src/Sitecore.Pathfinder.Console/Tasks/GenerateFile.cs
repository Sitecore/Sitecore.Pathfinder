// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class GenerateFile : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateFile([NotNull] IFileSystemService fileSystem, [NotNull] IPathMapperService pathMapper) : base("generate-file")
        {
            FileSystem = fileSystem;
            PathMapper = pathMapper;

            Alias = "generate";
            Shortcut = "g";
        }

        [NotNull, Option("generator", Alias = "g", PositionalArg = 1, HasOptions = true, PromptText = "Pick generator")]
        public string GeneratorDirectory { get; set; } = string.Empty;

        [NotNull, Option("filename", Alias = "f", PositionalArg = 2, DefaultValue = "file")]
        public string FileName { get; set; } = string.Empty;

        [NotNull]
        public IFileSystemService FileSystem { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        public override void Run(IBuildContext context)
        {
            var generatorsDirectory = Path.Combine(context.ToolsDirectory, "files\\generators");

            context.Trace.TraceInformation(Msg.G1009, Texts.Generating_files___);

            if (!FileSystem.DirectoryExists(GeneratorDirectory))
            {
                if (GeneratorDirectory.IndexOf('/') < 0 && GeneratorDirectory.IndexOf('\\') < 0)
                {
                    GeneratorDirectory = Path.Combine(generatorsDirectory, GeneratorDirectory);
                }
            }

            if (!FileSystem.DirectoryExists(GeneratorDirectory))
            {
                var dir = string.Empty;
                foreach (var directory in FileSystem.GetDirectories(generatorsDirectory).OrderBy(d => d))
                {
                    if (directory.StartsWith(GeneratorDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(dir))
                        {
                            context.Trace.TraceError(Msg.G1019, "Ambiguous generator");
                            return;
                        }

                        dir = directory;
                    }
                }

                GeneratorDirectory = dir;
            }

            if (!FileSystem.DirectoryExists(GeneratorDirectory))
            {
                context.Trace.TraceError(Msg.G1018, Texts.Generator_not_found, GeneratorDirectory);
                return;
            }

            var macros = new Dictionary<string, string>();
            macros["name"] = FileName;

            var textFileExtensions = context.Configuration.GetArray(Constants.Configuration.GenerateFile.TextFileExtensions);

            Copy(context, GeneratorDirectory, Directory.GetCurrentDirectory(), macros, textFileExtensions);
        }

        protected virtual void Copy([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string destinationDirectory, [NotNull] Dictionary<string, string> macros, [ItemNotNull, NotNull] IEnumerable<string> textFileExtensions)
        {
            foreach (var sourceFileName in FileSystem.GetFiles(sourceDirectory))
            {
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));

                destinationFileName = ReplaceMacros(destinationFileName, macros, "__", "__");
                context.Trace.TraceInformation(Msg.G1000, "Generating file", PathHelper.UnmapPath(context.ProjectDirectory, destinationFileName));

                var extension = Path.GetExtension(sourceFileName);
                if (textFileExtensions.Any(t => string.Equals(t, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    var projectFileName = PathHelper.UnmapPath(context.ProjectDirectory, destinationFileName);

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

                    var content = FileSystem.ReadAllText(sourceFileName);

                    content = ReplaceMacros(content, macros, "<%= ", " %>");

                    FileSystem.CreateDirectoryFromFileName(destinationFileName);
                    FileSystem.WriteAllText(destinationFileName, content);
                }
                else
                {
                    FileSystem.CreateDirectoryFromFileName(destinationFileName);
                    FileSystem.Copy(sourceFileName, destinationFileName);
                }
            }

            foreach (var sourceSubdirectory in FileSystem.GetDirectories(sourceDirectory))
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

        [NotNull, OptionValues("GeneratorDirectory")]
        protected IEnumerable<(string Name, string Value)> GetInformationOptions([NotNull] ITaskContext context)
        {
            var generatorsDirectory = Path.Combine(context.Configuration.GetToolsDirectory(), "files\\generators");
            if (!FileSystem.DirectoryExists(generatorsDirectory))
            {
                yield break;
            }

            foreach (var directory in FileSystem.GetDirectories(generatorsDirectory))
            {
                yield return (Path.GetFileName(directory).Replace("-", " "), directory);
            }
        }
    }
}
