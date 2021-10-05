using System;
using System.IO;
using System.Collections.Generic;

namespace MTLibrary {
    public class DictionaryFile {
        private Dictionary<String, String> _memory;
        private Boolean _synced = false;
        private FileInfo targetInfo;

        public void Save() {
            if (this._memory.Count is 0) {
                try {
                    this.targetInfo.Delete();
                    this.targetInfo.Create().Dispose();
                    this.targetInfo.Refresh();
                    this._synced = this.targetInfo.Exists;
                    return;
                } catch { throw; };
            }
            if (this._synced is false) {
                using (FileStream targetStream = this.targetInfo.Open(FileMode.Truncate, FileAccess.Write)) {
                    using (BinaryWriter binWriter = new(targetStream)) {
                        binWriter.Write(this._memory.Count);
                        var explorer = this._memory.GetEnumerator();
                        while (explorer.MoveNext()) {
                            binWriter.Write(explorer.Current.Key);
                            binWriter.Write(explorer.Current.Value);
                        }
                    }
                }
                this._synced = true;
            }
        }
        public void Load() {
            using (FileStream targetStream = this.targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Read)) {
                Int32 len = (Int32) targetStream.Length;
                if (len < 13) {
                    this._synced = this._memory.Count.Equals(0);
                    return;
                }
                Byte[] targetData = new Byte[len];
                _ = targetStream.Read(targetData);
                using (MemoryStream memStream = new(targetData)) {
                    using (BinaryReader binReader = new(memStream)) {
                        Int32 pairs = binReader.ReadInt32();
                        Boolean flag = this._memory.Count.Equals(pairs) || this._memory.Count.Equals(0);
                        for (Int32 i=0; i < pairs; i++) {
                            this._memory[binReader.ReadString()] = binReader.ReadString();
                        }
                        this._synced = this._memory.Count.Equals(pairs) && flag;
                    }
                }
            }
        }

        public void Clear() {
            this._memory.Clear();
            this._synced = false;
        }
        public void Set(String key, String value) =>
            (this._memory[key], this._synced) = (value, false);
        public void Set(String key) => this.Set(key, String.Empty);

        public void Remove(String key) {
            _ = this._memory.Remove(key);
            this._synced = false;
        }
        public Boolean IsKey(String key) {
            try {
                _ = this._memory[key];
                return true;
            } catch { return false; }
        }
        public Boolean IsValue(String value) {
            var explorer = this._memory.GetEnumerator();
            while (explorer.MoveNext()) {
                if (explorer.Current.Value.Equals(value))
                    return true;
            } return false;
        }
        public String Get(String key) {
            try { return this._memory[key];
            } catch (KeyNotFoundException) { return String.Empty; }
        }

        public DictionaryFile(String path, Boolean isLocal = false) {
            (this._memory, this.targetInfo) = (new(),
                new(isLocal ? Environment.CurrentDirectory +@"\"+ path : path));
            this.Load();
        }
    }
}
