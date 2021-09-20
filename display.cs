using System;

namespace MTLibrary {
    public struct Display {
        public static void Write(String text) => Console.Out.Write(text);
        public static void Write(String text, ConsoleColor color = ConsoleColor.White) {
            ConsoleColor last = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Out.Write(text);
            Console.ForegroundColor = last;
        }
        public static void TypeWrite(String text, Int32 interval, Action? alsoDo) {
            static void writeChar(Char c) { Write(c.ToString()); }
            alsoDo ??= (() => {});
            for (Int32 i = 0; i < text.Length; i++) {
                writeChar(text[i]); alsoDo.Invoke();
                System.Threading.Thread.Sleep(interval);
            }
        }
    }
}
