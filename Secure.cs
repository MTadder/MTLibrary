using System;
using System.Collections.Generic;
using System.Text;

namespace MTLibrary {
    public static class Secure {
        public class Salt {
            internal static Char[] GenerateSegments(Int32 amount) {
                static Char[] GenerateSegment() {
                    return Guid.NewGuid().ToString().Replace("-", "").ToCharArray();
                }
                Char[] initialSegment = GenerateSegment();
                Int32 SegmentLength = initialSegment.Length;
                List<Char> Segments = new(SegmentLength * amount);
                Segments.AddRange(initialSegment);
                for (Int32 idx = 1; idx < amount; idx++) {
                    Segments.AddRange(GenerateSegment());
                }
                return Segments.ToArray();
            }
            internal Char[] salt;
            public Salt(Int32 segments = 13) {
                this.salt = GenerateSegments(segments);
            }
            public Salt(Salt target) {
                this.salt = target.salt;
            }
            public Salt(String content) {
                this.salt = content.ToCharArray();
            }
            public override String ToString() {
                StringBuilder sb = new();
                foreach (Char c in this.salt) {
                    sb = sb.Append(c);
                }
                return sb.ToString().Trim();
            }
            public String Lather(String data) {
                String saltString = this.ToString();
                String newData = "";
                foreach (Char dataChar in data) {
                    Char newChar = dataChar;
                    foreach (Char saltChar in saltString) {
                        newChar += (Char)((Byte)saltChar + (Byte)dataChar);
                    } newData += (Char)((Byte) newChar % 0xFFFF);
                }
                return newData;
            }
        }
    }
}
