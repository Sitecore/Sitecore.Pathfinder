// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class TokenType
    {
        public const int At = 10;

        public const int Axis = 11;

        public const int Comma = 12;

        public const int Dollar = 13;

        public const int Dot = 14;

        public const int DotDot = 15;

        public const int DoubleSlash = 16;

        public const int End = 1;

        public const int EndParentes = 18;

        public const int EndSquareBracket = 17;

        public const int Equal = 34;

        public const int Greater = 19;

        public const int GreaterOrEquals = 20;

        public const int Guid = 35;

        public const int Identifier = 21;

        public const int Literal = 22;

        public const int Minus = 23;

        public const int Namespace = 24;

        public const int Number = 25;

        public const int Pipe = 26;

        public const int Plus = 27;

        public const int Slash = 33;

        public const int Smaller = 28;

        public const int SmallerOrEquals = 29;

        public const int Star = 30;

        public const int StartParentes = 32;

        public const int StartSquareBracket = 31;

        public const int Unequals = 36;
    }

    public struct Token
    {
        public int Index;

        public int NumberValue;

        public int Type;

        [NotNull]
        public string Value;

        [NotNull]
        public string Whitespace;

        internal bool Empty;
    }

    public class Tokenizer
    {
        public const string IdentifierCharacters = "_.";

        [NotNull]
        private readonly TokenBuilder _builder;

        [NotNull]
        private readonly char[] _text;

        private int _index;

        private Token _nextToken;

        public Tokenizer([NotNull] TokenBuilder builder, [NotNull] string text)
        {
            _builder = builder;
            _text = text.ToCharArray();
            _nextToken.Empty = true;
            _index = 0;
        }

        public virtual Token NextToken()
        {
            // ungetted token
            if (!_nextToken.Empty)
            {
                var result = _nextToken;
                _nextToken.Empty = true;
                return result;
            }

            _builder.Token.Index = _index;

            // whitespace
            var whitespace = new StringBuilder();
            while (_index < _text.Length && Char.IsWhiteSpace(_text[_index]))
            {
                whitespace.Append(_text[_index]);
                _index++;
            }
            _builder.Token.Whitespace = whitespace.ToString();

            // token
            if (_index >= _text.Length)
            {
                _builder.End();
            }
            else
            {
                switch (_text[_index])
                {
                    case '/':
                        _index++;
                        if (_text[_index] == '/')
                        {
                            _index++;
                            _builder.DoubleSlash();
                        }
                        else
                        {
                            _builder.Slash();
                        }
                        break;

                    case '=':
                        _index++;
                        _builder.Equals();
                        break;

                    case '@':
                        _index++;
                        _builder.At();
                        break;

                    case '*':
                        _index++;
                        _builder.Star();
                        break;

                    case '"':
                        _index++;
                        _builder.Literal(NextString('"'));
                        break;

                    case '\'':
                        _index++;
                        _builder.Literal(NextString('\''));
                        break;

                    case '{':
                        _builder.Guid(NextGuid());
                        break;

                    case '[':
                        _index++;
                        _builder.StartSquareBracket();
                        break;

                    case '+':
                        _index++;
                        _builder.Plus();
                        break;

                    case '-':
                        _index++;
                        if (Char.IsDigit(_text[_index]))
                        {
                            _builder.Number(-NextNumber());
                        }
                        else
                        {
                            _builder.Minus();
                        }
                        break;

                    case ']':
                        _index++;
                        _builder.EndSquareBracket();
                        break;

                    case '(':
                        _index++;
                        _builder.StartParentes();
                        break;

                    case ')':
                        _index++;
                        _builder.EndParentes();
                        break;

                    case '$':
                        _index++;
                        _builder.Dollar();
                        break;

                    case ',':
                        _index++;
                        _builder.Comma();
                        break;

                    case '>':
                        _index++;
                        if (_text[_index] == '=')
                        {
                            _index++;
                            _builder.GreaterOrEquals();
                        }
                        else
                        {
                            _builder.Greater();
                        }
                        break;

                    case '<':
                        _index++;
                        if (_text[_index] == '=')
                        {
                            _index++;
                            _builder.SmallerOrEquals();
                        }
                        else
                        {
                            _builder.Smaller();
                        }
                        break;

                    case '!':
                        _index++;
                        if (_text[_index] == '=')
                        {
                            _index++;
                            _builder.Unequals();
                        }
                        else
                        {
                            throw new QueryException("!= expected");
                        }
                        break;

                    case '|':
                        _index++;
                        _builder.Pipe();
                        break;

                    case '.':
                        _index++;
                        if (_text[_index] == '.')
                        {
                            _index++;
                            _builder.DotDot();
                        }
                        else
                        {
                            _builder.Dot();
                        }
                        break;

                    case ':':
                        _index++;
                        if (_text[_index] == ':')
                        {
                            _index++;
                            _builder.Axis();
                        }
                        else
                        {
                            _builder.Namespace();
                        }
                        break;

                    case '#':
                        _index++;
                        _builder.QuotedIdentifier(NextQuotedIdentifier());
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        _builder.Number(NextNumber());
                        break;

                    default:
                        if (Char.IsLetterOrDigit(_text[_index]) || IdentifierCharacters.IndexOf(_text[_index]) >= 0)
                        {
                            _builder.Identifier(NextIdentifier());
                        }
                        else
                        {
                            throw new QueryException("Unexpected character '" + _text[_index] + "'");
                        }
                        break;
                }
            }

            return _builder.Token;
        }

        public Token PeekToken()
        {
            var result = NextToken();
            UngetToken(result);
            return result;
        }

        public void UngetToken(Token token)
        {
            _nextToken = token;
        }

        [NotNull]
        private string NextGuid()
        {
            if (_index + 38 <= _text.Length)
            {
                var result = new StringBuilder();

                var n = _index + 38;

                while (_index < n)
                {
                    result.Append(_text[_index]);
                    _index++;
                }

                var id = result.ToString();

                Guid guid;
                if (Guid.TryParse(id, out guid))
                {
                    return id;
                }
            }

            throw new ParseException("Guid expected");
        }

        [NotNull]
        private string NextIdentifier()
        {
            var result = new StringBuilder();

            while (_index < _text.Length && (Char.IsLetterOrDigit(_text[_index]) || IdentifierCharacters.IndexOf(_text[_index]) >= 0))
            {
                result.Append(_text[_index]);
                _index++;
            }

            return result.ToString();
        }

        private int NextNumber()
        {
            var result = 0;

            while (_index < _text.Length && Char.IsDigit(_text[_index]))
            {
                result = result * 10 + (int)Char.GetNumericValue(_text[_index]) - (int)Char.GetNumericValue('0');
                _index++;
            }

            return result;
        }

        [NotNull]
        private string NextQuotedIdentifier()
        {
            var result = new StringBuilder();

            while (_index < _text.Length && _text[_index] != '#')
            {
                result.Append(_text[_index]);
                _index++;
            }

            if (_index >= _text.Length)
            {
                throw new ParseException("Unterminated identifier");
            }

            _index++;

            return result.ToString();
        }

        [NotNull]
        private string NextString(char end)
        {
            var result = new StringBuilder();

            while (_index < _text.Length && _text[_index] != end)
            {
                result.Append(_text[_index]);
                _index++;
            }

            if (_index >= _text.Length)
            {
                throw new ParseException("Unterminated string");
            }

            _index++;

            return result.ToString();
        }
    }

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
            QuotedIdentifier(value);
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
