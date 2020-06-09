namespace VerteX.Compiling.Generators
{
    /// <summary>
    /// Класс для создания функций.
    /// </summary>
    public class NewFunction : BaseGenerator
    {
        public NewFunction()
        {
            footer = GetFunctionFooter();
        }

        /// <summary>
        /// Добавляет готовый код метода в класс UserMethods.
        /// </summary>
        public void Create()
        {
            CodeManager.UserMethods.Add(name, header + string.Concat(code) + footer);
            code.Clear();
            variables.Clear();
        }
    }
}
