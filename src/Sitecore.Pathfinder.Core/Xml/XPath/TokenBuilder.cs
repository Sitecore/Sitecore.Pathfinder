using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class TokenBuilder
    {
        public Token Token;

        public void At()
        {
            Token.Type = TokenType.At;
        }

        public void Axis()
        {
            Token.Type = TokenType.Axis;
        }

        public void Comma()
        {
            Token.Type = TokenType.Comma;
        }

        public void Dollar()
        {
            Token.Type = TokenType.Dollar;
        }

        public void Dot()
        {
            Token.Type = TokenType.Dot;
        }

        public void DotDot()
        {
            Token.Type = TokenType.DotDot;
        }

        public void DoubleSlash()
        {
            Token.Type = TokenType.DoubleSlash;
        }

        public void End()
        {
            Token.Type = TokenType.End;
        }

        public void EndParentes()
        {
            Token.Type = TokenType.EndParentes;
        }

        public void EndSquareBracket()
        {
            Token.Type = TokenType.EndSquareBracket;
        }

        public void Equals()
        {
            Token.Type = TokenType.Equal;
        }

        public void Greater()
        {
            Token.Type = TokenType.Greater;
        }

        public void GreaterOrEquals()
        {
            Token.Type = TokenType.GreaterOrEquals;
        }

        public void Guid([NotNull] string value)
        {
            Token.Type = TokenType.Guid;
            Token.Value = value;
        }

        public virtual void Identifier([NotNull] string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "ancestor":
                    Token.Type = TokenType.Ancestor;
                    break;
                case "and":
                    Token.Type = TokenType.And;
                    break;
                case "child":
                    Token.Type = TokenType.Child;
                    break;
                case "descendant":
                    Token.Type = TokenType.Descendant;
                    break;
                case "div":
                    Token.Type = TokenType.Div;
                    break;
                case "false":
                    Token.Type = TokenType.False;
                    break;
                case "following":
                    Token.Type = TokenType.Following;
                    break;
                case "mod":
                    Token.Type = TokenType.Mod;
                    break;
                case "or":
                    Token.Type = TokenType.Or;
                    break;
                case "parent":
                    Token.Type = TokenType.Parent;
                    break;
                case "preceding":
                    Token.Type = TokenType.Preceding;
                    break;
                case "self":
                    Token.Type = TokenType.Self;
                    break;
                case "true":
                    Token.Type = TokenType.True;
                    break;
                case "xor":
                    Token.Type = TokenType.Xor;
                    break;
                default:
                    Token.Type = TokenType.Identifier;
                    Token.Value = value;
                    break;
            }
        }

        public void Literal([NotNull] string name)
        {
            Token.Type = TokenType.Literal;
            Token.Value = name;
        }

        public void Minus()
        {
            Token.Type = TokenType.Minus;
        }

        public void Namespace()
        {
            Token.Type = TokenType.Namespace;
        }

        public void Number(int value)
        {
            Token.Type = TokenType.Number;
            Token.Value = value.ToString();
            Token.NumberValue = value;
        }

        public void Pipe()
        {
            Token.Type = TokenType.Pipe;
        }

        public void Plus()
        {
            Token.Type = TokenType.Plus;
        }

        public virtual void QuotedIdentifier([NotNull] string value)
        {
            Token.Type = TokenType.Identifier;
            Token.Value = value;
        }

        public void Slash()
        {
            Token.Type = TokenType.Slash;
        }

        public void Smaller()
        {
            Token.Type = TokenType.Smaller;
        }

        public void SmallerOrEquals()
        {
            Token.Type = TokenType.SmallerOrEquals;
        }

        public void Star()
        {
            Token.Type = TokenType.Star;
            Token.Value = "*";
        }

        public void StartParentes()
        {
            Token.Type = TokenType.StartParentes;
        }

        public void StartSquareBracket()
        {
            Token.Type = TokenType.StartSquareBracket;
        }

        public void Unequals()
        {
            Token.Type = TokenType.Unequals;
        }
    }
}