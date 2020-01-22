using System.Collections.Generic;
using VerteX.Lexing;

namespace VerteX.Parsing
{
    public static class Check
    {
        public static bool IsFunctionCall(List<Token> lineTokens)
        {
            if (lineTokens.Count < 4) return false;

            bool oneIsId = lineTokens[0].type == TokenType.Id;
            bool twoIsBeginParenthesis = lineTokens[1].type == TokenType.BeginParenthesis;
            bool preLastIsEndParenthesis = lineTokens[lineTokens.Count - 2].type == TokenType.EndParenthesis;
            bool lastIsComandEnd = lineTokens[lineTokens.Count - 1].type == TokenType.ComandEnd;

            return oneIsId && twoIsBeginParenthesis && preLastIsEndParenthesis && lastIsComandEnd;
        }

        public static bool IsHaveParams(List<Token> lineTokens)
        {
            if (lineTokens.Count > 5) return true;

            return false;
        }

        public static bool IsVariableSet(List<Token> lineTokens)
        {
            if (lineTokens.Count > 3)
            {
                bool oneIsId = lineTokens[0].type == TokenType.Id;
                bool twoIsOperator = lineTokens[1].value == "=";
                bool threeIsBaseValue = IsBaseValue(new List<Token>() { lineTokens[2] });
                bool threeIsVariable = IsVariableName(new List<Token>() { lineTokens[2] });
                bool lastIsComandEnd = lineTokens[lineTokens.Count - 1].type == TokenType.ComandEnd;

                return oneIsId && twoIsOperator && lastIsComandEnd && (threeIsBaseValue || threeIsVariable);
            }
            return false;
        }

        public static bool IsFunctionCreation(List<Token> lineTokens)
        {
            if (lineTokens.Count < 5) return false;

            bool oneIsKeyword = lineTokens[0].value == "функция";
            bool thoIsId = lineTokens[1].type == TokenType.Id;
            bool theeIsBeginParenthesis = lineTokens[2].type == TokenType.BeginParenthesis;
            bool foreIsEndParenthesis = lineTokens[lineTokens.Count - 2].type == TokenType.EndParenthesis;
            bool fiveIsBeginBrace = lineTokens[lineTokens.Count - 1].type == TokenType.BeginBrace;

            return oneIsKeyword && thoIsId && theeIsBeginParenthesis && foreIsEndParenthesis && fiveIsBeginBrace;
        }

        public static bool IsEndFunctionCreation(List<Token> lineTokens)
        {
            if (lineTokens.Count > 1) return false;

            return lineTokens[0].type == TokenType.EndBrace;
        }

        public static bool IsBaseValue(List<Token> attributesTokens)
        {
            if (attributesTokens.Count != 1) return false;

            bool isString = attributesTokens[0].type == TokenType.String;
            bool isNumber = attributesTokens[0].type == TokenType.Number;

            return isString || isNumber;
        }

        public static bool IsVariableName(List<Token> attributesTokens)
        {
            if (attributesTokens.Count != 1) return false;

            return attributesTokens[0].type == TokenType.Id;
        }

        public static bool IsExpression(List<Token> tokens)
        {
            return false;
        }
    }
}
