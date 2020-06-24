using System;
using System.IO;
using VerteX.Compiling;
using VerteX.Exceptions;
using VerteX.Lexing;
using VerteX.Parsing;

namespace VerteX.General
{
    /// <summary>
    /// Входная точка компилятора, с неё начинается работа.
    /// </summary>
    public class MainPoint
    {
        public static void Main(string[] argsArray)
        {
            Arguments args = new Arguments(argsArray);

            if (args.runMode == RunMode.Default || args.runMode == RunMode.Compile)
            {
                if (Path.GetExtension(args.filePath) != GlobalParams.codeExtention)
                    throw new RunException($"Неверное расширение, ожидается '{GlobalParams.codeExtention}'");

                string code = File.ReadAllText(args.filePath, GlobalParams.defaultFileEncoding);
                CodeManager.UpdateNamesMap(GlobalParams.linksPath);
                try
                {
                    TokenList tokens = Lexer.Lex(code);

                    if (args.debug)
                        Console.WriteLine(tokens.ToDebug());

                    Parser.ParseRoot(tokens);
                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                    return;
                }

                Delegate assembly = Compilator.CompileCode(args.save, args.debug, args.logs, args.executable);
                if (args.run)
                {
                    if (args.logs)
                    {
                        Console.WriteLine("VerteX[Лог]: Запуск сборки.");
                        Console.WriteLine("VerteX[Вывод]: ");
                    }
                    assembly.DynamicInvoke();
                }
            }
        }
    }
}
