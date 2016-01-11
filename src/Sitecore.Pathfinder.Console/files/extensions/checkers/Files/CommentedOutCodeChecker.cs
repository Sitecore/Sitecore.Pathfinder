using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.files.extensions.checkers.Files
{
    public class CommentedOutCodeChecker : CheckerBase
    {
        [NotNull]
        private SyntaxTree Tree { get; set; }

        public CommentedOutCodeChecker() : base("Commented out code", Files)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var file in context.Project.Files.Where(file => Path.GetExtension(file.FilePath) == ".cs"))
            {
                using (var stream = File.OpenRead(file.QualifiedName))
                {
                    Tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: file.FilePath);
                }

                var root = (CompilationUnitSyntax)Tree.GetRoot();
                var nameSpaceDeclaration = root.Members[0];

                var childNodes= nameSpaceDeclaration.ChildNodes();
                bool hasComments = false;
                foreach (SyntaxNode childNode in childNodes)
                {
                    if (childNode.ChildTokens().Any(token => token.ValueText.Contains("/*")))
                    {
                        hasComments = true;
                        context.Project.Diagnostics.Add(new Projects.Diagnostic(Msg.CR2000, file.QualifiedName, Snapshots.TextSpan.Empty, Severity.Warning, "Commented code has been found. Please use source control instead of commenting out irrelevant code. "));

                    }
                }
            }
        }
    }
}
