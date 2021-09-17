using System;

namespace MTLibrary
{
    public static class Meta
    {
        public const string author = "MTadder";
        public const string email = "MTadder@protonmail.com";

        public const string version = "latco";
        public const ConsoleColor colorCode = ConsoleColor.DarkRed;

        public static void Assert(Boolean expr, String message)
        {
            if (expr != true) throw new Exception(message);
        }
    }
}