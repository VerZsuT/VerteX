using System.Collections.Generic;

namespace VerteX.Lexing
{
    /// <summary>
    /// Анализатор текста.
    /// </summary>
    public static class Lexer
    {
        /// <summary>
        /// Индекс текущего распознаваемого символа.
        /// </summary>
        private static int currentIndex;

        /// <summary>
        /// Хранит весь код для распознавания.
        /// </summary>
        private static string fullCode;

        /// <summary>
        /// Разбивает строку на базовые языковые единицы (токены).
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Список токенов для парсинга.</returns>
        public static TokenList Lex(string code)
        {
            fullCode = code;
            TokenList tokens = new TokenList();
            currentIndex = 0;

            while (currentIndex < code.Length)
            {
                char ch = GetCurrentChar();

                if (char.IsWhiteSpace(ch) || char.IsControl(ch))
                {
                    currentIndex++;
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    tokens.Add(TokenType.Number, ReadNumber());
                }
                else if (GetOperatorType(ch) != TokenType.UndefinedOperator)
                {
                    string op = ReadOperator();

                    tokens.Add(GetOperatorType(op), op);
                }
                else if (char.IsLetter(ch))
                {
                    string id = ReadId();
                    if (Keywords.IsKeyword(id))
                    {
                        tokens.Add(TokenType.Keyword, id);
                    }
                    else
                    {
                        tokens.Add(TokenType.Id, id);
                    }
                }
                else if (ch == '"')
                {
                    tokens.Add(TokenType.String, ReadString('"'));
                }
                else if (ch == '\'')
                {
                    tokens.Add(TokenType.String, ReadString('\''));
                }
                else
                {
                    tokens.Add(GetOperatorType(ch), ch);
                }
                GetNextChar();
            }

            return tokens;
        }

        /// <summary>
        /// Получает следующий символ в строке и увеличивает индекс.
        /// </summary>
        private static char GetNextChar()
        {
            if (++currentIndex < fullCode.Length)
            {
                return fullCode[currentIndex];
            }
            else
            {
                return ' ';
            }
        }

        /// <summary>
        /// Получает текущий символ в строке.
        /// </summary>
        private static char GetCurrentChar()
        {
            return fullCode[currentIndex];
        }

        /// <summary>
        /// Считывает число и переводит индекс.
        /// </summary>
        private static string ReadNumber()
        {
            char ch = GetCurrentChar();
            string number = "";

            while (char.IsDigit(ch) || ch == '.')
            {
                if (ch == '.')
                {
                    string firstPart = number;
                    string secondPart = "";

                    ch = GetNextChar();
                    while (char.IsDigit(ch))
                    {
                        secondPart += ch;
                        ch = GetNextChar();
                    }
                    currentIndex--;
                    if (secondPart == "") 
                    { 
                        return firstPart; 
                    }
                    else
                    {
                        return $"{firstPart}.{secondPart}";
                    }
                }

                number += ch;
                ch = GetNextChar();
            }
            currentIndex--;
            return number;
        }

        /// <summary>
        /// Считывает строку и переводит индекс.
        /// </summary>
        /// <param name="quote">Разделитель строки (одинарная или двойная кавычка).</param>
        private static string ReadString(char quote)
        {
            char ch = GetNextChar();
            string str = "";


            while (ch != quote)
            {
                str += ch;
                ch = GetNextChar();
            }

            return str;
        }

        /// <summary>
        /// Считывает оператор и переводит индекс.
        /// </summary>
        private static string ReadOperator()
        {
            char ch = GetCurrentChar();
            string beforeOp = "";
            string newOp = ch.ToString();

            while (GetOperatorType(newOp) != TokenType.UndefinedOperator)
            {
                beforeOp += ch;
                ch = GetNextChar();
                newOp += ch;
            }
            currentIndex--;
            return beforeOp;
        }

        /// <summary>
        /// Считывает идентификатор и переводит индекс.
        /// </summary>
        private static string ReadId()
        {
            char ch = GetCurrentChar();
            string id = "";

            while (char.IsLetter(ch) || char.IsDigit(ch) || ch == '_')
            {
                id += ch;
                ch = GetNextChar();
            }
            currentIndex--;
            return id;
        }

        /// <summary>
        /// Возвращает тип оператора.
        /// </summary>
        private static TokenType GetOperatorType(char ch)
        {
            return GetOperatorType(ch.ToString());
        }

        /// <summary>
        /// Возвращает тип оператора.
        /// </summary>
        private static TokenType GetOperatorType(string op)
        {
            switch (op)
            {
                case "(":
                    return TokenType.BeginParenthesis;
                case ")":
                    return TokenType.EndParenthesis;
                case "{":
                    return TokenType.BeginBrace;
                case "}":
                    return TokenType.EndBrace;
                case "[":
                    return TokenType.BeginSquereBracket;
                case "]":
                    return TokenType.EndParenthesis;
                case ";":
                    return TokenType.ComandEnd;
                case ":":
                    return TokenType.Colon;
            }

            List<string> arithmeticOperators = new List<string>()
            {
                "-", "+", "/", "*"
            };
            List<string> logicOperators = new List<string>()
            {
                ">", "<", "&", "&&", "|", "||", "!", "==", "!=", "<=", ">="
            };

            if (arithmeticOperators.Contains(op))
            {
                return TokenType.ArithmeticalOperator;
            }
            else if (logicOperators.Contains(op))
            {
                return TokenType.LogicOperator;
            }
            else if (op == ".")
            {
                return TokenType.Dot;
            }
            else if (op == "=")
            {
                return TokenType.AssignOperator;
            }
            else if (op == ",")
            {
                return TokenType.Comma;
            }

            return TokenType.UndefinedOperator;
        }
    }
}