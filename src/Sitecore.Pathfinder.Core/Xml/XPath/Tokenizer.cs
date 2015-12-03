// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
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
                        if (_index < _text.Length && _text[_index] == '/')
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
                        if (_index < _text.Length && _text[_index] == '=')
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
                        if (_index < _text.Length && _text[_index] == '=')
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
                        if (_index < _text.Length && _text[_index] == '=')
                        {
                            _index++;
                            _builder.Unequals();
                        }
                        else
                        {
                            throw new XPathException("!= expected");
                        }
                        break;

                    case '|':
                        _index++;
                        _builder.Pipe();
                        break;

                    case '.':
                        _index++;
                        if (_index < _text.Length && _text[_index] == '.')
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
                        if (_index < _text.Length && _text[_index] == ':')
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
                    case '^':
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
                            throw new XPathException("Unexpected character '" + _text[_index] + "'");
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

            throw new XPathParseException("Guid expected");
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

            while (_index < _text.Length && _text[_index] != '#' && _text[_index] != '^')
            {
                result.Append(_text[_index]);
                _index++;
            }

            if (_index >= _text.Length)
            {
                throw new XPathParseException("Unterminated identifier");
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
                throw new XPathParseException("Unterminated string");
            }

            _index++;

            return result.ToString();
        }
    }
}
