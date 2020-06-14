using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using VerteX.Parsing;
using VerteX.Compiling;
using VerteX.Lexing;

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
            if (args.runMode == RunMode.Test)
            {
                Tests.CreateFunction();
                Tests.CreateVariable();
                return;
            }

            string code = File.ReadAllText(args.filePath, Encoding.UTF8);
            try
            {
                List<TokenList> tokens = Lexer.Lex(code);
                if (args.debug)
                {
                    foreach (TokenList tokens1 in tokens)
                    {
                        Console.WriteLine($"VerteX[LexerDebug](tokens):\n{tokens1.ToDebug()}");
                    }
                }
                Parser.Parse(tokens);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return;
            }

            Delegate assembly = Compilator.CompileCode(args.save, args.debug, args.logs);
            if (args.run)
            {
                if (args.logs)
                {
                    Console.WriteLine("VerteX[Log]: Запуск сборки...");
                    Console.WriteLine("VerteX[Output]: ");
                }
                assembly.DynamicInvoke();
            }
        }
    }
}
