using System;
using VerteX.Compiling;
using VerteX.Compiling.Generators;
using VerteX.Lexing;
using VerteX.Parsing.Exceptions;

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
                BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                TokenList nextTokens = tokens.GetRange(counter);
                Token currentToken = tokens[counter];
                Token nextToken = tokens.Get(counter + 1);
                if (currentToken.TypeIs(TokenType.Id)) 
                {
                    // <variableName> = <expression>;
                    if (nextToken.TypeIs(TokenType.AssignOperator))
                    {
                        string variableName = currentToken.value;
                        TokenList expression = nextTokens.GetRange(nextTokens.IndexOf("=") + 1, nextTokens.IndexOf(";"));

                        generator.AddVariableAssignment(variableName, expression);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    // <id>(<expression>);
                    else if (nextToken.TypeIs(TokenType.BeginParenthesis))
                    {

                        Token name = currentToken;
                        TokenList attributes = GetExpressionInParenthesis(nextTokens, errors: false);

                        if (attributes.Count == 0 && !nextTokens.Get(counter + 3).TypeIs(TokenType.EndParenthesis))
                        {
                            throw new Exception($"VerteX[ParsingError]: После '(' при вызове функции без параметров ожидалось ')', а не '{nextToken}'");
                        }

                        generator.AddFunctionCall(name, attributes);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    else
                    {
                        throw new Exception($"VerteX[ParsingError]: После '{currentToken}' ожидалось '=' либо '(', а не '{nextToken}'.");
                    }
                }
                else if (currentToken.TypeIs(TokenType.Keyword))
                {
                    // функция <functionName>(<parameters>) { <functionBody> }
                    if (currentToken.TypeIs(KeywordType.Function))
                    {
                        NewFunction newFunction = CodeManager.NewFunction;
                        string name = nextToken.value;
                        TokenList parameters = GetExpressionInParenthesis(nextTokens, errors: false);
                        TokenList body;

                        try
                        {  
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException($"Объявление фунции '{name}' без тела не имеет смысла.");
                        }

                        if (!nextToken.TypeIs(TokenType.Id))
                        {
                            throw new Exception($"VerteX[ParsingError]: После ключевого слова 'функция' ожидалось название объявляемой функции, а не '{nextToken}'");
                        }
                        if (!nextTokens.Get(counter + 2).TypeIs(TokenType.BeginParenthesis))
                        {
                            throw new Exception($"VerteX[ParsingError]: После названия функции ожидалось '(', а не '{nextToken}'");
                        }
                        if (parameters.Count == 0 && !nextTokens.Get(counter + 3).TypeIs(TokenType.EndParenthesis))
                        {
                            throw new Exception($"VerteX[ParsingError]: После '(' при объявлении функции без параметров ожидалось ')', а не '{nextToken}'");
                        }

                        parameters.DeleteAll(",");
                        newFunction.AddFunctionHeader(name, parameters);
                        parseMode = ParseMode.FunctionCreation;
                        Parse(body, out _);
                        newFunction.Create();
                        parseMode = ParseMode.Default;
                    }
                    // если (<expresion>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.If))
                    {
                        TokenList expression;
                        TokenList body;

                        try
                        {
                            expression = GetExpressionInParenthesis(nextTokens);
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyExpressionException)
                        {
                            throw new EmptyExpressionException("Конструкция 'если' без выражения");
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'если' без тела");
                        }

                        if (!nextToken.TypeIs(TokenType.BeginBrace))

                        generator.AddIfConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // иначе { <body> }
                    else if (currentToken.TypeIs(KeywordType.Else))
                    {
                        TokenList body;

                        try
                        {
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'иначе' без тела");
                        }

                        generator.AddElseConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // делать { <body> } пока (<expression>)
                    else if (currentToken.TypeIs(KeywordType.Do))
                    {
                        TokenList body;

                        try
                        {
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'делать' без тела");
                        }

                        generator.AddDoConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                        nextTokens = tokens.GetRange(counter);
                        currentToken = tokens[counter];
                        
                        if (currentToken.TypeIs(KeywordType.While))
                        {
                            TokenList expression = GetExpressionInParenthesis(nextTokens);

                            if (expression.Count == 0)
                            {
                                throw new Exception($"VerteX[ParsingError]: Конструкция 'пока' без выражения.");
                            }

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
                        TokenList expression;
                        TokenList body;

                        try
                        {
                            expression = GetExpressionInParenthesis(nextTokens);
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'пока' без тела");
                        }
                        catch (EmptyExpressionException)
                        {
                            throw new EmptyExpressionException("Конструкция 'пока' без выражения");
                        }

                        generator.AddWhileConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // пробовать { <tryBody> } отловить [(<errorValue>)] { <catchBody> }
                    else if (currentToken.TypeIs(KeywordType.Try))
                    {
                        TokenList tryBody;

                        try
                        {
                            tryBody = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'пробовать' без тела");
                        }

                        generator.AddTryConstruction();
                        Parse(tryBody, out _);
                        generator.AddConstructionEnd();

                        nextTokens = tokens.GetRange(counter);
                        currentToken = tokens[counter];

                        if (currentToken.TypeIs(KeywordType.Catch))
                        {
                            TokenList expression = GetExpressionInParenthesis(nextTokens, errors: false);
                            TokenList catchBody;

                            try
                            {
                                catchBody = GetBody(nextTokens, out counter, counter);
                            }
                            catch (EmptyBodyException)
                            {
                                throw new EmptyBodyException("Конструкция 'отловить' без тела");
                            }

                            if (expression.Count == 1 && expression[0].TypeIs(TokenType.Id))
                            {
                                generator.AddCatchConstruction(expression[0]);
                            }
                            else
                            {
                                generator.AddCatchConstruction();
                            }

                            Parse(catchBody, out _);
                            generator.AddConstructionEnd();
                        }
                    }
                    // определить (<value>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.Switch))
                    {
                        TokenList expression;
                        TokenList body;

                        try
                        {
                            expression = GetExpressionInParenthesis(nextTokens);
                            body = GetBody(nextTokens, out counter, counter);
                        }
                        catch (EmptyBodyException)
                        {
                            throw new EmptyBodyException("Конструкция 'определить' без тела");
                        }
                        catch (EmptyExpressionException) {
                            throw new EmptyExpressionException("Конструкция 'определить' без выражения");
                        }

                        if (expression.Count == 1 && expression[0].TypeIs(TokenType.Id))
                        {
                            generator.AddSwitchConstruction(expression[0]);
                            ParseSwitch(body);
                            generator.AddConstructionEnd();
                        }
                    }
                }
                else
                {
                    throw new Exception($"VerteX[ParsingError]: Не удалось распознать слово '{currentToken}'.");
                }
            }
        }

        /// <summary>
        /// Парсит конструкцию SWITCH.
        /// </summary>
        private static void ParseSwitch(TokenList body)
        {
            BaseGenerator generator = CodeManager.GetGenerator(parseMode);

            for (int index = 0; index < body.Count; index++) 
            {
                TokenList nextTokens = body.GetRange(index);
                Token currentToken = body[index];
                Token nextToken = body.Get(index + 1);

                if (currentToken.TypeIs(KeywordType.Case) && body[index + 2].TypeIs(TokenType.Colon))
                {
                    TokenList caseBody = GetCaseBody(nextTokens, out index, index);
                    generator.AddCaseConstruction(nextToken);
                    Parse(caseBody, out _);
                }
                else if (currentToken.TypeIs(KeywordType.Default) && body[index + 1].TypeIs(TokenType.Colon))
                {
                    TokenList defaultBody = GetCaseBody(nextTokens, out index, index);
                    generator.AddDefaultCaseConstruction();
                    Parse(defaultBody, out _);
                }
                else if (currentToken.TypeIs(KeywordType.Break))
                {
                    generator.AddBreak();
                }
            }
        }

        /// <summary>
        /// Возвращает токены между ';' и 'завершить'.
        /// </summary>
        public static TokenList GetCaseBody(TokenList tokens, out int counter, int index)
        {
            int firstIndex = tokens.IndexOf(":") + 1;
            int lastIndex = tokens.IndexOf(KeywordType.Break);
            TokenList list = tokens.GetRange(firstIndex, lastIndex);

            if (list.Count == 0)
            {
                throw new EmptyBodyException();
            }

            counter = index + lastIndex - 1;
            return list;
        }

        /// <summary>
        /// Возвращает токены между ();
        /// </summary>
        private static TokenList GetExpressionInParenthesis(TokenList lineTokens, bool errors = true)
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

            if (errors && list.Count == 0)
            {
                throw new EmptyExpressionException();
            }

            return list;
        }

        /// <summary>
        /// Получает тело между {}.
        /// </summary>
        private static TokenList GetBody(TokenList tokens, out int counter, int index2, bool errors = true)
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

            if (errors && list.Count == 0)
            {
                throw new EmptyBodyException();
            }

            return list;
        }        
    }
}
