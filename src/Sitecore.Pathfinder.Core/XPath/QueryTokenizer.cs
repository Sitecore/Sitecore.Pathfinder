// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.XPath
{
    public class QueryTokenType : TokenType
    {
        public const int Ancestor = 100;

        public const int And = 101;

        public const int Child = 102;

        public const int Descendant = 103;

        public const int Div = 104;

        public const int False = 107;

        public const int Following = 108;

        public const int Mod = 110;

        public const int Or = 111;

        public const int Parent = 112;

        public const int Preceding = 113;

        public const int Self = 114;

        public const int True = 115;

        public const int Xor = 117;
    }

    public class QueryTokenBuilder : TokenBuilder
    {
        public override void Identifier(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "ancestor":
                    Token.Type = QueryTokenType.Ancestor;
                    break;
                case "and":
                    Token.Type = QueryTokenType.And;
                    break;
                case "child":
                    Token.Type = QueryTokenType.Child;
                    break;
                case "descendant":
                    Token.Type = QueryTokenType.Descendant;
                    break;
                case "div":
                    Token.Type = QueryTokenType.Div;
                    break;
                case "false":
                    Token.Type = QueryTokenType.False;
                    break;
                case "following":
                    Token.Type = QueryTokenType.Following;
                    break;
                case "mod":
                    Token.Type = QueryTokenType.Mod;
                    break;
                case "or":
                    Token.Type = QueryTokenType.Or;
                    break;
                case "parent":
                    Token.Type = QueryTokenType.Parent;
                    break;
                case "preceding":
                    Token.Type = QueryTokenType.Preceding;
                    break;
                case "self":
                    Token.Type = QueryTokenType.Self;
                    break;
                case "true":
                    Token.Type = QueryTokenType.True;
                    break;
                case "xor":
                    Token.Type = QueryTokenType.Xor;
                    break;
                default:
                    Token.Type = TokenType.Identifier;
                    Token.Value = value;
                    break;
            }
        }
    }
}
