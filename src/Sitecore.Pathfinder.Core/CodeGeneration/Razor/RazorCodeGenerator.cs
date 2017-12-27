// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RazorLight;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.CodeGeneration.Razor
{
    [Export(typeof(ICodeGenerator)), Shared]
    public class RazorCodeGenerator : ICodeGenerator
    {
        [ImportingConstructor]
        public RazorCodeGenerator([NotNull] IFactory factory, [NotNull] IFileSystem fileSystem)
        {
            Factory = factory;
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public void Generate(IBuildContext context, ITextTemplatingEngine textTemplatingEngine, IProjectBase project)
        {
            var extension = '.' + context.Configuration.GetString(Constants.Configuration.GenerateCode.Extension, ".*").TrimStart('.');
            var pathMatcher = Factory.PathMatcher(context.Configuration.GetString(Constants.Configuration.GenerateCode.Include), context.Configuration.GetString(Constants.Configuration.GenerateCode.Exclude));

            foreach (var fileName in FileSystem.GetFiles(context.ProjectDirectory, "*" + extension, SearchOption.AllDirectories))
            {
                if (!pathMatcher.IsMatch(fileName))
                {
                    continue;
                }

                context.Trace.TraceInformation(Msg.G1008, Texts.Generating_code, PathHelper.UnmapPath(context.ProjectDirectory, fileName));

                try
                {
                    var targetFileName = fileName.Left(fileName.Length - extension.Length);
                    var template = FileSystem.ReadAllText(fileName);

                    var output = textTemplatingEngine.Generate(template, project);

                    if (string.IsNullOrEmpty(output))
                    {
                        FileSystem.DeleteFile(targetFileName);
                        continue;
                    }

                    if (output.IndexOf("<pagebreak", StringComparison.Ordinal) < 0)
                    {
                        FileSystem.WriteAllText(targetFileName, output);
                        continue;
                    }

                    SplitOutputIntoFiles(output, fileName);
                }
                catch (TemplateParsingException ex)
                {
                    foreach (var parserError in ex.ParserErrors)
                    {
                        context.Trace.TraceError(Msg.G1007, parserError.Message, fileName, new TextSpan(parserError.Location.LineIndex, parserError.Location.CharacterIndex, parserError.Length));
                    }
                }
                catch (TemplateCompilationException ex)
                {
                    foreach (var compilationError in ex.CompilationErrors)
                    {
                        context.Trace.TraceError(Msg.G1007, compilationError, fileName, TextSpan.Empty);
                    }
                }
                catch (Exception ex)
                {
                    context.Trace.TraceError(Msg.G1007, ex.Message, fileName, TextSpan.Empty);
                }
            }
        }

        protected virtual void SplitOutputIntoFiles([NotNull] string output, [NotNull] string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            var targetFileName = string.Empty;
            var content = new StringBuilder();
            var reader = new StringReader(output);
            var lineNumber = 0;
            do
            {
                lineNumber++;

                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                var trimmedLine = line.Trim();
                if (!trimmedLine.StartsWith("<pagebreak", StringComparison.Ordinal) || !trimmedLine.EndsWith("/>", StringComparison.Ordinal))
                {
                    content.Append(line);
                    content.Append(Environment.NewLine);
                    continue;
                }

                if (!string.IsNullOrEmpty(targetFileName) && content.Length > 0)
                {
                    FileSystem.WriteAllText(targetFileName, content.ToString());
                }

                var match = Regex.Match(line, "file=\"([^\"]*)\"");
                if (!match.Success)
                {
                    throw new InvalidOperationException("'file' attribute expected in line " + lineNumber);
                }

                targetFileName = PathHelper.Combine(directory, match.Groups[1].Value);
                content.Clear();
            }
            while (true);

            if (!string.IsNullOrEmpty(targetFileName) && content.Length > 0)
            {
                FileSystem.WriteAllText(targetFileName, content.ToString());
            }
        }
    }
}
