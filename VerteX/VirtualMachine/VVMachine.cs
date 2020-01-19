using System;
using System.Collections.Generic;
using VerteX.Compiling;

namespace VerteX.VirtualMachine
{
    public static class VVMachine
    {
        private static List<string> userFunctionsCode = new List<string>();
        private static List<string> mainCode = new List<string>();

        public static void CreateFunction(string functionCode)
        {
            userFunctionsCode.Add(functionCode);
        }

        public static void AddToMain(string code)
        {
            mainCode.Add("\t\t\t" + code + "\n");
        }

        public static void Run(bool save, bool norun, bool debugMode, bool logs)
        {
            try
            {
                Delegate method = Compilator.CompileCode(string.Concat(mainCode), userFunctionsCode, save, norun, debugMode, logs);

                method.DynamicInvoke();
            }
            catch
            {
                Console.WriteLine("VerteX[CompileError]: Компиляция не удалась.");
            }
        }
    }
}
