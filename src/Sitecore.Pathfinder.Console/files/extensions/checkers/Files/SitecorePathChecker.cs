using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Diagnostic = Sitecore.Pathfinder.Projects.Diagnostic;
using TextSpan = Sitecore.Pathfinder.Snapshots.TextSpan;

namespace Sitecore.Pathfinder.files.extensions.checkers.Files
{
    public class SitecorePathChecker : CheckerBase
    {
        private const string SitecorePathString = "/sitecore/content";

        public SitecorePathChecker() : base("Sitecore Path Checker", Files)
        {
        }

        [NotNull]
        private SyntaxTree Tree { get; set; }

        public override void Check(ICheckerContext context)
        {
            foreach (var file in context.Project.Files.Where(file => Path.GetExtension(file.FilePath) == ".cs"))
            {
                using (var stream = File.OpenRead(file.QualifiedName))
                {
                    Tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: file.FilePath);
                }

                var root = (CompilationUnitSyntax) Tree.GetRoot();
                var namespaceDeclaration = root.Members[0];
                var templatesStruct = (NamespaceDeclarationSyntax) namespaceDeclaration;

                var fieldDeclarations = from fieldDeclaration in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                    select fieldDeclaration;

                var fieldDeclarationList =
                    fieldDeclarations.Where(fd => fd.Declaration.Type.ToString().Trim() == "string").ToList();

                var illegalDeclarations = GetIllegalFieldDeclarations(fieldDeclarationList);

                if (!illegalDeclarations.Any())
                    return;

                foreach (var fieldDeclarationSyntax in illegalDeclarations)
                {
                    context.Project.Diagnostics.Add(new Diagnostic(Msg.CR1000, file.QualifiedName, TextSpan.Empty,
                        Severity.Warning,
                        "You should not reference Sitecore items by path. Replace with Sitecore ID if possible"));
                }
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<FieldDeclarationSyntax> GetIllegalFieldDeclarations(
            List<FieldDeclarationSyntax> fieldDeclarationList)
        {
            if (fieldDeclarationList == null)
                throw new ArgumentNullException(nameof(fieldDeclarationList));


            return
                fieldDeclarationList.Where(
                    fieldDeclarationSyntax =>
                        fieldDeclarationSyntax.Declaration.Variables.Any(
                            v => v.GetLastToken().ValueText.Contains(SitecorePathString)));
        }
    }
}