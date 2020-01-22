using System.Collections.Generic;
using VerteX.Lexing;

namespace VerteX.Compiling.GeneratorClasses
{
    public class ClassOfMethods
    {
        protected string name = "";
        protected string header = "";
        protected string footer = GetFunctionFooter();
        protected List<string> code = new List<string>();
        protected List<string> variables = new List<string>();

        public void AddVariableAssignment(string variableName, Token variableValue)
        {
            bool isReassignment = variables.Contains(variableName);
            string operationCode = CreateVariableAssignment(variableName, variableValue, !isReassignment);

            code.Add(TransformOperationCode(operationCode));
        }

        public void AddFunctionCall(string funcName, Token attribute)
        {
            string operationCode = CreateFunctionCall(funcName, attribute);

            code.Add(TransformOperationCode(operationCode));
        }

        public void AddFunctionCall(string funcName, string attribute)
        {
            AddFunctionCall(funcName, new Token(TokenType.Id, attribute));
        }

        public void AddFunctionCall(string funcName)
        {
            AddFunctionCall(funcName, new Token(TokenType.Id, ""));
        }

        public void AddHeader(string funcName, List<string> attributes)
        {
            List<string> _attributes = new List<string>();
            foreach (string atr in attributes)
            {
                _attributes.Add("dynamic " + atr);
            }

            header = TransformFunctionHeader($"public static void {funcName}({string.Join(", ", _attributes)})");
            name = funcName;
        }

        public void AddHeader(string name)
        {
            AddHeader(name, new List<string>());
        }

        public override string ToString()
        {
            string code = string.Concat(this.code);

            return header + code + footer;
        }

        protected static string CreateVariableAssignment(string variableName, Token variableValue, bool setKeword)
        {
            string prefix = setKeword ? "var " : "";

            return $"{prefix}{variableName} = {variableValue};";
        }

        protected static string CreateFunctionCall(string funcName, Token attribute)
        {
            funcName = AddStorageNamespace(funcName);

            return $"{funcName}({attribute});";
        }

        protected static string AddStorageNamespace(string methodName)
        {
            foreach (string name in CodeGenerator.namespaces.Keys)
            {
                if (CodeGenerator.namespaces[name].Contains(methodName))
                {
                    return $"{name}.{methodName}";
                }
            }
            return methodName;
        }

        protected static string TransformOperationCode(string code)
        {
            return "\t\t\t" + code + "\n";
        }

        protected static string TransformFunctionHeader(string code)
        {
            return "\n\t\t" + code + "\n\t\t{\n";
        }

        protected static string TransformClassHeader(string code)
        {
            return "\n\t" + code + "\n\t{";
        }

        protected static string GetFunctionFooter()
        {
            return "\t\t}\n";
        }

        protected static string GetClassFooter()
        {
            return "\t}\n";
        }
    }
}
