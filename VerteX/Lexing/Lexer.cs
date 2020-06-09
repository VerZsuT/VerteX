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
        /// Массив строк для распозназнавания.
        /// </summary>
        private static string[] source;

        /// <summary>
        /// Текущая распознаваемая линия.
        /// </summary>
        private static string currentLine;

        /// <summary>
        /// Разбивает строку на базовые языковые единицы (токены).
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Список токенов для парсинга.</returns>
        public static List<TokenList> Lex(string code)
        {
            List<TokenList> tokens = new List<TokenList>();

            source = code.Split('\n');

            foreach (string line in source)
            {
                currentIndex = 0;
                TokenList tokenList = new TokenList();
                currentLine = line;
                while (currentIndex < line.Length)
                {
                    char ch = GetCurrentChar();

                    if (char.IsWhiteSpace(ch) || char.IsControl(ch))
                    {
                        currentIndex++;
                        continue;
                    }

                    if (char.IsDigit(ch))
                    {
                        tokenList.Add(TokenType.Number, ReadNumber());
                    }
                    else if (GetOperatorType(ch) != TokenType.UndefinedOperator)
                    {
                        string op = ReadOperator();

                        tokenList.Add(GetOperatorType(op), op);
                    }
                    else if (char.IsLetter(ch))
                    {
                        string id = ReadId();
                        if (Keywords.IsKeyword(id))
                        {
                            tokenList.Add(TokenType.Keyword, id);
                        }
                        else
                        {
                            tokenList.Add(TokenType.Id, id);
                        }
                    }
                    else if (ch == '"')
                    {
                        tokenList.Add(TokenType.String, ReadString('"'));
                    }
                    else if (ch == '\'')
                    {
                        tokenList.Add(TokenType.String, ReadString('\''));
                    }
                    else
                    {
                        tokenList.Add(GetOperatorType(ch), ch);
                    }
                    GetNextChar();
                }
                tokens.Add(tokenList);
            }

            return tokens;
        }

        /// <summary>
        /// Получает следующий символ в строке и увеличивает индекс.
        /// </summary>
        private static char GetNextChar()
        {
            if (++currentIndex < currentLine.Length)
            {
                return currentLine[currentIndex];
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
            return currentLine[currentIndex];
        }

        /// <summary>
        /// Считывает число и переводит индекс.
        /// </summary>
        private static string ReadNumber()
        {
            char ch = GetCurrentChar();
            string number = "";

            while (char.IsDigit(ch))
            {
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

            while (char.IsLetter(ch) || char.IsDigit(ch))
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
            }

            List<string> arithmeticOperators = new List<string>()
            {
                "-", "+", "/", "*"
            };
            List<string> logicOperators = new List<string>()
            {
                ">", "<", "&", "|", "!", "==", "!=", "<=", ">="
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

            return TokenType.UndefinedOperator;
        }
    }
}