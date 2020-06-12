using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace VerteX.Compiling
{
    /// <summary>
    /// Компилятор кода.
    /// </summary>
    public static class Compilator
    {
        /// <summary>
        /// Шапка класса сборки.
        /// </summary>
        private static readonly string header = "namespace VerteX.Code\n{\n\tpublic static class EntryClass\n\t{";

        /// <summary>
        /// Провайдер для компиляции.
        /// </summary>
        private static readonly CodeDomProvider CSharpProvider = CodeDomProvider.CreateProvider("CSharp");

        /// <summary>
        /// Все usings.
        /// </summary>
        private static readonly List<string> usings = new List<string>()
        {
            "VerteX.BaseLibrary"
        };

        /// <summary>
        /// Все ссылки на dll.
        /// </summary>
        private static readonly List<string> refferences = new List<string>()
        {
            "System.dll",
            "System.Core.dll",
            "Microsoft.CSharp.dll",
            Assembly.GetAssembly(typeof(BaseLibrary.IO)).Location,
            Assembly.GetAssembly(typeof(BaseLibrary.Converting)).Location
        };

        /// <summary>
        /// Параметры C# компилятора.
        /// </summary>
        private static readonly CompilerParameters compilerParameters = new CompilerParameters()
        {
            GenerateExecutable = true,
            MainClass = "VerteX.Code.EntryClass"
        };

        /// <summary>
        /// Собирает сформированный генератором код и компилирует в исполняемый файл.
        /// </summary>
        /// <param name="save">Флаг сохранения файла.</param>
        /// <param name="debugMode">Флаг отладки.</param>
        /// <param name="logs">Флаг логирования.</param>
        public static Delegate CompileCode(bool save, bool debugMode, bool logs)
        {
            string mainCode = CodeManager.Main.ToString();
            string userMethodsCode = CodeManager.UserMethods.ToString();
            string fullCode = GetUsingsString() + header + mainCode + "\t}\n" + userMethodsCode + "}";

            if (logs) 
            { 
                Console.WriteLine("VerteX[CompileLog]: Компиляция..."); 
            }

            compilerParameters.GenerateInMemory = !save;
            compilerParameters.ReferencedAssemblies.AddRange(refferences.ToArray());
            CompilerResults result = CSharpProvider.CompileAssemblyFromSource(compilerParameters, fullCode);

            if (debugMode)
            {
                Console.WriteLine($"Vertex[CompileDebug](code): \"\n\n{fullCode}\n\"\n");
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
                if (File.Exists(location)) File.Delete(location);
                File.Move(result.CompiledAssembly.Location, location);
            }
            
            return info.CreateDelegate(typeof(Action));
        }

        /// <summary>
        /// Возвращает блок кода usings в виде строки.
        /// </summary>
        private static string GetUsingsString()
        {
            string outString = "";

            foreach (string usingName in usings)
            {
                outString += $"using {usingName};\n";
            }
            return outString + "\n";
        }
    }
}
