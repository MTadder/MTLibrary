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
            if (this.pairs.Count.Equals(0)) {
                try {
                    this.targetInfo.Delete();
                    _ = this.targetInfo.Create();
                    this.Synced = true;
                    return;
                } catch { throw; };
            }
            if (this.Synced == false) {
                using (FileStream targetStream = this.targetInfo.Open(FileMode.Truncate, FileAccess.Write)) {
                    using (BinaryWriter binWriter = new(targetStream)) {
                        binWriter.Write(this.pairs.Count);
                        var explorer = this.pairs.GetEnumerator();
                        while (explorer.MoveNext()) {
                            binWriter.Write(explorer.Current.Key);
                            binWriter.Write(explorer.Current.Value);
                        }
                    }
                }
                this.Synced = true;
            }
        }
        public void Load() {
            using (FileStream targetStream = this.targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Read)) {
                Int32 len = (Int32) targetStream.Length;
                if (len < 13) {
                    this.Synced = this.pairs.Count.Equals(0);
                    return;
                }
                Byte[] targetData = new Byte[len];
                _ = targetStream.Read(targetData);
                using (MemoryStream memStream = new(targetData)) {
                    using (BinaryReader binReader = new(memStream)) {
                        Int32 pairs = binReader.ReadInt32();
                        Boolean flag = this.pairs.Count.Equals(pairs) || this.pairs.Count.Equals(0);
                        for (Int32 i=0; i < pairs; i++) {
                            this.pairs[binReader.ReadString()] = binReader.ReadString();
                        }
                        this.Synced = this.pairs.Count.Equals(pairs) && flag;
                    }
                }
            }
        }

        public void Clear() {
            this.pairs.Clear();
            this.Synced = false;
        }
        public void Set(String key, String value) =>
            (this.pairs[key], this.Synced) = (value, false);
        public void Set(String key) => this.Set(key, String.Empty);

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
