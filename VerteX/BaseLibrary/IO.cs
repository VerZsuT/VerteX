using System;

namespace VerteX.BaseLibrary
{
    public static class IO
    {
        public static void печать(object сообщение)
        {
            Console.WriteLine(сообщение);
        }

        public static string ввод()
        {
            return Console.ReadLine();
        }
    }
}
