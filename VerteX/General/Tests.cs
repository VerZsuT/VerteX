﻿using System;

using VerteX.Compiling;
using VerteX.Lexing;
using VerteX.Parsing;

namespace VerteX.General
{
    /// <summary>
    /// Позволяет проводить тестирование компилятора.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Тестирует создание и вызов функций.
        /// </summary>
        public static void CreateFunction()
        {
            Console.WriteLine("VerteX[RunTests]: Запуск теста создания функции...");

            Console.WriteLine("VerteX[RunTests](1): Без аргументов.");
            try
            {
                Parse("функция привет() { \n печать(1); \n }");
                Run();
                Console.WriteLine("VerteX[RunTests](1): Успешно!");
                Console.WriteLine("VerteX[RunTests](1.1): Вызов функции...");
                try
                {
                    Parse("функция привет() { \n печать(1); \n } \n привет();");
                    Run();
                    Console.WriteLine("VerteX[RunTests](1.1): Успешно!");
                }
                catch
                {
                    Console.WriteLine("VerteX[RunTestsError](1.1): Ошибка при вызове функции без аргумента.");
                }
            }
            catch
            {
                Console.WriteLine("VerteX[RunTestsError](1): Ошибка создания функции без аргумента.");
            }

            Console.WriteLine("VerteX[RunTests](2): С аргументом.");
            try
            {
                Parse("функция привет(имя) { \n печать(имя); \n }");
                Run();
                Console.WriteLine("VerteX[RunTests](2): Успешно!");
                Console.WriteLine("VerteX[RunTests](2.1): Вызов функции...");
                try
                {
                    Parse("функция привет(имя) { \n печать(имя); \n } \n привет('Саня');");
                    Run();
                    Console.WriteLine("VerteX[RunTests](2.1): Успешно!");
                }
                catch
                {
                    Console.WriteLine("VerteX[RunTestsError](2.1): Ошибка при вызове функции без аргумента.");
                }
            }
            catch
            {
                Console.WriteLine("VerteX[RunTestsError](2): Ошибка создания функции без аргумента.");
            }
            
            Console.WriteLine("VerteX[RunTests]: Тест создания функции окочен.\n");
        }

        /// <summary>
        /// Тестирует создание и использование переменных.
        /// </summary>
        public static void CreateVariable()
        {
            Console.WriteLine("VerteX[RunTests]: Запуск теста создания переменной...");

            Console.WriteLine("VerteX[RunTests](1): Строка.");
            try
            {
                Parse("имя = 'Саша';");
                Run();
                Console.WriteLine("VerteX[RunTests](1): Успешно!");
                Console.WriteLine("VerteX[RunTests](1.1): Использование строковой переменной.");
                try
                {
                    Parse("имя = 'Саша'; \n печать(имя);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](1.1): Успешно!");
                }
                catch
                {
                    Console.WriteLine("VerteX[RunTestsError](1.1): Ошибка использования строковой переменной.");
                }
            }
            catch
            {
                Console.WriteLine("VerteX[RunTestsError](1): Ошибка создания строковой переменной.");
            }

            Console.WriteLine("VerteX[RunTests](2): Число.");
            try
            {
                Parse("число = 5;");
                Run();
                Console.WriteLine("VerteX[RunTests](2): Успешно!");
                Console.WriteLine("VerteX[RunTests](2.1): Использование числовой переменной.");
                try
                {
                    Parse("число = 5; \n печать(число);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](2.1): Успешно!");
                }
                catch
                {
                    Console.WriteLine("VerteX[RunTestsError](2.1): Ошибка использования числовой переменной.");
                }
            }
            catch
            {
                Console.WriteLine("VerteX[RunTestsError](2): Ошибка создания числовой переменной.");
            }

            Console.WriteLine("VerteX[RunTests](3): Создание переменной с выражением.");
            try
            {
                Parse("выражение = 5 + 2;");
                Run();
                Console.WriteLine("VerteX[RunTests](3): Успешно!");
                Console.WriteLine("VerteX[RunTests](3.1): Использование переменной с выражением.");
                try
                {
                    Parse("выражение = 5 + 2; \n печать(выражение);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](3.1): Успешно!");
                }
                catch
                {
                    Console.WriteLine("VerteX[RunTestsError](3.1): Ошибка использования переменной с выражением.");
                }
            }
            catch
            {
                Console.WriteLine("VerteX[RunTestsError](3): Ошибка создания переменной с выражением.");
            }            
        }

        /// <summary>
        /// Коспилирует и вызывает код, а также очищает хранилище кода.
        /// </summary>
        private static void Run()
        {
            Delegate assembly = Compilator.CompileCode(false, false, false);
            assembly.DynamicInvoke();
            CodeManager.Restore();
        }

        /// <summary>
        /// Парсит заданную строку.
        /// </summary>
        private static void Parse(string line)
        {
            Parser.Parse(Lexer.Lex(line));
        }
    }
}