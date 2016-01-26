// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Sitecore.Pathfinder.Roslyn.Extensibility
{
    public class CsharpCompilerService
    {
        public Assembly Compile(string toolsDirectory, string assemblyFileName, IEnumerable<string> sourceFileNames)
        {
            // to do: if a *.sln file exists, compile that. Otherwise look for *.csproj files. Lastly compile all C# files. 
            // Unfortunately CodeAnalysis.Workspaces is a 6Mb payload and depends on MSBuild, so postponed for now.

            if (!sourceFileNames.Any())
            {
                return null;
            }

            // check if assembly is newer than all checkers
            if (File.Exists(assemblyFileName))
            {
                var writeTime = File.GetLastWriteTimeUtc(assemblyFileName);
                if (sourceFileNames.All(f => File.GetLastWriteTimeUtc(f) < writeTime))
                {
                    return Assembly.LoadFrom(assemblyFileName);
                }
            }

            // check if Roslyn is available
            var collectionsFileName = Path.Combine(toolsDirectory, "System.Collections.Immutable.dll");
            if (!File.Exists(collectionsFileName))
            {
                Console.WriteLine("System.Collections.Immutable.dll is missing. Extensions will not be loaded");
                return null;
            }

            var codeAnalysisFileName = Path.Combine(toolsDirectory, "Microsoft.CodeAnalysis.dll");
            if (!File.Exists(codeAnalysisFileName))
            {
                Console.WriteLine("Microsoft.CodeAnalysis.dll is missing. extensions will not be loaded");
                return null;
            }

            return CompileExtensions(toolsDirectory, assemblyFileName, sourceFileNames);
        }

        protected Assembly CompileExtensions(string toolsDirectory, string assemblyFileName, IEnumerable<string> sourceFileNames)
        {
            Console.WriteLine("Compiling extensions...");

            var syntaxTrees = sourceFileNames.Select(File.ReadAllText).Select(code => CSharpSyntaxTree.ParseText(code)).ToList();

            var references = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => MetadataReference.CreateFromFile(assembly.Location)).ToList();

            // todo: add references from scconfig.json
            references.Add(MetadataReference.CreateFromFile(typeof(XDocument).Assembly.Location)); // add System.Xml.Linq assembly

            Directory.CreateDirectory(Path.GetDirectoryName(assemblyFileName) ?? string.Empty);

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(assemblyFileName), syntaxTrees, references, options);

            EmitResult result;
            using (var stream = new FileStream(assemblyFileName, FileMode.Create))
            {
                result = compilation.Emit(stream);
            }

            if (result.Success)
            {
                return Assembly.LoadFrom(assemblyFileName);
            }

            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            foreach (var diagnostic in failures)
            {
                Console.WriteLine(@"scc.cmd(0,0): {0} {1}: {2}", diagnostic.Severity, diagnostic.Id, diagnostic.GetMessage());
            }

            return null;
        }
    }
}
