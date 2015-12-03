// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Axes;

namespace Sitecore.Pathfinder.Xml.XPath
{
    /// <summary>Parses a string into an Sitecore XPath query.</summary>
    /// <remarks>
    /// ExpressionOrQuery = Expression
    /// Queries = Query | ( Queries "|" Query )
    /// Query = PathStep ( PathStep )*
    /// PathStep = Node | Children | Descendants 
    /// Children = "/" Node
    /// Descendants = "//" Node
    /// Node = Attribute | Element
    /// Attribute = "@" Name
    /// Element = [ Axis ] ( "*" | "." | ".." | Name ) [ Predicate ]
    /// Name = Identifier | Name Identifier
    /// Axis = ( "ancestor" | "ancestor-or-self" | "descendants" | "descendants-or-self" |
    ///        "child" | "parent" | "following" | "preceding" | "self" ) "::"
    /// Predicate = "[" Expression "]"
    /// Expression = Term3 [ ( "or" | "xor" ) Term3 ]
    /// Term3 = Term2 [ "and" Term2 ]
    /// Term2 = Term1 [ ( "=" | ">" | ">=" | "&lt;" | "&lt;=" | "!=" ) Term1 ]
    /// Term1 = Term0 [ ( "+" | "-" ) Term0 ]
    /// Term0 = Factor [ ( "*" | "div" | "mod" ) Factor ]
    /// Factor = Queries | Literal | Number | True | False | "(" Expression ")" | 
    ///          Attribute | Function | Parameter
    /// Parameter = "$" Identifier
    /// Function = Identifier "(" ExpressionList ")"
    /// ExpressionList = Expression | ( ExpressionList "," Expression )
    /// </remarks>
    public class XPathParser
    {
        [NotNull]
        private XPathFactory _factory;

        private Token _token;

        [NotNull]
        private Tokenizer _tokenizer;

        [NotNull]
        public Opcode Parse([NotNull] string xpath)
        {
            return DoParse(xpath);
        }

        [NotNull]
        public Opcode ParsePredicate([NotNull] string xpath)
        {
            return DoParsePredicate(xpath);
        }

        [NotNull]
        protected virtual Opcode DoParse([NotNull] string xpath)
        {
            Initialize(xpath);

            var result = GetExpressionOrQuery();

            Match(TokenType.End, "End of string expected");

            return result;
        }

        [NotNull]
        protected virtual Opcode DoParseExpression([NotNull] string xpath)
        {
            Initialize(xpath);

            var result = GetExpression();

            Match(TokenType.End, "End of string expected");

            return result;
        }

        [NotNull]
        protected virtual Opcode DoParsePredicate([NotNull] string xpath)
        {
            Initialize(xpath);

            var result = _factory.Predicate(GetExpression());

            Match(TokenType.End, "End of string expected");

            return result;
        }

        [NotNull]
        protected StepBase GetAncestorAxis()
        {
            Match(TokenType.Ancestor, "\"ancestor::\" or \"ancestor-or-self::\" expected");

            StepBase result;

            if (_token.Type == TokenType.Minus)
            {
                Match();
                Match(TokenType.Or, "\"or\" expected");
                Match(TokenType.Minus, "\"-\" expected");
                Match(TokenType.Self, "\"self\" expected");
                Match(TokenType.Axis, "\"::\" expected");

                var element = GetElement();

                result = _factory.AncestorOrSelf(element);
            }
            else
            {
                Match(TokenType.Axis, "\"::\" expected");

                var element = GetElement();

                result = _factory.Ancestor(element);
            }

            return result;
        }

        [NotNull]
        protected ElementBase GetAttribute()
        {
            Match(TokenType.At, "\"@\" expected");

            var name = "";

            if (_token.Type == TokenType.At)
            {
                Match();
                name = "@";
            }

            name += _token.Value;

            Match(TokenType.Identifier, "Identifier expected");

            while (_token.Type == TokenType.Identifier || _token.Type == TokenType.Number)
            {
                name += _token.Whitespace + _token.Value;
                Match();
            }

            var result = _factory.FieldElement(name);

            return result;
        }

        [NotNull]
        protected StepBase GetChildAxis()
        {
            Match(TokenType.Child, "\"child::\" expected");
            Match(TokenType.Axis, "\"::\" expected");

            return _factory.Children(GetElement());
        }

        [CanBeNull]
        protected StepBase GetChildren()
        {
            Match(TokenType.Slash, "\"/\" expected");

            var result = GetNode();

            if (result is ElementBase)
            {
                result = _factory.Children(result as ElementBase);
            }

            if (!(result is StepBase))
            {
                Raise("Syntax error");
            }

            return result as StepBase;
        }

        [CanBeNull]
        protected StepBase GetDescendants()
        {
            Match(TokenType.DoubleSlash, "\"//\" expected");

            var result = GetNode();

            if (result is ElementBase)
            {
                result = _factory.Descendants(result as ElementBase);
            }

            if (!(result is StepBase))
            {
                Raise("Syntax error");
            }

            return result as StepBase;
        }

        [NotNull]
        protected StepBase GetDescendantsAxis()
        {
            Match(TokenType.Descendant, "\"descendants::\" or \"descendants-or-self::\"expected");

            if (_token.Type == TokenType.Minus)
            {
                Match();
                Match(TokenType.Or, "\"or\" expected");
                Match(TokenType.Minus, "\"-\" expected");
                Match(TokenType.Self, "\"self\" expected");
                Match(TokenType.Axis, "\"::\" expected");

                return _factory.DescendantsOrSelf(GetElement());
            }
            Match(TokenType.Axis, "\"::\" expected");

            return _factory.Descendants(GetElement());
        }

        [NotNull]
        protected StepBase GetDotDot()
        {
            Match(TokenType.DotDot, "\"..\" expected");

            return _factory.Parent();
        }

        [NotNull]
        protected ElementBase GetElement()
        {
            Predicate predicate = null;
            var name = "";

            switch (_token.Type)
            {
                case TokenType.Identifier:
                    name = _token.Value;
                    Match();

                    while (_token.Type == TokenType.Identifier || _token.Type == TokenType.Number)
                    {
                        name += _token.Whitespace + _token.Value;
                        Match();
                    }

                    break;

                case TokenType.Number:
                    name = _token.Value;
                    Match();

                    while (_token.Type == TokenType.Identifier || _token.Type == TokenType.Number)
                    {
                        name += _token.Whitespace + _token.Value;
                        Match();
                    }

                    break;

                case TokenType.Guid:
                    name = _token.Value;
                    Match();
                    break;

                case TokenType.Star:
                    name = "*";
                    Match();
                    break;

                default:
                    Raise("Identifier, GUID or \"*\" expected");
                    break;
            }

            if (_token.Type == TokenType.StartSquareBracket)
            {
                predicate = GetPredicate();
            }

            var result = _factory.ItemElement(name, predicate);

            return result;
        }

        [NotNull]
        protected Opcode GetExpression()
        {
            var result = GetTerm3();

            while (true)
            {
                switch (_token.Type)
                {
                    case TokenType.Or:
                        Match();
                        result = _factory.Or(result, GetTerm3());
                        break;

                    case TokenType.Xor:
                        Match();
                        result = _factory.Xor(result, GetTerm3());
                        break;

                    default:
                        return result;
                }
            }
        }

        [NotNull]
        protected Opcode GetFactor()
        {
            Opcode result = null;

            switch (_token.Type)
            {
                case TokenType.Literal:
                    result = _factory.Literal(_token.Value);
                    Match();
                    break;

                case TokenType.Number:
                    result = _factory.Number(_token.NumberValue);
                    Match();
                    break;

                case TokenType.Identifier:
                    if (Peek() == TokenType.StartParentes)
                    {
                        result = GetFunction();
                    }
                    else
                    {
                        result = GetQuery();
                    }
                    break;

                case TokenType.Ancestor:
                case TokenType.Child:
                case TokenType.Descendant:
                case TokenType.Dot:
                case TokenType.DotDot:
                case TokenType.DoubleSlash:
                case TokenType.Following:
                case TokenType.Guid:
                case TokenType.Parent:
                case TokenType.Preceding:
                case TokenType.Self:
                case TokenType.Slash:
                case TokenType.Star:
                    result = GetQueries();
                    break;

                case TokenType.True:
                    result = _factory.BooleanValue(true);
                    Match();
                    break;

                case TokenType.False:
                    result = _factory.BooleanValue(false);
                    Match();
                    break;

                case TokenType.At:
                    result = GetAttribute();
                    break;

                case TokenType.StartParentes:
                    Match();

                    result = GetExpression();

                    Match(TokenType.EndParentes, "\")\" expected");
                    break;

                case TokenType.Dollar:
                    result = GetParameter();
                    break;

                default:
                    Raise("Expression expected");
                    break;
            }

            if (result == null)
            {
                throw new XPathParseException("Expression expected");
            }

            return result;
        }

        [NotNull]
        protected StepBase GetFollowingAxis()
        {
            Match(TokenType.Following, "\"following::\" expected");
            Match(TokenType.Axis, "\"::\" expected");

            return _factory.Following(GetElement());
        }

        [NotNull]
        protected Opcode GetFunction()
        {
            var name = _token.Value;

            Match(TokenType.Identifier, "Function name expected");
            Match(TokenType.StartParentes, "\"(\" expected");

            var result = _factory.Function(name);

            if (_token.Type != TokenType.EndParentes)
            {
                bool more;
                do
                {
                    more = false;

                    var expression = GetExpression();
                    result.Add(expression);

                    if (_token.Type == TokenType.Comma)
                    {
                        Match();
                        more = true;
                    }
                }
                while (more);
            }

            Match(TokenType.EndParentes, "\")\" expected");

            return result;
        }

        [NotNull]
        protected object GetNode()
        {
            object result;

            switch (_token.Type)
            {
                case TokenType.Ancestor:
                    result = GetAncestorAxis();
                    break;

                case TokenType.At:
                    result = GetAttribute();
                    break;

                case TokenType.Child:
                    result = GetChildAxis();
                    break;

                case TokenType.Descendant:
                    result = GetDescendantsAxis();
                    break;

                case TokenType.DotDot:
                    result = GetDotDot();
                    break;

                case TokenType.Dot:
                    result = GetSelf();
                    break;

                case TokenType.Following:
                    result = GetFollowingAxis();
                    break;

                case TokenType.Parent:
                    result = GetParentAxis();
                    break;

                case TokenType.Preceding:
                    result = GetPrecedingAxis();
                    break;

                case TokenType.Self:
                    result = GetSelfAxis();
                    break;

                default:
                    result = GetElement();
                    break;
            }

            return result;
        }

        [NotNull]
        protected ElementBase GetParameter()
        {
            Match(TokenType.Dollar, "\"$\" expected");

            var name = _token.Value;

            Match(TokenType.Identifier, "Identifier expected");

            return _factory.QueryParameter(name);
        }

        [NotNull]
        protected StepBase GetParentAxis()
        {
            Match(TokenType.Parent, "\"parent::\" expected");
            Match(TokenType.Axis, "\"::\" expected");

            var result = _factory.Parent();

            result.NextStep = _factory.Children(GetElement());

            return result;
        }

        [CanBeNull]
        protected StepBase GetPathStep()
        {
            object result;

            if (_token.Type == TokenType.Slash)
            {
                result = GetChildren();
            }
            else if (_token.Type == TokenType.DoubleSlash)
            {
                result = GetDescendants();
            }
            else
            {
                result = GetNode();

                if (result is ElementBase)
                {
                    result = _factory.Children(result as ElementBase);
                }

                if (!(result is StepBase))
                {
                    Raise("Syntax error");
                }
            }

            return result as StepBase;
        }

        [NotNull]
        protected StepBase GetPrecedingAxis()
        {
            Match(TokenType.Preceding, "\"preceding::\" expected");
            Match(TokenType.Axis, "\"::\" expected");

            return _factory.Preceding(GetElement());
        }

        [NotNull]
        protected Predicate GetPredicate()
        {
            Match(TokenType.StartSquareBracket, "\"[\" expected");

            var result = _factory.Predicate(GetExpression());

            Match(TokenType.EndSquareBracket, "\"]\" expected");

            return result;
        }

        [NotNull]
        protected Opcode GetQueries()
        {
            var result = GetQuery();

            while (_token.Type == TokenType.Pipe)
            {
                Match();
                result = _factory.Add(result, GetQuery());
            }

            return result;
        }

        [NotNull]
        protected Opcode GetQuery()
        {
            StepBase result = null;

            if (_token.Type == TokenType.Slash || _token.Type == TokenType.DoubleSlash)
            {
                result = _factory.Root();
            }

            var step = GetPathStep();

            if (result != null)
            {
                result.NextStep = step;
            }
            else
            {
                result = step;
            }

            while (_token.Type == TokenType.Slash || _token.Type == TokenType.DoubleSlash

                //_token.Type == TokenType.Star ||
                //_token.Type == TokenType.At ||
                //_token.Type == TokenType.Guid ||
                //_token.Type == TokenType.Identifier
                )
            {
                if (step == null)
                {
                    throw new XPathParseException("Step expected");
                }

                step.NextStep = GetPathStep();
                step = step.NextStep;
            }

            if (result == null)
            {
                throw new XPathParseException("Query expected");
            }

            return result;
        }

        [NotNull]
        protected StepBase GetSelf()
        {
            Match(TokenType.Dot, "\".\" expected");

            Predicate predicate = null;
            if (_token.Type == TokenType.StartSquareBracket)
            {
                predicate = GetPredicate();
            }

            return _factory.Self(predicate);
        }

        [NotNull]
        protected StepBase GetSelfAxis()
        {
            Match(TokenType.Self, "\"self::\" expected");
            Match(TokenType.Axis, "\"::\" expected");

            Predicate predicate = null;
            if (_token.Type == TokenType.StartSquareBracket)
            {
                predicate = GetPredicate();
            }

            if (predicate == null)
            {
                throw new XPathParseException("Predicate expected");
            }

            return _factory.Self(predicate);
        }

        [NotNull]
        protected Opcode GetTerm0()
        {
            var result = GetFactor();

            while (true)
            {
                switch (_token.Type)
                {
                    case TokenType.Star:
                        Match();
                        result = _factory.Multiply(result, GetFactor());
                        break;

                    case TokenType.Div:
                        Match();
                        result = _factory.Divide(result, GetFactor());
                        break;

                    case TokenType.Mod:
                        Match();
                        result = _factory.Modulus(result, GetFactor());
                        break;

                    default:
                        return result;
                }
            }
        }

        [NotNull]
        protected Opcode GetTerm1()
        {
            var result = GetTerm0();

            while (true)
            {
                switch (_token.Type)
                {
                    case TokenType.Plus:
                        Match();
                        result = _factory.Add(result, GetTerm0());
                        break;

                    case TokenType.Minus:
                        Match();
                        result = _factory.Minus(result, GetTerm0());
                        break;

                    default:
                        return result;
                }
            }
        }

        [NotNull]
        protected Opcode GetTerm2()
        {
            var result = GetTerm1();

            while (true)
            {
                switch (_token.Type)
                {
                    case TokenType.Equal:
                        Match();
                        result = _factory.Equals(result, GetTerm1());
                        break;

                    case TokenType.Unequals:
                        Match();
                        result = _factory.Unequals(result, GetTerm1());
                        break;

                    case TokenType.Smaller:
                        Match();
                        result = _factory.Smaller(result, GetTerm1());
                        break;

                    case TokenType.SmallerOrEquals:
                        Match();
                        result = _factory.SmallerOrEquals(result, GetTerm1());
                        break;

                    case TokenType.GreaterOrEquals:
                        Match();
                        result = _factory.GreaterOrEquals(result, GetTerm1());
                        break;

                    case TokenType.Greater:
                        Match();
                        result = _factory.Greater(result, GetTerm1());
                        break;

                    default:
                        return result;
                }
            }
        }

        [NotNull]
        protected Opcode GetTerm3()
        {
            var result = GetTerm2();

            while (true)
            {
                switch (_token.Type)
                {
                    case TokenType.And:
                        Match();
                        result = _factory.And(result, GetTerm2());
                        break;

                    default:
                        return result;
                }
            }
        }

        protected void Match()
        {
            _token = _tokenizer.NextToken();
        }

        protected void Match(int type, [NotNull] string error)
        {
            MatchCheck(type, error);
            Match();
        }

        protected void MatchCheck(int type, [NotNull] string error)
        {
            if (_token.Type != type)
            {
                Raise(error);
            }
        }

        protected int Peek()
        {
            var t = _tokenizer.NextToken();

            _tokenizer.UngetToken(t);

            return t.Type;
        }

        [NotNull]
        private Opcode GetExpressionOrQuery()
        {
            return GetExpression();
        }

        private void Initialize([NotNull] string query)
        {
            _tokenizer = new Tokenizer(new TokenBuilder(), query);
            _factory = new XPathFactory();

            Match();
        }

        private void Raise([NotNull] string error)
        {
            throw new XPathParseException(error + " at position " + _token.Index + ".");
        }
    }
}
