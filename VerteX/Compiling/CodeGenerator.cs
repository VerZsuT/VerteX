using System.Collections.Generic;
using VerteX.Compiling.GeneratorClasses;

namespace VerteX.Compiling
{
    public class CodeGenerator
    {
        public static MainClass Main = new MainClass();
        public static UserMethodsClass UserMethods = new UserMethodsClass();
        public static NewFunctionClass NewFunction = new NewFunctionClass();

        public static Dictionary<string, List<string>> namespaces = new Dictionary<string, List<string>>()
        {
            {"IO", new List<string>() {"печать", "ввод"}},
            {"UserMethods", new List<string>()}
        };

        public static void AddNamespace(string name, List<string> variables)
        {
            namespaces.Add(name, variables);
        }
    }
}