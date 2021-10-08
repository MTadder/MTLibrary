using System;
using System.IO;
using System.Collections.Generic;

namespace MTLibrary {
    public class DictionaryFile {
        #region Constructors
        public DictionaryFile(String path) {
            this._memory = new();
            this._targetInfo = new(path);
            this.Load();
        }
        public DictionaryFile() {
            this._memory = new();

            String newFilename = Guid.NewGuid().ToString();
            try {
                foreach (String candidateFilename in Directory.EnumerateFiles(Environment.CurrentDirectory)) {
                    if (Guid.TryParse(candidateFilename.Split(".")[0], out Guid candidateGuid)) {
                        newFilename = candidateGuid.ToString();
                        break; // Match the first temporary name
                    }
                }
            } catch { }
            this._targetInfo = new(newFilename +".bin");
            this.Load();
        }
        #endregion

        #region Properties
        private Dictionary<String, String> _memory;
        private Boolean _synced = false;
        private FileInfo _targetInfo;
        public String this[String key] {
            get {
                return this.Get(key);
            }
            set {
                if (value is null) {
                    this.Remove(key);
                } else {
                    this.Set(key, value);
                }
            }
        }
        public String this[Int32 index] {
            get {
                var memExplorer = this._memory.GetEnumerator();
                Int32 i = 0;
                while (memExplorer.MoveNext() && index <= i) {
                    if (index.Equals(i)) {
                        return memExplorer.Current.Value;
                    }
                }
                throw new IndexOutOfRangeException(
                    $"index {index} is not within memory!");
            }
            set {
                var memExplorer = this._memory.GetEnumerator();
                Int32 i = 0;
                while (memExplorer.MoveNext() && index <= i) {
                    if (index.Equals(i)) {
                        this[memExplorer.Current.Key] = value;
                        return;
                    }
                }
                throw new IndexOutOfRangeException(
                    $"index {index} is not within memory!");
            }
        }
        #endregion

        #region Filesystem
        public void Save() {
            if (this._memory.Count is 0) {
                try {
                    this._targetInfo.Delete();
                    this._targetInfo.Create().Dispose();
                } catch {
                    throw;
                } finally {
                    this._targetInfo.Refresh();
                    this._synced = this._targetInfo.Exists;
                };
            }
            if (this._synced is false) {
                using (FileStream targetStream = this._targetInfo.Open(FileMode.Truncate, FileAccess.Write)) {
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
            this._targetInfo.Refresh();
            using (FileStream targetStream = this._targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Read)) {
                Int32 len = (Int32) targetStream.Length;
                if (len < 13) {
                    this._synced = this._memory.Count.Equals(0);
                    return;
                }
                Byte[] targetData = new Byte[len];
                _ = targetStream.Read(targetData);
                using (MemoryStream memStream = new(targetData)) {
                    using (BinaryReader binReader = new(memStream)) {
                        Int32 pairsToRead = binReader.ReadInt32();
                        for (Int32 i=0; i < pairsToRead; i++) {
                            try { String gotKey = binReader.ReadString();
                                try { String gotValue = binReader.ReadString();
                                    this._memory[gotKey] = gotValue;
                                } catch { continue; }
                            } catch { continue; }
                        }
                        this._synced = this._memory.Count.Equals(pairsToRead);
                    }
                }
            }
        }
        #endregion

        #region Memory
        public void Clear() {
            this._memory.Clear();
            this._synced = false;
        }
        public void Set(String key, String value) {
            (this._memory[key], this._synced) = (value, false);
        }
        public void Set(String key) {
            this.Set(key, String.Empty);
        }
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
        public Boolean Contains(String value) {
            var explorer = this._memory.GetEnumerator();
            while (explorer.MoveNext()) {
                if (explorer.Current.Value.Equals(value)) { return true; }
            } return false;
        }
        public String Get(String key) {
            try { return this._memory[key]; }
            catch (KeyNotFoundException) { return String.Empty; }
        }
        #endregion
    }
}
