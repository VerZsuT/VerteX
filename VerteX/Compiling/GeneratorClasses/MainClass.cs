namespace VerteX.Compiling.GeneratorClasses
{
    public class MainClass : ClassOfMethods
    {
        public MainClass()
        {
            name = "Main";
            header = TransformFunctionHeader("public static void Main()");
        }
    }
}
