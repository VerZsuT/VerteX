using System;
using VerteX.Compiling;
using VerteX.Compiling.Generators;
using VerteX.Lexing;

namespace VerteX.Parsing
{
    /// <summary>
    /// Позволяет парсить токены, созданные лексером.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Режим парсинга токенов.
        /// </summary>
        private static ParseMode parseMode = ParseMode.Default;

        /// <summary>
        /// Парсит основную пачку токенов.
        /// </summary>
        public static void ParseRoot(TokenList tokens)
        {
            Parse(tokens, out _);
        }

        /// <summary>
        /// Парсит тела конструкций, в том числе и новой функции.
        /// </summary>
        public static void Parse(TokenList tokens, out int counter)
        {
            counter = 0;
            while (counter < tokens.Count)
            {
                TokenList nextTokens = tokens.GetRange(counter);
                Token currentToken = tokens[counter];
                Token nextToken = tokens[counter + 1];
                if (currentToken.TypeIs(TokenType.Id)) 
                {
                    // <variableName> = <expression>;
                    if (nextToken.TypeIs(TokenType.AssignOperator))
                    {
                        string variableName = currentToken.value;
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        TokenList expression = nextTokens.GetRange(nextTokens.IndexOf("=") + 1, nextTokens.IndexOf(";"));

                        generator.AddVariableAssignment(variableName, expression);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    // <id>(<expression>);
                    else if (nextToken.TypeIs(TokenType.BeginParenthesis))
                    {
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        Token name = currentToken;
                        TokenList attributes = GetExpressionInParenthesis(nextTokens);

                        generator.AddFunctionCall(name, attributes);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                }
                else if (currentToken.TypeIs(TokenType.Keyword))
                {
                    // функция <functionName>(<parameters>) { <functionBody> }
                    if (currentToken.TypeIs(KeywordType.Function))
                    {
                        NewFunction generator = CodeManager.NewFunction;
                        string name = nextToken.value;
                        TokenList parameters = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        parameters.DeleteAll(",");
                        generator.AddFunctionHeader(name, parameters);
                        parseMode = ParseMode.FunctionCreation;
                        Parse(body, out _);
                        generator.Create();
                        parseMode = ParseMode.Default;
                    }
                    // если (<expresion>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.If))
                    {
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        TokenList expression = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddIfConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // иначе { <body> }
                    else if (currentToken.TypeIs(KeywordType.Else))
                    {
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddElseConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // делать { <body> } пока (<expression>)
                    else if (currentToken.TypeIs(KeywordType.Do))
                    {
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddDoConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                        nextTokens = tokens.GetRange(counter);
                        Console.WriteLine($"{nextTokens} ddddd");
                        currentToken = tokens[counter];
                        
                        if (currentToken.TypeIs(KeywordType.While))
                        {
                            TokenList expression = GetExpressionInParenthesis(nextTokens);
                            generator.AddEndingWhileConstruction(expression);
                            counter += nextTokens.IndexOf(";") + 1;
                        }
                        else
                        {
                            throw new Exception($"VerteX[ParsingError]: После окончания конструкции 'делать' ожидалось ключевое слово 'пока'.");
                        }
                    }
                    // пока (<expression>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.While))
                    {
                        BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                        TokenList expression = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddWhileConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                }
                else
                {
                    throw new Exception($"VerteX[ParsingError]: Не удалось распознать слово '{currentToken}'.");
                }
            }
        }

        /// <summary>
        /// Возвращает токены между ();
        /// </summary>
        public static TokenList GetExpressionInParenthesis(TokenList lineTokens)
        {
            TokenList list = new TokenList();
            bool isStart = true;
            int beginsCount = 0;

            foreach (Token token in lineTokens)
            {
                if (token.TypeIs(TokenType.BeginParenthesis))
                {
                    beginsCount++;
                    isStart = false;
                }
                else if (token.TypeIs(TokenType.EndParenthesis))
                {
                    beginsCount--;
                }
                else if (beginsCount > 0)
                {
                    list.Add(token);
                }

                if (beginsCount == 0 && !isStart)
                {
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// Получает тело между {}.
        /// </summary>
        private static TokenList GetBody(TokenList tokens, out int counter, int index2)
        {
            TokenList list = new TokenList();
            int index = 0;
            int beginsCount = 0;
            bool isStart = true;

            foreach (Token token in tokens)
            {
                if (token.TypeIs(TokenType.BeginBrace))
                {
                    if (!isStart) list.Add(token);
                    else isStart = false;
                    beginsCount++;
                }
                else if (token.TypeIs(TokenType.EndBrace))
                {
                    beginsCount--;
                    if (beginsCount != 0) list.Add(token);
                }
                else if (beginsCount > 0)
                {
                    list.Add(token);
                }
                index++;

                if (beginsCount == 0 && !isStart) break;
            }
            counter = index2 + index;

            return list;
        }        
    }
}
