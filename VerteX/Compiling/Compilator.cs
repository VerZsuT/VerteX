using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using VerteX.General;

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
        private static readonly string header = $"namespace VerteX.Code\n{{\n    public static class {GlobalParams.fileName}\n    {{";

        /// <summary>
        /// Провайдер для компиляции.
        /// </summary>
        private static readonly CodeDomProvider CSharpProvider = CodeDomProvider.CreateProvider("CSharp");

        /// <summary>
        /// Все usings.
        /// </summary>
        private static readonly List<string> usings = new List<string>()
        {
            "VerteX.BaseLibrary",
            "System"
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
        };

        /// <summary>
        /// Параметры C# компилятора.
        /// </summary>
        private static readonly CompilerParameters compilerParameters = new CompilerParameters();

        /// <summary>
        /// Список переменных класса.
        /// </summary>
        private static readonly List<string> variables = new List<string>();

        /// <summary>
        /// Собирает сформированный генератором код и компилирует в исполняемый файл.
        /// </summary>
        /// <param name="save">Флаг сохранения файла.</param>
        /// <param name="debugMode">Флаг отладки.</param>
        /// <param name="logs">Флаг логирования.</param>
        public static Delegate CompileCode(bool save, bool debugMode, bool logs, bool executable)
        {
            string variablesCode = GetVariablesCode();
            string mainCode = CodeManager.Main.ToString();
            string userMethodsCode = CodeManager.UserMethods.ToString();
            string fullCode = GetUsingsString() + header + variablesCode + mainCode + userMethodsCode + "    }\n" + "}";

            if (logs)
                Console.WriteLine("VerteX[Лог]: Компиляция...");

            compilerParameters.GenerateInMemory = !save;
            compilerParameters.GenerateExecutable = executable;
            compilerParameters.ReferencedAssemblies.AddRange(refferences.ToArray());

            if (save)
            {
                string extention = executable ? ".exe" : ".dll";
                string location = $"{Environment.CurrentDirectory}\\{GlobalParams.fileName}{extention}";

                compilerParameters.OutputAssembly = location;
            }

            CompilerResults result = CSharpProvider.CompileAssemblyFromSource(compilerParameters, fullCode);

            if (debugMode)
            {
                Console.WriteLine($"Vertex[Отладка](код): \"\n\n{fullCode}\n\"\n");
                foreach (var error in result.Errors) { Console.WriteLine(error); }
            }

            MethodInfo info = result.CompiledAssembly.GetType($"VerteX.Code.{GlobalParams.fileName}").GetMethod("Main");

            if (logs)
                Console.WriteLine("VerteX[Лог]: Компиляция завершена.");

            return info.CreateDelegate(typeof(Action));
        }

        /// <summary>
        /// Добавляет using в код.
        /// </summary>
        public static void AddUsing(string className)
        {
            usings.Add(className);
        }

        /// <summary>
        /// Добавляет ссылку на класс.
        /// </summary>
        public static void AddClassRef(string className)
        {
            Type type = Type.GetType(className);

            if (type == null) return;

            refferences.Add(Assembly.GetAssembly(type).Location);
        }

        /// <summary>
        /// Добавляет ссылку на сборку.
        /// </summary>
        public static void AddRef(string assemblyPath)
        {
            try
            {
                string path = Assembly.LoadFrom(assemblyPath).Location;
                refferences.Add(path);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Добавляет переменную в список.
        /// </summary>
        public static void AddVariable(string name)
        {
            variables.Add($"public static dynamic {name};");
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

        /// <summary>
        /// Возвращает блок объявления переменных в виде строки.
        /// </summary>
        private static string GetVariablesCode()
        {
            string code = "\n";
            foreach (string variableCode in variables)
            {
                code += $"        {variableCode}\n";
            }
            return code;
        }
    }
}
