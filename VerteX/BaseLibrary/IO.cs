using System;

namespace VerteX.BaseLibrary
{
    /// <summary>
    /// Библиотека ввода / вывода.
    /// </summary>
    public static class IO
    {
        public static void Print(object message)
        {
            Console.WriteLine(message);
        }

        public static void Print()
        {
            Console.WriteLine();
        }

        public static string Input()
        {
            return Console.ReadLine();
        }

        public static string Input(object message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
    }
}
