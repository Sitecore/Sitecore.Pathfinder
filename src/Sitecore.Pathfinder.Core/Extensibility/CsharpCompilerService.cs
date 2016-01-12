// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    public class CsharpCompilerService
    {
        [CanBeNull]
        public Assembly Compile([NotNull] string toolsDirectory, [NotNull] string assemblyFileName, [NotNull] [ItemNotNull] IEnumerable<string> sourceFileNames)
        {
            // check if assembly is newer than all checkers
            if (File.Exists(assemblyFileName))
            {
                var writeTime = File.GetLastWriteTimeUtc(assemblyFileName);
                if (sourceFileNames.All(f => File.GetLastWriteTimeUtc(f) < writeTime))
                {
                    return Assembly.LoadFrom(assemblyFileName);
                }
            }

            var collectionsFileName = Path.Combine(toolsDirectory, "System.Collections.Immutable.dll");
            if (!File.Exists(collectionsFileName))
            {
                Console.WriteLine(Texts.System_Collections_Immutable_dll_is_missing__Extensions_will_not_be_loaded_);
                return null;
            }

            var codeAnalysisFileName = Path.Combine(toolsDirectory, "Microsoft.CodeAnalysis.dll");
            if (!File.Exists(codeAnalysisFileName))
            {
                Console.WriteLine(Texts.Microsoft_CodeAnalysis_dll_is_missing__Extensions_will_not_be_loaded_);
                return null;
            }

            // compile extensions
            Console.WriteLine(Texts.scc_cmd_0_0___information_SCC0000__Compiling_checkers___);

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
