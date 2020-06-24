using System.Collections.Generic;

namespace VerteX.Lexing
{
    /// <summary>
    /// Структура для работы с ключевыми словами.
    /// </summary>
    public class Keywords
    {
        /// <summary>
        /// Словарь всех ключевых слов на всех поддерживаемых языках.
        /// </summary>
        private static readonly Dictionary<KeywordType, Token> list = new Dictionary<KeywordType, Token>()
        {
            {KeywordType.If,       new Token(TokenType.Keyword, "если")},
            {KeywordType.Function, new Token(TokenType.Keyword, "функция")},
            {KeywordType.Else,     new Token(TokenType.Keyword, "иначе")},
            {KeywordType.True,     new Token(TokenType.Keyword, "истина")},
            {KeywordType.False,    new Token(TokenType.Keyword, "лож")},
            {KeywordType.While,    new Token(TokenType.Keyword, "пока")},
            {KeywordType.Do,       new Token(TokenType.Keyword, "делать")},
            {KeywordType.Try,      new Token(TokenType.Keyword, "пробовать")},
            {KeywordType.Catch,    new Token(TokenType.Keyword, "отловить")},
            {KeywordType.Switch,   new Token(TokenType.Keyword, "определить")},
            {KeywordType.Case,     new Token(TokenType.Keyword, "случай")},
            {KeywordType.Break,    new Token(TokenType.Keyword, "завершить")},
            {KeywordType.Default,  new Token(TokenType.Keyword, "по_умолчанию")},
            {KeywordType.Use,      new Token(TokenType.Keyword, "использовать")},
            {KeywordType.Links,    new Token(TokenType.Keyword, "ссылки")},
            {KeywordType.Import,   new Token(TokenType.Keyword, "импорт")}
        };

        /// <summary>
        /// Определяет, является ли токен ключевым словом.
        /// </summary>
        public static bool IsKeyword(Token token)
        {
            return list.ContainsValue(token);
        }

        /// <summary>
        /// Определяет, является ли строка ключевым словом.
        /// </summary>
        public static bool IsKeyword(string value)
        {
            return IsKeyword(new Token(TokenType.Keyword, value));
        }

        /// <summary>
        /// Возвращает тип ключевого слова.
        /// </summary>
        public static KeywordType GetKeywordType(Token token)
        {
            if (IsKeyword(token))
            {
                foreach (KeywordType type in list.Keys)
                {
                    if (list[type] == token)
                    {
                        return type;
                    }
                }
            }

            return KeywordType.Undefined;
        }
    }
}
