using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sitecore.Pathfinder.Code.Tests.FieldDeclaration
{
    public class SitecorePath
    {
        private const string SitecorePathString = "/sitecore/content";
        private readonly SyntaxTree _tree;

        public SitecorePath(string text)
        {
            _tree = CSharpSyntaxTree.ParseText(text);
        }

        public void Start()
        {
            var root = (CompilationUnitSyntax) _tree.GetRoot();
            var namespaceDeclaration = root.Members[0];
            var templatesStruct = (NamespaceDeclarationSyntax) namespaceDeclaration;

            var fieldDeclarations = from fieldDeclaration in root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                select fieldDeclaration;

            var t = fieldDeclarations.ToList();
            var i = t[4];
            var type = i.Declaration.Type;

            var isString = type.GetText().ToString().Trim() == "string";

            var variables = i.Declaration.Variables;

            var lastToken = variables[0].GetLastToken().ValueText.Contains(SitecorePathString);
        }
    }
}
