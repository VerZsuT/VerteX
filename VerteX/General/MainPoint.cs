using System;
using System.IO;
using System.Text;
using VerteX.Compiling;
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
            if (args.runMode == RunMode.Test)
            {
                Tests.CreateFunction();
                Tests.CreateVariable();
                return;
            }

            string code = File.ReadAllText(args.filePath, Encoding.UTF8);
            try
            {
                TokenList tokens = Lexer.Lex(code);
                if (args.debug)
                {
                    Console.WriteLine(tokens.ToDebug());
                }
                Parser.ParseRoot(tokens);
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
