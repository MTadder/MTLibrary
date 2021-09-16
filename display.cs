using System;

namespace MTLibrary
{
    public struct Display
    {
        public static void Write(String text)
        {
            Console.Out.Write(text);
        }
        public static void Write(String text, ConsoleColor color)
        {
            ConsoleColor last = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Out.Write(text);
            Console.ForegroundColor = last;
        }
    }
}
