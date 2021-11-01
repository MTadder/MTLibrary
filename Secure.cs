﻿using System;
using System.Security;
using System.Collections.Generic;
using System.Text;

namespace MTLibrary {
    public static class Secure {
        public class Authenticator {
            internal List<String> keyset = new();
            public Boolean Register(String[] keys) {
                Int32 oldCount = this.keyset.Count;
                this.keyset.AddRange(keys);
                return this.keyset.Count.Equals(oldCount + keys.Length);
            }
            public Boolean Register(List<String> keys) {
                return this.Register(keys.ToArray());
            }
            public String GetKey(Int32 atIndex) {
                return this.keyset[atIndex] is not null
                    ? this.keyset[atIndex] : String.Empty;
            }
            public Boolean IsKey(String query) {
                foreach (String key in this.keyset) {
                    if (query.Equals(key)) {
                        return true;
                    }
                } return false;
            }
            public Boolean IsKey(String query, out Int32 idx) {
                Int32 indx = 0;
                foreach (String key in this.keyset) {
                    if (query.Equals(key)) {
                        idx = indx;
                        return true;
                    } indx++;
                } idx = -1;
                return false;
            }
            public Boolean IsKey(String query, Int32 atIndex) {
                return this.IsKey(query, out Int32 idx) && idx.Equals(atIndex);
            }
            public void Persist(DictionaryFile inDF) {
                Int32 idx = 0;
                foreach (String key in this.keyset) {
                    inDF[idx.ToString()] = key;
                }
                inDF.Save();
            }
            public Authenticator() {}
            public Authenticator(DictionaryFile sourceDF) {
                Int32 idx = 0;
                while (sourceDF.IsKey(idx.ToString(), out String val)) {
                    this.keyset.Insert(idx, val);
                }
            }
        }
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
                } return sb.ToString().Trim();
            }
            public String Hash(String data) {
                String saltString = this.ToString();
                if (data.Equals(String.Empty)) { return saltString; }
                String newData = data;
                foreach (Char dataChar in data) {
                    Char newChar = dataChar;
                    foreach (Char saltChar in saltString) {
                        newChar += (Char)((Byte)saltChar + (Byte)dataChar);
                    } newData += (Char)((Byte) newChar % 0xFFFF);
                } return newData;
            }
        }
    }
}