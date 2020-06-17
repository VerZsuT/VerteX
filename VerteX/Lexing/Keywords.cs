using System.Collections.Generic;
using VerteX.Compiling;

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
        private static readonly Dictionary<string, Dictionary<KeywordType, Token>> langs = new Dictionary<string, Dictionary<KeywordType, Token>>()
        {
            {"ru", new Dictionary<KeywordType, Token> () {
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
                {KeywordType.Default,  new Token(TokenType.Keyword, "по_умолчанию")} 
            }},
            {"en", new Dictionary<KeywordType, Token> () {
                {KeywordType.If,       new Token(TokenType.Keyword, "if")},
                {KeywordType.Function, new Token(TokenType.Keyword, "function")},
                {KeywordType.Else,     new Token(TokenType.Keyword, "else")},
                {KeywordType.True,     new Token(TokenType.Keyword, "true")},
                {KeywordType.False,    new Token(TokenType.Keyword, "false")},
                {KeywordType.While,    new Token(TokenType.Keyword, "while")},
                {KeywordType.Do,       new Token(TokenType.Keyword, "do")},
                {KeywordType.Try,      new Token(TokenType.Keyword, "try")},
                {KeywordType.Catch,    new Token(TokenType.Keyword, "catch")},
                {KeywordType.Switch,   new Token(TokenType.Keyword, "switch")},
                {KeywordType.Case,     new Token(TokenType.Keyword, "case")},
                {KeywordType.Break,    new Token(TokenType.Keyword, "break")},
                {KeywordType.Default,  new Token(TokenType.Keyword, "default")}
            }},
        };

        /// <summary>
        /// Определяет, является ли токен ключевым словом.
        /// </summary>
        public static bool IsKeyword(Token token)
        {
            return langs[CodeManager.Lang].ContainsValue(token);
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
                foreach (KeywordType type in langs[CodeManager.Lang].Keys)
                {
                    if (langs[CodeManager.Lang][type] == token)
                    {
                        return type;
                    }
                }
            }
            
            return KeywordType.Undefined;    
        }
    }
}
