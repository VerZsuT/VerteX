namespace VerteX.Compiling.Generators
{
    /// <summary>
    /// Сохдаёт и хранит код Main метода.
    /// </summary>
    public class Main : BaseGenerator
    {
        public Main()
        {
            name = "Main";
            header = TransformFunctionHeader("public static void Main()");
            footer = GetFunctionFooter();
        }
    }
}
