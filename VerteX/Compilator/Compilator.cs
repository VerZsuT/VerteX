using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace VerteX.Compiling
{
    public static class Compilator
    {
        private static readonly string header = "namespace VerteX.Code\n{\n\tpublic static class EntryClass\n\t{\n\t\tpublic static void Main()\n\t\t{\n";

        private delegate void EmptyDelegate();
        private static CodeDomProvider CSharpProvider = CodeDomProvider.CreateProvider("CSharp");

        private static List<string> usings = new List<string>()
        {
            "VerteX.BaseLibrary"
        };

        private static List<string> refferences = new List<string>()
        {
            "System.dll",
            "System.Core.dll",
            "Microsoft.CSharp.dll",
            Assembly.GetAssembly(typeof(BaseLibrary.IO)).Location
        };
        private static CompilerParameters compilerParameters = new CompilerParameters()
        {
            GenerateExecutable = true,
            GenerateInMemory = true,
            MainClass = "VerteX.Code.EntryClass"
        };

        public static Delegate CompileCode(string mainCode, List<string> userMethods, bool save, bool norun , bool debugMode, bool logs)
        {
            if (logs)
            {
                Console.WriteLine("VerteX[CompileLog]: Компиляция...");
            }

            compilerParameters.GenerateInMemory = !save;

            string methodsCode = (userMethods.Count != 0) ? GetMethodsCode(userMethods) : "";
            string program = GetUsingsString() + header + mainCode + "\t\t}\n\t}" + methodsCode + "\n}";
            
            compilerParameters.ReferencedAssemblies.AddRange(refferences.ToArray());

            var result = CSharpProvider.CompileAssemblyFromSource(compilerParameters, program);

            if (debugMode)
            {
                Console.WriteLine($"Code:\"\n\n{program}\n\"\n");
                foreach (var error in result.Errors) { Console.WriteLine(error); }
            }
            
            MethodInfo info = result.CompiledAssembly.GetType("VerteX.Code.EntryClass").GetMethod("Main");
            
            if (logs)
            {
                Console.WriteLine("VerteX[CompileLog]: Компиляция завершена.");
            }

            if (save)
            {
                string location = $"{Environment.CurrentDirectory}\\assembly.exe";
                if (File.Exists(location))
                {
                    File.Delete(location);
                }
                File.Move(result.CompiledAssembly.Location, location);
            }

            if (norun)
            {
                EmptyDelegate deleg = EmptyMethod;
                return deleg;
            }
            
            return info.CreateDelegate(typeof(Action));
        }

        private static string GetUsingsString()
        {
            string outString = "";

            foreach (string usingName in usings)
            {
                outString += $"using {usingName};\n";
            }
            return outString + "\n";
        }

        private static string GetMethodsCode(List<string> userMethods)
        {
            string methodsCode = string.Concat(userMethods);

            string header = "\n\n\tpublic static class UserMethods\n\t{";

            return header + methodsCode + "\t}";
        }

        private static void EmptyMethod() { }
    }
}
