// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    public class ExtensionsCompiler
    {
        [CanBeNull]
        public Assembly GetExtensionsAssembly([NotNull] string extensionsDirectory)
        {
            var fileNames = Directory.GetFiles(extensionsDirectory, "*.cs", SearchOption.AllDirectories);

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
        public Assembly GetUnitTestAssembly([NotNull] string fileName)
        {
            var code = File.ReadAllText(fileName);

            var syntaxTrees = new[]
            {
                CSharpSyntaxTree.ParseText(code)
            };

            var nunitFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "nunit.framework.dll");

            var references = AppDomain.CurrentDomain.GetAssemblies().Select(MetadataReference.CreateFromAssembly).ToList();
            references.Add(MetadataReference.CreateFromFile(nunitFileName));
            references.Add(MetadataReference.CreateFromAssembly(typeof(HttpUtility).Assembly));
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create("Sitecore.Pathfinder.Tests", syntaxTrees, references, options);

            EmitResult result;
            using (var stream = new MemoryStream())
            {
                result = compilation.Emit(stream);

                if (result.Success)
                {
                    // todo: load in AppDomain
                    stream.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(stream.ToArray());
                }
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
