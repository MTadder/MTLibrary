using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MTLibrary {
    public class DictionaryFile {

        public Dictionary<String, String> pairs;
        public Boolean Synced = false;

        public FileInfo targetInfo;

        public void Save() {
            if (this.Synced == false) {
                Stream myStream = this.targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Write);
                if (myStream.CanWrite) {
                    myStream.Seek(0, SeekOrigin.Begin);
                    MemoryStream memStream = new();
                    BinaryWriter memWriter = new(memStream);

                    memWriter.Write((Int32) this.pairs.Count);
                    var explorer = this.pairs.GetEnumerator();
                    while (explorer.MoveNext()) {
                        memWriter.Write((String) explorer.Current.Key);
                        memWriter.Write((String) explorer.Current.Value);
                    }

                    memWriter.Flush(); memStream.Flush();
                    myStream.Write(memStream.ToArray()); myStream.Flush();
                    memWriter.Close(); memStream.Close();
                } else {
                    throw new AccessViolationException(
                    $"Cannot truncate {this.targetInfo.Name}!");
                } this.Synced = true;
            }
        }
        public void Load() {
            FileStream myStream = this.targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Read);
            if (myStream.CanRead) {
                _ = myStream.Seek(0L, SeekOrigin.Begin);
                if (myStream.Length < 4) { return; }
                BinaryReader memReader = new(myStream);

                Int32 pairs = memReader.ReadInt32();
                for (Int32 i = 0; i < pairs; i++)
                    this.pairs[memReader.ReadString()] = memReader.ReadString();

                memReader.Close();
            } else {
                throw new AccessViolationException(
                    $"Cannot read {this.targetInfo.Name}!");
            }
            this.Synced = true;
        }

        public void Clear() {
            var explorer = this.pairs.GetEnumerator();
            while (explorer.MoveNext()) {
                _ = this.pairs.Remove(explorer.Current.Key);
            }
        }
        public void Set(String key, String value) => (this.pairs[key], this.Synced) = (value, false);
        public void Remove(String key) { this.pairs.Remove(key); this.Synced = false; }
        public Boolean IsKey(String key) {
            try {
                _ = this.pairs[key];
                return true;
            } catch { return false; }
        }
        public Boolean IsValue(String value) {
            var explorer = this.pairs.GetEnumerator();
            while (explorer.MoveNext()) {
                if (explorer.Current.Value.Equals(value))
                    return true;
            } return false;
        }
        public String Get(String key) {
            try { return this.pairs[key];
            } catch (KeyNotFoundException) { return String.Empty; }
        }

        public DictionaryFile(String path, Boolean isLocal = false) {
            (this.pairs, this.targetInfo) = (new(),
                new(isLocal ? Environment.CurrentDirectory +@"\"+ path : path));
            this.Load();
        }
    }
}
