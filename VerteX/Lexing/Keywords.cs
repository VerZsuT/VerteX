using System.Collections.Generic;

namespace VerteX.Lexing
{
    /// <summary>
    /// Структура для работы с ключевыми словами.
    /// </summary>
    public class Keywords
    {
        /// <summary>
        /// Словарь всех ключевых слов языка.
        /// </summary>
        private static readonly Dictionary<KeywordType, Token> dict = new Dictionary<KeywordType, Token>()
        {
            {KeywordType.If, new Token(TokenType.Keyword, "если")},
            {KeywordType.Function, new Token(TokenType.Keyword, "функция")},
            {KeywordType.Else, new Token(TokenType.Keyword, "иначе")},
            {KeywordType.True, new Token(TokenType.Keyword, "истина")},
            {KeywordType.False, new Token(TokenType.Keyword, "лож")}
        };

        /// <summary>
        /// Определяет, является ли токен ключевым словом.
        /// </summary>
        public static bool IsKeyword(Token token)
        {
            return dict.ContainsValue(token);
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
                foreach (KeywordType type in dict.Keys)
                {
                    if (dict[type] == token)
                    {
                        return type;
                    }
                }
            }
            
            return KeywordType.Undefined;    
        }
    }
}
