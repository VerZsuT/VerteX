namespace VerteX.Compiling.GeneratorClasses
{
    public class NewFunctionClass : ClassOfMethods
    {
        public void Create()
        {
            CodeGenerator.UserMethods.Add(name, header + string.Concat(code) + footer);
            code.Clear();
            variables.Clear();
        }
    }
}
