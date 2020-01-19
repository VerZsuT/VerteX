using System.Collections.Generic;

namespace VerteX.Lexing
{
    public static class Lexer
    {
        private static int currentIndex;
        private static string source;

        public static List<Token> Lex(string str)
        {
            List<Token> tokenList = new List<Token>();

            currentIndex = 0;
            source = str;

            while (currentIndex < source.Length)
            {
                char ch = GetCurrentChar();

                if (char.IsWhiteSpace(ch) || char.IsControl(ch))
                {
                    ch = GetNextChar();
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    tokenList.Add(new Token(TokenType.Number, ReadNumber()));
                }
                else if (ch == '=')
                {
                    tokenList.Add(new Token(TokenType.Operator, ReadOperator()));
                }
                else if (char.IsLetter(ch))
                {
                    tokenList.Add(new Token(TokenType.Id, ReadId()));
                }
                else if (ch == '"')
                {
                    tokenList.Add(new Token(TokenType.String, ReadString()));
                }
                else
                {
                    switch (ch)
                    {
                        case '(':
                            tokenList.Add(new Token(TokenType.BeginParenthesis, "("));
                            break;
                        case ')':
                            tokenList.Add(new Token(TokenType.EndParenthesis, ")"));
                            break;
                        case '{':
                            tokenList.Add(new Token(TokenType.BeginBrace, "{"));
                            break;
                        case '}':
                            tokenList.Add(new Token(TokenType.EndBrace, "}"));
                            break;
                        case '[':
                            tokenList.Add(new Token(TokenType.BeginSquereBracket, "["));
                            break;
                        case ']':
                            tokenList.Add(new Token(TokenType.EndParenthesis, "]"));
                            break;
                        case ';':
                            tokenList.Add(new Token(TokenType.ComandEnd, ";"));
                            break;
                        case '.':
                            tokenList.Add(new Token(TokenType.Operator, "."));
                            break;
                    }
                    GetNextChar();
                }

            }

            return tokenList;
        }
        
        private static char GetNextChar()
        {
            if (++currentIndex < source.Length)
            {
                return source[currentIndex];
            }
            else
            {
                return ' ';
            }
        }

        private static char GetCurrentChar()
        {
            return source[currentIndex];
        }

        private static string ReadNumber()
        {
            char ch = GetCurrentChar();
            string number = "";

            while (char.IsDigit(ch))
            {
                number += ch;
                ch = GetNextChar();
            }

            return number;
        }

        private static string ReadString()
        {
            char ch = GetNextChar();
            string str = "";

            while (ch != '"')
            {
                str += ch;
                ch = GetNextChar();
            }

            GetNextChar();

            return str;
        }

        private static string ReadOperator()
        {
            char ch = GetCurrentChar();
            string op = "";

            while (IsOperator(ch))
            {
                op += ch;
                ch = GetNextChar();
            }

            return op;
        }

        private static string ReadId()
        {
            char ch = GetCurrentChar();
            string id = "";

            while (char.IsLetter(ch))
            {
                id += ch;
                ch = GetNextChar();
            }

            return id;
        }

        private static bool IsOperator(char ch)
        {
            switch (ch)
            {
                case '=':
                case '-':
                case '+':
                case '/':
                case '*':
                case '<':
                case '>':
                case '?':
                case ':':
                case '|':
                case '&':
                case '!':
                case '.':
                    return true;
                default:
                    return false;
            }
        }
    }
}