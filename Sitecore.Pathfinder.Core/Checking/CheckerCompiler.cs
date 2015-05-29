namespace Sitecore.Pathfinder.Checking
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using Microsoft.CodeAnalysis;
  using Microsoft.CodeAnalysis.CSharp;
  using Microsoft.CodeAnalysis.Emit;
  using Sitecore.Pathfinder.Diagnostics;

  public class CheckerCompiler
  {
    [CanBeNull]
    public Assembly GetAssembly([NotNull] string checkersDirectory)
    {
      var fileNames = Directory.GetFiles(checkersDirectory, "*.cs", SearchOption.AllDirectories);

      // check if assembly is newer than all checkers
      var checkerAssemblyFileName = Path.Combine(checkersDirectory, "Sitecore.Pathfinder.Checker.dll");
      if (File.Exists(checkerAssemblyFileName))
      {
        var checkerWriteTime = File.GetLastWriteTimeUtc(checkerAssemblyFileName);
        if (fileNames.All(f => File.GetLastWriteTimeUtc(f) < checkerWriteTime))
        {
          return Assembly.LoadFrom(checkerAssemblyFileName);
        }
      }

      // compile all checkers
      Console.WriteLine(Texts.scc_cmd_0_0___information_SCC0000__Compiling_checkers___);

      var syntaxTrees = fileNames.Select(File.ReadAllText).Select(code => CSharpSyntaxTree.ParseText(code)).ToList();
      var references = AppDomain.CurrentDomain.GetAssemblies().Select(MetadataReference.CreateFromAssembly).ToList();
      var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

      var compilation = CSharpCompilation.Create("Sitecore.Pathfinder.Checker", syntaxTrees, references, options);

      EmitResult result;
      using (var stream = new FileStream(checkerAssemblyFileName, FileMode.Create))
      {
        result = compilation.Emit(stream);
      }

      if (result.Success)
      {
        return Assembly.LoadFrom(checkerAssemblyFileName);
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
