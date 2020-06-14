using System.Collections.Generic;
using VerteX.Compiling;

namespace VerteX.Lexing
{
    /// <summary>
    /// Базовая языковая единица.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Тип токена.
        /// </summary>
        public TokenType type;

        /// <summary>
        /// Содержание токена.
        /// </summary>
        public string value;


        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// Проверяет является ли токен данного типа.
        /// </summary>
        public bool TypeIs(TokenType type)
        {
            return this.type == type;
        }

        /// <summary>
        /// Проверяет является ли токен данного типа.
        /// </summary>
        public bool TypeIs(KeywordType type)
        {
            return Keywords.GetKeywordType(this) == type;
        }

        public override bool Equals(object obj)
        {
            return obj is Token token &&
                   type == token.type &&
                   value == token.value;
        }

        public override int GetHashCode()
        {
            var hashCode = 1148455455;
            hashCode = hashCode * -1521134295 + type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(value);
            return hashCode;
        }

        /// <summary>
        /// Возвращает строку валидного кода.
        /// </summary>
        public override string ToString()
        {
            if (type == TokenType.String)
            {
                return $"\"{value}\"";
            }
            else if (type == TokenType.Id)
            {
                return CodeManager.TransformId(value);
            }
            else if (TypeIs(KeywordType.True))
            {
                return "true";
            }
            else if (TypeIs(KeywordType.False))
            {
                return "false";
            }

            return value;
        }

        public static bool operator ==(Token left, Token right)
        {
            return (left.value == right.value) && (left.type == right.type);
        }

        public static bool operator !=(Token left, Token right)
        {
            return (left.value != right.value) || (left.type != right.type);
        }
    }
}
