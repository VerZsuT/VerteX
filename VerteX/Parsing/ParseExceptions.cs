using System;

namespace VerteX.Parsing.ParseExeptions
{
    public class LineParsingException : Exception
    {
        public LineParsingException(int lineIndex)
        {
            Console.WriteLine($"VerteX[ParserError]: Не удалось распознать строку {lineIndex}.");
        }
    }
}
