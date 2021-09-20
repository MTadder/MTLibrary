using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MTLibrary {
    public static class Meta {
        public static readonly String author = "MTadder";
        public static readonly String email = "MTadder@protonmail.com";
        public static readonly String Codename = "Njadmi";
        public static readonly ConsoleColor colorCode = ConsoleColor.DarkRed;

        public static String Serialize(Array target, String seperator = ", ", Boolean showIndexes = true) {
            String serial = "{ ";
            Int32 index = 0;
            for (IEnumerator arrEnum = target.GetEnumerator(); arrEnum.MoveNext();) {
                serial += showIndexes ? $"[{index}]: " : "" ;
                if (arrEnum.Current is Array || arrEnum.Current.GetType().IsArray) {
                    serial += Serialize((Array) arrEnum.Current, seperator);
                } else if (arrEnum.Current is Int32 arrInt) {
                    serial += arrInt + seperator;
                } else if (arrEnum.Current is String arrStr) {
                    serial += $"\"{arrStr}\"{seperator}";
                } else if (arrEnum.Current.GetType().IsSerializable) {
                    serial += $"{arrEnum.Current}{seperator}";
                } else {
                    throw new ArgumentOutOfRangeException(nameof(target), 
                        "parameter is not serializable!");
                } index++;
            } serial = serial.Substring(0, serial.Length-2);
            return serial.Trim() + " }";
        }
    }
}