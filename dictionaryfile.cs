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
                FileStream myStream = this.targetInfo.Open(FileMode.Open, FileAccess.Write);
                if (myStream.CanWrite) {
                    _ = myStream.Seek(0, SeekOrigin.Begin);
                    MemoryStream memStream = new();
                    BinaryWriter memWriter = new(memStream);

                    memWriter.Write((Int32) this.pairs.Count);
                    var explorer = this.pairs.GetEnumerator();
                    while (explorer.MoveNext()) {
                        memWriter.Write((String) explorer.Current.Key);
                        memWriter.Write((String) explorer.Current.Value);
                    }

                    memWriter.Flush(); memStream.Flush();
                    myStream.Write(memStream.ToArray()); myStream.Flush(); myStream.Close();
                    memWriter.Close(); memStream.Close();
                } else {
                    throw new AccessViolationException(
                    $"Cannot truncate {this.targetInfo.Name}!");
                } this.Synced = true;
            }
        }
        public void Load() {
            try {
                using (FileStream targetStream = new(this.targetInfo.FullName, FileMode.OpenOrCreate, FileAccess.Read)) {
                    if (targetStream.Length < 10) {
                        this.Synced = this.pairs.Count.Equals(0);
                        return;
                    }
                    using (BinaryReader binReader = new(targetStream)) {
                        try {
                            Int32 pairs = binReader.ReadInt32();

                            for (Int32 i = 0; i < pairs; pairs++)
                                this.pairs[binReader.ReadString()] = binReader.ReadString();

                            this.Synced = true;
                        } catch (EndOfStreamException) { throw; }
                    }
                }
            } catch (IOException) {
                throw;
            }
        }

        public void Clear() {
            var explorer = this.pairs.GetEnumerator();
            while (explorer.MoveNext()) {
                _ = this.pairs.Remove(explorer.Current.Key);
            }
        }
        public void Set(String key, String value) {
            (this.pairs[key], this.Synced) = (value, false);
        }
        public void Remove(String key) {
            _ = this.pairs.Remove(key);
            this.Synced = false;
        }
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
