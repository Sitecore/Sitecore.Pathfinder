// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    public class CsharpCompiler
    {
        [CanBeNull]
        public Assembly GetExtensionsAssembly([NotNull] string extensionsDirectory, [NotNull] IEnumerable<string> directories)
        {
            var fileNames = new List<string>();
            foreach (var directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    fileNames.AddRange(Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories));
                }
            }

            // check if assembly is newer than all checkers
            var extensionsAssemblyFileName = Path.Combine(extensionsDirectory, Constants.ExtensionsAssemblyFileName);
            if (File.Exists(extensionsAssemblyFileName))
            {
                var writeTime = File.GetLastWriteTimeUtc(extensionsAssemblyFileName);
                if (fileNames.All(f => File.GetLastWriteTimeUtc(f) < writeTime))
                {
                    return Assembly.LoadFrom(extensionsAssemblyFileName);
                }
            }

            // compile extensions
            Console.WriteLine(Texts.scc_cmd_0_0___information_SCC0000__Compiling_checkers___);

            var syntaxTrees = fileNames.Select(File.ReadAllText).Select(code => CSharpSyntaxTree.ParseText(code)).ToList();
            var references = AppDomain.CurrentDomain.GetAssemblies().Select(MetadataReference.CreateFromAssembly).ToList();
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create("Sitecore.Pathfinder.Extensions", syntaxTrees, references, options);

            EmitResult result;
            using (var stream = new FileStream(extensionsAssemblyFileName, FileMode.Create))
            {
                result = compilation.Emit(stream);
            }

            if (result.Success)
            {
                return Assembly.LoadFrom(extensionsAssemblyFileName);
            }

            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            foreach (var diagnostic in failures)
            {
                Console.WriteLine(@"scc.cmd(0,0): {0} {1}: {2}", diagnostic.Severity, diagnostic.Id, diagnostic.GetMessage());
            }

            return null;
        }

        [CanBeNull]
        public Assembly GetUnitTestAssembly([NotNull] IEnumerable<string> references, [NotNull] IEnumerable<string> fileNames)
        {
            var assemblyFileName = Path.ChangeExtension(Path.GetTempFileName(), ".dll");
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyFileName) ?? string.Empty;

            var syntaxTrees = fileNames.Select(s =>
            {
                var code = File.ReadAllText(s);
                return CSharpSyntaxTree.ParseText(code).WithFilePath(s);
            }).ToList();

            var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(MetadataReference.CreateFromAssembly).ToList();
            refs.AddRange(references.Select(r => MetadataReference.CreateFromFile(r)));

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, refs, options);

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
                var span = diagnostic.Location.GetLineSpan();

                // todo: hmm... hardcoded
                var fileName = "Tests\\server\\" + Path.GetFileName(diagnostic.Location.SourceTree.FilePath ?? string.Empty);

                var lineInfo = $"{fileName}({span.Span.Start.Line + 1},{span.Span.Start.Character + 1},{span.Span.End.Line + 1},{span.Span.End.Character + 1})";

                Console.WriteLine(@"{0}: {1} {2}: {3}", lineInfo, diagnostic.Severity, diagnostic.Id, diagnostic.GetMessage());
            }

            return null;
        }
    }
}
