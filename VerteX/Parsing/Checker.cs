﻿using VerteX.Lexing;

namespace VerteX.Parsing
{
    /// <summary>
    /// Предоставляет методы проверки токенов для определения типа команды.
    /// </summary>
    public static class Checker
    {
        /// <summary>
        /// Определяет имеет ли конструкция начало тела.
        /// </summary>
        public static bool IsBeginBody(TokenList lineTokens)
        {
            return IsFunctionCreation(lineTokens) || IsIfConstruction(lineTokens) || IsElseConstruction(lineTokens);
        }

        /// <summary>
        /// Определяет имеет ли конструкция конец тела.
        /// </summary>
        public static bool IsEndingBody(TokenList lineTokens)
        {
            return IsEndBrace(lineTokens) || IsElseConstruction(lineTokens);
        }

        /// <summary>
        /// Определяет вызов функции.
        /// </summary>
        public static bool IsFunctionCall(TokenList lineTokens)
        {
            if (lineTokens.Count < 4) return false;

            return lineTokens[0].TypeIs(TokenType.Id) &&
                   lineTokens[1].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-2].TypeIs(TokenType.EndParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.ComandEnd);
        }

        /// <summary>
        /// Определяет, имеет ли функция параметры при объявлении.
        /// </summary>
        public static bool IsHaveParams(TokenList lineTokens)
        {
            if (lineTokens.Count > 5) return true;
            return false;
        }

        /// <summary>
        /// Определяет присвоение переменной.
        /// </summary>
        public static bool IsVariableSet(TokenList lineTokens)
        {
            if (lineTokens.Count > 3)
            {
                return lineTokens[0].TypeIs(TokenType.Id) &&
                       lineTokens[1].TypeIs(TokenType.AssignOperator) &&
                       lineTokens[-1].TypeIs(TokenType.ComandEnd);
            }
            return false;
        }

        /// <summary>
        /// Определяет объявление функции.
        /// </summary>
        public static bool IsFunctionCreation(TokenList lineTokens)
        {
            if (lineTokens.Count < 5) return false;

            return lineTokens[0].TypeIs(KeywordType.Function) &&
                   lineTokens[1].TypeIs(TokenType.Id) &&
                   lineTokens[2].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-2].TypeIs(TokenType.EndParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет закрывающую фигурную скобку.
        /// </summary>
        public static bool IsEndBrace(TokenList lineTokens)
        {
            if (lineTokens.Count > 1) return false;

            return lineTokens[0].TypeIs(TokenType.EndBrace);
        }

        /// <summary>
        /// Определяет конструкцию IF.
        /// </summary>
        public static bool IsIfConstruction(TokenList lineTokens)
        {
            if (lineTokens.Count <= 4) return false;

            return lineTokens[0].TypeIs(KeywordType.If) &&
                   lineTokens[1].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-2].TypeIs(TokenType.EndParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет конструкцию ELSE.
        /// </summary>
        public static bool IsElseConstruction(TokenList lineTokens)
        {
            if (lineTokens.Count < 3) return false;
            return lineTokens[0].TypeIs(TokenType.EndBrace) &&
                   lineTokens[1].TypeIs(KeywordType.Else) &&
                   lineTokens[2].TypeIs(TokenType.BeginBrace);
        }
    }
}
