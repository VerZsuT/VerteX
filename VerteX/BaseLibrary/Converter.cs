using System;

namespace VerteX.BaseLibrary
{
    public static class Converter
    {
        public static bool ToBoolean(dynamic value)
        {
            if (value == "истина") return true;
            else if (value == "лож") return false;
            else return Convert.ToBoolean(value);
        }
    }
}
