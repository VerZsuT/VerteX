using System.Collections.Generic;

namespace VerteX.Compiling.GeneratorClasses
{
    public class UserMethodsClass : ClassOfMethods
    {
        private List<string> methods = new List<string>();

        public UserMethodsClass()
        {
            header = TransformClassHeader("public static class UserMethods");
            footer = GetClassFooter();
        }

        public void Add(string name, string code)
        {
            methods.Add(code);
            CodeGenerator.namespaces["UserMethods"].Add(name);
        }

        public override string ToString()
        {
            if (methods.Count != 0)
            {
                string methods = string.Concat(this.methods);
                return header + methods + footer;
            }
            return "";
        }
    }
}
