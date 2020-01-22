using System.Collections.Generic;
using VerteX.Lexing;
using VerteX.Compiling;
using VerteX.Parsing.ParseExeptions;

namespace VerteX.Parsing
{
    public static class Parser
    {
        private static ParseMode parseMode = ParseMode.Default;
        private static int lineIndex = 0;

        public static void Parse(List<Token> lineTokens)
        {
            lineIndex++;

            if (lineTokens.Count == 0) return;

            if (Check.IsFunctionCall(lineTokens))
            {
                ParseFunctionCall(lineTokens);
            }
            else if (Check.IsVariableSet(lineTokens))
            {
                ParseVariableSet(lineTokens);
            }
            else if (Check.IsFunctionCreation(lineTokens))
            {
                ParseFunctionCreation(lineTokens);
            }
            else if (Check.IsEndFunctionCreation(lineTokens))
            {
                ParseEndFunctionCreation(lineTokens);
            }
            else
            {
                throw new LineParsingException(lineIndex);
            }
        }

        private static void ParseFunctionCall(List<Token> lineTokens)
        {
            List<Token> funcAttrTokens = GetFunctionAttributes(lineTokens);
            string methodName = lineTokens[0].value;

            if (Check.IsBaseValue(funcAttrTokens))
            {
                Token attribute = funcAttrTokens[0];
                if (parseMode == ParseMode.Default)
                {
                    CodeGenerator.Main.AddFunctionCall(methodName, attribute);
                }
                else
                {
                    CodeGenerator.NewFunction.AddFunctionCall(methodName, attribute);
                }
            }
            else if (Check.IsVariableName(funcAttrTokens))
            {
                string variableName = funcAttrTokens[0].value;
                if (parseMode == ParseMode.Default)
                {
                    CodeGenerator.Main.AddFunctionCall(methodName, variableName);
                }
                else
                {
                    CodeGenerator.NewFunction.AddFunctionCall(methodName, variableName);
                }
            }
            else if (Check.IsExpression(funcAttrTokens)) { }
            else
            {
                if (parseMode == ParseMode.Default)
                {
                    CodeGenerator.Main.AddFunctionCall(methodName);
                }
                else
                {
                    CodeGenerator.NewFunction.AddFunctionCall(methodName);
                }
            }
        }

        private static void ParseVariableSet(List<Token> lineTokens)
        {
            string variableName = lineTokens[0].value;
            List<Token> valueTokens = lineTokens.GetRange(2, lineTokens.Count - 3);

            if (Check.IsBaseValue(valueTokens) || Check.IsVariableName(valueTokens))
            {
                Token valueToken = valueTokens[0];
                if (parseMode == ParseMode.Default)
                {
                    CodeGenerator.Main.AddVariableAssignment(variableName, valueToken);
                }
                else
                {
                    CodeGenerator.NewFunction.AddVariableAssignment(variableName, valueToken);
                }
            }
        }        

        private static void ParseFunctionCreation(List<Token> lineTokens)
        {
            string methodName = lineTokens[1].value;

            if (Check.IsHaveParams(lineTokens))
            {
                List<string> parameters = GetFunctionParameters(lineTokens);
                CodeGenerator.NewFunction.AddHeader(methodName, parameters);
            }
            else
            {
                CodeGenerator.NewFunction.AddHeader(methodName);
            }

            parseMode = ParseMode.FunctionCreation;
        }

        private static void ParseEndFunctionCreation(List<Token> lineTokens)
        {
            CodeGenerator.NewFunction.Create();
            parseMode = ParseMode.Default;
        }

        private static List<string> GetFunctionParameters(List<Token> lineTokens)
        {
            List<string> functionParameters = new List<string>();

            foreach (Token paramsToken in lineTokens.GetRange(3, lineTokens.Count - 5))
            {
                if (paramsToken.value != ",")
                {
                    functionParameters.Add(paramsToken.value);
                }
            }
            return functionParameters;
        }

        private static List<Token> GetFunctionAttributes(List<Token> lineTokens)
        {
            return lineTokens.GetRange(2, lineTokens.Count - 4);
        }
    }
}
