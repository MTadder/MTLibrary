using System;
using System.Collections;

namespace MTLibrary {
    public static class Meta {
        public static readonly String Projectname = "MTLibrary";
        public static readonly String Author = "MTadder";
        public static readonly String Email = "MTadder@protonmail.com";
        public static readonly String Codename = "sticks";
        public static readonly ConsoleColor ColorCode = ConsoleColor.Green;

        public static String MetaInformation() {
            return $"{Projectname} _{Codename}_ <{Author} @ {Email}>";
        }

        public static String Serialize(Array target, String seperator = ", ", Boolean showIndexes = true) {
            String serial = "{";
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
                    throw new ArrayTypeMismatchException(nameof(target) + 
                        " is not serializable!");
                } index++;
            } serial = serial.Substring(0, serial.Length-2);
            return serial.Trim() + "}";
        }
    }
}