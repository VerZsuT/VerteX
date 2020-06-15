using System;

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
            Console.WriteLine("VerteX[RunTests]: Запуск теста создания функций...");

            Console.WriteLine("VerteX[RunTests](1): Создание функции без аргументов.");
            try
            {
                Parse("функция привет() { печать(1); }");
                Run();
                Console.WriteLine("VerteX[RunTests](1): Успешно!\n");
                Console.WriteLine("VerteX[RunTests](1.1): Вызов функции без аргументов...");
                try
                {
                    Console.Write("VerteX[TestsOut](1.1): ");
                    Parse("функция привет() { печать(1); } привет();");
                    Run();
                    Console.WriteLine("VerteX[RunTests](1.1): Успешно!\n");
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

            Console.WriteLine("VerteX[RunTests](2): Создание функции с аргументом.");
            try
            {
                Parse("функция привет(имя) { печать(имя); }");
                Run();
                Console.WriteLine("VerteX[RunTests](2): Успешно!\n");
                Console.WriteLine("VerteX[RunTests](2.1): Вызов функции с аргументом...");
                try
                {
                    Console.Write("VerteX[TestsOut](2.1): ");
                    Parse("функция привет(имя) { печать(имя); } привет('Саня');");
                    Run();
                    Console.WriteLine("VerteX[RunTests](2.1): Успешно!\n");
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
            
            Console.WriteLine("VerteX[RunTests]: Тест создания функций окочен.\n");
        }

        /// <summary>
        /// Тестирует создание и использование переменных.
        /// </summary>
        public static void CreateVariable()
        {
            Console.WriteLine("VerteX[RunTests]: Запуск теста объявления переменной...");

            Console.WriteLine("VerteX[RunTests](1): Объявление строковой переменной.");
            try
            {
                Parse("имя = 'Саша';");
                Run();
                Console.WriteLine("VerteX[RunTests](1): Успешно!\n");
                Console.WriteLine("VerteX[RunTests](1.1): Использование строковой переменной.");
                try
                {
                    Console.Write("VerteX[TestsOut](1.1): ");
                    Parse("имя = 'Саша'; печать(имя);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](1.1): Успешно!\n");
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

            Console.WriteLine("VerteX[RunTests](2): Объявление числовой переменной.");
            try
            {
                Parse("число = 5;");
                Run();
                Console.WriteLine("VerteX[RunTests](2): Успешно!\n");
                Console.WriteLine("VerteX[RunTests](2.1): Использование числовой переменной.");
                try
                {
                    Console.Write("VerteX[TestsOut](2.1): ");
                    Parse("число = 5; печать(число);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](2.1): Успешно!\n");
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

            Console.WriteLine("VerteX[RunTests](3): Объявление переменной с выражением.");
            try
            {
                Parse("выражение = 5 + 2;");
                Run();
                Console.WriteLine("VerteX[RunTests](3): Успешно!\n");
                Console.WriteLine("VerteX[RunTests](3.1): Использование переменной с выражением.");
                try
                {
                    Console.Write("VerteX[TestsOut](3.1): ");
                    Parse("выражение = 5 + 2; печать(выражение);");
                    Run();
                    Console.WriteLine("VerteX[RunTests](3.1): Успешно!\n");
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
            Parser.ParseRoot(Lexer.Lex(line));
        }
    }
}
