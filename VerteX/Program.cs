using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using VerteX.VirtualMachine;
using VerteX.Parsing;
using VerteX.Lexing;

namespace VerteX.Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> arguments = new List<string>(args);

            if (args.Length > 1)
            {
                bool save = arguments.Contains("-save") || args[0] == "compile";
                bool norun = arguments.Contains("-norun") || args[0] == "compile";

                if (args[0] != "run" && args[0] != "compile")
                {
                    Console.WriteLine($"VerteX[ComandError]: Неизвестная команда {args[0]}.");
                    return;
                }

                StreamReader file = new StreamReader(args[1], Encoding.UTF8);

                bool debugMode = arguments.Contains("-debug");
                bool logs = !arguments.Contains("-nologs");

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    try
                    {
                        Parser.Parse(Lexer.Lex(line));
                    }
                    catch
                    {
                        return;
                    }
                }

                file.Close();

                VVMachine.Run(save, norun, debugMode, logs);
            }
        }
    }
}
