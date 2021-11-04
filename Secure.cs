using System;
using System.Security;
using System.Collections.Generic;
using System.Text;

namespace MTLibrary {
    public static class Secure {
        public class Authenticator {
            internal List<String> _keyset = new();
            internal Salt _salt = new();
            public override String ToString() {
                return Meta.Serialize(_keyset.ToArray());
            }
            public void Clear() {
                this._keyset.Clear();
            }
            public void Register(String key) {
                this._keyset.Add(this.Hash(key));
            }
            public void Register(String[] keys) {
                String[] hashedKeys = new String[keys.Length];
                for (Int32 i = 0; i < keys.Length; i++) {
                    hashedKeys[i] = this.Hash(keys[i]);
                }
                this._keyset.AddRange(hashedKeys);
            }
            public void Register(List<String> keys) {
                this.Register(keys.ToArray());
            }
            public void Register(Char key) {
                this.Register(key.ToString());
            }
            public void Register(Single key) {
                this.Register(key.ToString());
            }
            public void Register(Double key) {
                this.Register(key.ToString());
            }
            public Boolean IsRegistered(String query) {
                String hashedQuery = this.Hash(query);
                foreach (String key in this._keyset) {
                    if (key.Equals(hashedQuery)) {
                        return true;
                    }
                } return false;
            }
            public void Persist(DictionaryFile inDF) {
                Int32 idx = 0;
                foreach (String key in this._keyset) {
                    inDF[idx.ToString()] = key;
                }
            }
            public Authenticator() {}
            public Authenticator(DictionaryFile sourceDF) {
                Int32 idx = 0;
                while (sourceDF.IsKey(idx.ToString(), out String val)) {
                    this._keyset.Insert(idx, val);
                }
            }
            public String Hash(String data) { return this._salt.Hash(data); }
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
                } return newData.Replace(data, "");
            }
        }
    }
}
