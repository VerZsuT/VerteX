using System;
using System.Collections.Generic;
using VerteX.Lexing;
using VerteX.VirtualMachine;

namespace VerteX.Parsing
{
    public static class Parser
    {
        private static ParseMode parseMode = ParseMode.Default;
        private static int lineIndex = 0;

        private static Dictionary<string, List<string>> objectNames = new Dictionary<string, List<string>>()
        {
            {"IO", new List<string>() {"печать", "ввод"}},
            {"UserMethods", new List<string>()}
        };

        private static string createMethodName = "";
        private static List<string> createMethodCode = new List<string>();
        private static List<string> mainMethodVariables = new List<string>();
        private static List<string> createMethodVariables = new List<string>();
        private static List<string> createMethodParamsInfo = new List<string>();

        public static void Parse(List<Token> tokens)
        {
            lineIndex++;

            if (tokens.Count == 0) return;

            if (IsFunctionCall(tokens))
            {
                ParseFunctionCall(tokens);
            }
            else if (IsVariableSet(tokens))
            {
                ParseVariableSet(tokens);
            }
            else if (IsFunctionCreation(tokens))
            {
                ParseFunctionCreation(tokens);
            }
            else if (IsEndFunctionCreation(tokens))
            {
                ParseEndFunctionCreation(tokens);
            }
            else
            {
                Console.WriteLine($"VerteX[ParserError]: Не удалось распознать строку {lineIndex}.");
                throw new Exception();
            }
        }

        private static void ParseFunctionCall(List<Token> tokens)
        {
            List<Token> functionAttributes = GetFunctionAttributes(tokens);
            string methodName = TransformFunctionName(tokens[0].value);

            if (IsBaseValue(functionAttributes))
            {
                Variable attribute = GetVariable(functionAttributes[0]);
                if (parseMode == ParseMode.Default)
                {
                    VVMachine.AddToMain($"{methodName}({attribute});");
                }
                else
                {
                    AddToCreate($"{methodName}({attribute});");
                }
            }
            else if (IsVariableName(functionAttributes))
            {
                string variableName = functionAttributes[0].value;
                if (parseMode == ParseMode.Default)
                {
                    VVMachine.AddToMain($"{methodName}({variableName});");
                }
                else
                {
                    AddToCreate($"{methodName}({variableName});");
                }
            }
            else if (IsExpression(functionAttributes)) { }
            else
            {
                if (parseMode == ParseMode.Default)
                {
                    VVMachine.AddToMain($"{methodName}();");
                }
                else
                {
                    AddToCreate($"{methodName}();");
                }
            }
        }

        private static void ParseVariableSet(List<Token> tokens)
        {
            string variableName = tokens[0].value;
            List<Token> valueTokens = tokens.GetRange(2, tokens.Count - 3);

            if (IsBaseValue(valueTokens) || IsVariableName(valueTokens))
            {
                Token valueToken = valueTokens[0];
                if (parseMode == ParseMode.Default)
                {
                    if (mainMethodVariables.Contains(variableName))
                    {
                        VVMachine.AddToMain($"{variableName} = {valueToken};");
                    }
                    else
                    {
                        mainMethodVariables.Add(variableName);
                        VVMachine.AddToMain($"var {variableName} = {valueToken};");
                    }
                }
                else
                {
                    if (createMethodVariables.Contains(variableName))
                    {
                        AddToCreate($"{variableName} = {valueToken};");
                    }
                    else
                    {
                        createMethodVariables.Add(variableName);
                        AddToCreate($"var {variableName} = {valueToken};");
                    }
                }
            }
        }

        private static void AddToCreate(string code)
        {
            createMethodCode.Add($"\t\t\t{code}\n");
        }

        private static void ParseFunctionCreation(List<Token> tokens)
        {
            createMethodName = tokens[1].value;
            if (IsHaveParams(tokens))
            {
                createMethodParamsInfo = GetFunctionParams(tokens);
            }
            else
            {
                createMethodParamsInfo = new List<string>();
            }

            parseMode = ParseMode.FunctionCreation;
        }

        private static void ParseEndFunctionCreation(List<Token> tokens)
        {
            List<string> methodParamsList = new List<string>();
            objectNames["UserMethods"].Add(createMethodName);

            foreach (string param in createMethodParamsInfo)
            {
                methodParamsList.Add("dynamic " + param);
            }

            string methodParams = string.Join(",", methodParamsList);
            string methodHeader = $"\n\t\tpublic static void {createMethodName}({methodParams})\n\t\t{{\n";
            string methodFooter = $"\n\t\t}}\n";

            VVMachine.CreateFunction(methodHeader + string.Concat(createMethodCode) + methodFooter);

            mainMethodVariables.Clear();
            createMethodName = "";
            createMethodVariables.Clear();
            createMethodCode = new List<string>();
            createMethodParamsInfo = new List<string>();
            parseMode = ParseMode.Default;
        }

        private static string TransformFunctionName(string methodName)
        {
            foreach (string name in objectNames.Keys)
            {
                foreach (string method in objectNames[name])
                {
                    if (method == methodName)
                    {
                        methodName = $"{name}.{methodName}";
                    }
                }
            }
            return methodName;
        }

        private static bool IsFunctionCall(List<Token> tokens)
        {
            if (tokens.Count < 4) return false;

            bool oneIsId = tokens[0].type == TokenType.Id;
            bool twoIsBeginParenthesis = tokens[1].type == TokenType.BeginParenthesis;
            bool preLastIsEndParenthesis = tokens[tokens.Count - 2].type == TokenType.EndParenthesis;
            bool lastIsComandEnd = tokens[tokens.Count - 1].type == TokenType.ComandEnd;

            return oneIsId && twoIsBeginParenthesis && preLastIsEndParenthesis && lastIsComandEnd;
        }

        private static bool IsHaveParams(List<Token> tokens)
        {
            if (tokens.Count > 5) return true;

            return false;
        }

        private static bool IsVariableSet(List<Token> tokens)
        {
            if (tokens.Count > 3)
            {
                bool oneIsId = tokens[0].type == TokenType.Id;
                bool twoIsOperator = tokens[1].value == "=";
                bool threeIsBaseValue = IsBaseValue(new List<Token>() { tokens[2] });
                bool threeIsVariable = IsVariableName(new List<Token>() { tokens[2] });
                bool lastIsComandEnd = tokens[tokens.Count - 1].type == TokenType.ComandEnd;

                return oneIsId && twoIsOperator && lastIsComandEnd && (threeIsBaseValue || threeIsVariable);
            }
            return false;
        }

        private static bool IsFunctionCreation(List<Token> tokens)
        {
            if (tokens.Count < 5) return false;

            bool oneIsKeyword = tokens[0].value == "функция";
            bool thoIsId = tokens[1].type == TokenType.Id;
            bool theeIsBeginParenthesis = tokens[2].type == TokenType.BeginParenthesis;
            bool foreIsEndParenthesis = tokens[tokens.Count - 2].type == TokenType.EndParenthesis;
            bool fiveIsBeginBrace = tokens[tokens.Count - 1].type == TokenType.BeginBrace;

            return oneIsKeyword && thoIsId && theeIsBeginParenthesis && foreIsEndParenthesis && fiveIsBeginBrace;
        }

        private static bool IsEndFunctionCreation(List<Token> tokens)
        {
            if (tokens.Count > 1) return false;

            return tokens[0].type == TokenType.EndBrace;
        }

        private static bool IsBaseValue(List<Token> attributes)
        {
            if (attributes.Count != 1) return false;

            bool isString = attributes[0].type == TokenType.String;
            bool isNumber = attributes[0].type == TokenType.Number;

            return isString || isNumber;
        }

        private static bool IsVariableName(List<Token> attributes)
        {
            if (attributes.Count != 1) return false;

            return attributes[0].type == TokenType.Id;
        }

        private static bool IsExpression(List<Token> tokens)
        {
            return false;
        }

        private static VariableType GetBaseType(string str)
        {
            if (str[0] == '\"' && str[str.Length - 1] == '\"')
            {
                return VariableType.String;
            }
            return VariableType.Integer;
        }

        private static Variable GetVariable(Token token)
        {
            VariableType variableType = VariableType.Undefined;

            switch (token.type)
            {
                case TokenType.String:
                    variableType = VariableType.String;
                    break;
                case TokenType.Number:
                    variableType = VariableType.Integer;
                    break;
            }
            return new Variable(variableType, token.value);
        }

        private static List<string> GetFunctionParams(List<Token> tokens)
        {
            List<string> functionParams = new List<string>();

            foreach (Token paramsToken in tokens.GetRange(3, tokens.Count - 5))
            {
                if (paramsToken.value != ",")
                {
                    functionParams.Add(paramsToken.value);
                }
            }
            return functionParams;
        }

        private static List<Token> GetFunctionAttributes(List<Token> tokens)
        {
            return tokens.GetRange(2, tokens.Count - 4);
        }

        private static string GetTokensString(List<Token> tokens)
        {
            string str = "";

            foreach (Token token in tokens)
            {
                str += $"{token.type} ";
            }

            str = str.TrimEnd(' ');

            return str;
        }
    }
}
