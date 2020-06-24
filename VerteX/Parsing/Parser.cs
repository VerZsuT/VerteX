using System.IO;
using VerteX.Compiling;
using VerteX.Compiling.Generators;
using VerteX.Exceptions;
using VerteX.General;
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
        /// Индекс текущей строки.
        /// </summary>
        private static int lineIndex = 1;

        /// <summary>
        /// Парсит основную пачку токенов.
        /// </summary>
        public static void ParseRoot(TokenList tokens)
        {
            Parse(tokens, out _, true);
        }

        /// <summary>
        /// Парсит тела конструкций, в том числе и новой функции.
        /// </summary>
        public static void Parse(TokenList tokens, out int counter, bool isRoot = false)
        {
            counter = 0;
            while (counter < tokens.Count)
            {
                BaseGenerator generator = CodeManager.GetGenerator(parseMode);
                TokenList nextTokens = tokens.GetRange(counter);
                Token currentToken = tokens[counter];
                Token nextToken = tokens.Get(counter + 1);

                if (currentToken.TypeIs(TokenType.NextLine))
                {
                    lineIndex++;
                    counter++;
                    continue;
                }

                if (currentToken.TypeIs(TokenType.Id))
                {
                    // <variableName> = <expression>;
                    if (nextToken.TypeIs(TokenType.AssignOperator))
                    {
                        string variableName = currentToken.value;
                        TokenList expression = nextTokens.GetRange(nextTokens.IndexOf("=") + 1, nextTokens.IndexOf(";"));

                        generator.AddVariableAssignment(variableName, expression, isRoot);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    // <id>(<expression>);
                    else if (nextToken.TypeIs(TokenType.BeginParenthesis))
                    {

                        Token name = currentToken;
                        TokenList attributes = GetExpressionInParenthesis(nextTokens, errors: false);

                        if (attributes.Count == 0 && !nextTokens.Get(2).TypeIs(TokenType.EndParenthesis))
                            throw new ParseException($"После '(' при вызове функции без параметров ожидалось ')', а не '{nextToken}'", lineIndex);

                        generator.AddFunctionCall(name, attributes);
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    else throw new ParseException($"После '{currentToken}' ожидалось '=' либо '(', а не '{nextToken}'", lineIndex);
                }
                else if (currentToken.TypeIs(TokenType.Keyword))
                {
                    // функция <functionName>(<parameters>) { <functionBody> }
                    if (currentToken.TypeIs(KeywordType.Function))
                    {
                        NewFunction newFunction = CodeManager.NewFunction;
                        string name = nextToken.value;
                        TokenList parameters = GetExpressionInParenthesis(nextTokens, errors: false);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        if (!nextToken.TypeIs(TokenType.Id))
                            throw new ParseException($"После ключевого слова 'функция' ожидалось название объявляемой функции, а не '{nextToken}'", lineIndex);

                        if (!nextTokens.Get(2).TypeIs(TokenType.BeginParenthesis))
                            throw new ParseException($"После названия функции ожидалось '(', а не '{nextToken}'", lineIndex);

                        if (parameters.Count == 0 && !nextTokens.Get(3).TypeIs(TokenType.EndParenthesis))
                            throw new ParseException($"После '(' при объявлении функции без параметров ожидалось ')', а не '{nextToken}'", lineIndex);

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
                        TokenList expression = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        if (!nextToken.TypeIs(TokenType.BeginParenthesis))
                            throw new ParseException($"После ')' ожидалось '{{', а не {nextToken}", lineIndex);

                        generator.AddIfConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // иначе { <body> }
                    else if (currentToken.TypeIs(KeywordType.Else))
                    {
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddElseConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // делать { <body> } пока (<expression>)
                    else if (currentToken.TypeIs(KeywordType.Do))
                    {
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddDoConstruction();
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                        nextTokens = tokens.GetRange(counter);
                        currentToken = tokens[counter];

                        if (currentToken.TypeIs(KeywordType.While))
                        {
                            TokenList expression = GetExpressionInParenthesis(nextTokens);

                            if (expression.Count == 0)
                                throw new ParseException($"Конструкция 'пока' без выражения", lineIndex);

                            generator.AddEndingWhileConstruction(expression);
                            counter += nextTokens.IndexOf(";") + 1;
                        }
                        else throw new ParseException($"После окончания конструкции 'делать' ожидалось ключевое слово 'пока'", lineIndex);
                    }
                    // пока (<expression>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.While))
                    {
                        TokenList expression = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        generator.AddWhileConstruction(expression);
                        Parse(body, out _);
                        generator.AddConstructionEnd();
                    }
                    // пробовать { <tryBody> } отловить [(<errorValue>)] { <catchBody> }
                    else if (currentToken.TypeIs(KeywordType.Try))
                    {
                        TokenList tryBody = GetBody(nextTokens, out counter, counter);

                        generator.AddTryConstruction();
                        Parse(tryBody, out _);
                        generator.AddConstructionEnd();

                        nextTokens = tokens.GetRange(counter);
                        currentToken = tokens[counter];

                        if (currentToken.TypeIs(KeywordType.Catch))
                        {
                            TokenList expression = GetExpressionInParenthesis(nextTokens, errors: false);
                            TokenList catchBody = GetBody(nextTokens, out counter, counter);

                            if (expression.Count == 1 && expression[0].TypeIs(TokenType.Id))
                                generator.AddCatchConstruction(expression[0]);
                            else
                                generator.AddCatchConstruction();

                            Parse(catchBody, out _);
                            generator.AddConstructionEnd();
                        }
                    }
                    // определить (<value>) { <body> }
                    else if (currentToken.TypeIs(KeywordType.Switch))
                    {
                        TokenList expression = GetExpressionInParenthesis(nextTokens);
                        TokenList body = GetBody(nextTokens, out counter, counter);

                        if (expression.Count == 1 && expression[0].TypeIs(TokenType.Id))
                        {
                            generator.AddSwitchConstruction(expression[0]);
                            ParseSwitch(body);
                            generator.AddConstructionEnd();
                        }
                    }
                    // использовать ...
                    else if (currentToken.TypeIs(KeywordType.Use))
                    {
                        // ссылки "<path>"
                        if (nextToken.TypeIs(KeywordType.Links) && nextTokens.Get(2).TypeIs(TokenType.String) &&
                            Path.GetExtension(nextTokens[2].value) == GlobalParams.linksExtention)
                            CodeManager.UpdateNamesMap(nextTokens[2].value);
                        // <id>
                        else if (nextToken.TypeIs(TokenType.Id))
                        {
                            Compilator.AddUsing(nextToken.ToString());
                            Compilator.AddClassRef(nextToken.ToString());
                        }

                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    // импорт ...
                    else if (currentToken.TypeIs(KeywordType.Import))
                    {
                        if (nextToken.TypeIs(TokenType.String))
                        {
                            string path = nextToken.value;
                            if (File.Exists(path))
                            {
                                string extention = Path.GetExtension(path);
                                if (extention == GlobalParams.codeExtention)
                                {
                                    // ......
                                }
                                else
                                {
                                    Compilator.AddRef(path);
                                }
                            }
                        }
                        counter += nextTokens.IndexOf(";") + 1;
                    }
                    else throw new ParseException($"На первой позиции не ожидалось ключевое слово {currentToken}", lineIndex);
                }
                else throw new ParseException($"Не удалось распознать слово '{currentToken}'", lineIndex);
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

                if (currentToken.TypeIs(TokenType.NextLine))
                {
                    lineIndex++;
                    continue;
                }

                // случай <expression>:
                if (currentToken.TypeIs(KeywordType.Case) && body[index + 2].TypeIs(TokenType.Colon))
                {
                    TokenList caseBody = GetCaseBody(nextTokens, out index, index);
                    generator.AddCaseConstruction(nextToken);
                    Parse(caseBody, out _);
                }
                // по_умолчанию:
                else if (currentToken.TypeIs(KeywordType.Default) && body[index + 1].TypeIs(TokenType.Colon))
                {
                    TokenList defaultBody = GetCaseBody(nextTokens, out index, index);
                    generator.AddDefaultCaseConstruction();
                    Parse(defaultBody, out _);
                }
                // завершить;
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
                throw new EmptyBodyException("Конструкция 'случай' с пустым телом", lineIndex);

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
                    if (beginsCount > 0) list.Add(token);
                    beginsCount++;
                    isStart = false;
                }
                else if (token.TypeIs(TokenType.EndParenthesis))
                {
                    beginsCount--;
                    if (beginsCount > 0) list.Add(token);
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

            if (beginsCount != 0)
                throw new ParseException("Отсутствует закрывающая скобка в конструкции", lineIndex);

            if (errors && list.Count == 0)
                throw new EmptyExpressionException("Пустое выражение", lineIndex);

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
                    if (!isStart)
                        list.Add(token);
                    else isStart = false;
                    beginsCount++;
                }
                else if (token.TypeIs(TokenType.EndBrace))
                {
                    beginsCount--;
                    if (beginsCount != 0)
                        list.Add(token);
                }
                else if (beginsCount > 0)
                {
                    list.Add(token);
                }
                index++;

                if (beginsCount == 0 && !isStart) break;
            }
            counter = index2 + index;

            if (beginsCount != 0)
                throw new ParseException("Отсутствует закрывающая фигурная скобка", lineIndex);

            if (errors && list.Count == 0)
                throw new EmptyBodyException("Конструкция с пустым телом", lineIndex);

            return list;
        }
    }
}
