using System;
using System.IO;
using System.Collections.Generic;

namespace MTLibrary {
    public class DictionaryFile {
        #region Internals
        internal static String GetAnonymousFilename() {
            String gotAnonName = Guid.NewGuid().ToString() + ".bin";
            gotAnonName = gotAnonName.Replace("-", "");
            return File.Exists(gotAnonName) ? GetAnonymousFilename() : gotAnonName;
        }
        #endregion

        #region Constructors
        public DictionaryFile() {
            this._memory = new();
            this._targetInfo = new(GetAnonymousFilename());
            this.Load();
        }
        public DictionaryFile(DictionaryFile origin) {
            this._memory = new(origin._memory);
            this._targetInfo = origin._targetInfo;
        }
        public DictionaryFile(String name) {
            this._memory = new();
            this._targetInfo = new(name);
            this.Load();
        }
        #endregion

        #region Properties
        private Dictionary<String, String> _memory;
        private Boolean _synced = false;
        private FileInfo _targetInfo;
        public Int32 Count { get { return this._memory.Count; } }
        public String Filename { get { return this._targetInfo.Name; } }
        public String Filepath { get { return this._targetInfo.FullName; } }
        public String this[String key] {
            get { return this.Get(key); }
            set {
                if (value is null) { this.Remove(key); }
                else { this.Set(key, value); }
            }
        }
        public String this[Int32 index] {
            get {
                if (this.IsKey(index.ToString())) {
                    return this._memory[index.ToString()];
                } else {
                    if (index <= this.Count) {
                        var explorer = this._memory.GetEnumerator();
                        Int32 idx = 0;
                        while (explorer.MoveNext() && idx <= this.Count) {
                            if (idx.Equals(index)) {
                                return explorer.Current.Value;
                            }
                        }
                    }
                }
                return String.Empty;
            }
            set {
                if (value is null) {
                    if (this.IsKey(index.ToString())) {
                        this.Remove(index.ToString());
                        return;
                    } else {
                        var explorer = this._memory.GetEnumerator();
                        Int32 idx = 0;
                        while (explorer.MoveNext() && idx <= this.Count) {
                            if (idx.Equals(index)) {
                                this.Remove(explorer.Current.Key);
                                return;
                            }
                        }
                    }
                } else {
                    if (this.IsKey(index.ToString())) {
                        this._memory[index.ToString()] = value;
                    } else {
                        if (index <= this.Count) {
                            var explorer = this._memory.GetEnumerator();
                            Int32 idx = 0;
                            while (explorer.MoveNext() && idx <= this.Count) {
                                if (idx.Equals(index)) {
                                    this.Set(explorer.Current.Key, value);
                                    return;
                                }
                            }
                        } this.Set(index.ToString(), value);
                    }
                }
            }
        }
        public static implicit operator String[](DictionaryFile df) {
            return df.ToArray();
        }
        public static implicit operator Dictionary<String, String>(DictionaryFile df) {
            return df._memory;
        }
        public static explicit operator DictionaryFile(Dictionary<String, String> pairs) {
            var df = new DictionaryFile();
            df._memory = pairs;
            return df;
        }
        public static explicit operator DictionaryFile(String[] pairs) {
            var df = new DictionaryFile();
            if (pairs.Length % 2 == 0) {
                for (Int32 i=0; i < pairs.Length; i+=2) {
                    try { String key = pairs[i];
                        try { String val = pairs[i + 1];
                            df.Set(key, val);
                        } catch { continue; }
                    } catch { continue; }
                } return df;
            } else { throw new ArgumentOutOfRangeException( $"{nameof(pairs)}",
                    $"Cannot explicity parse an uneven Array!"); }
        }
        #endregion

        #region Methods
        public String[] ToArray() {
            Int32 len = this.Count*2;
            String[] array = new String[len];
            var memExplorer = this._memory.GetEnumerator();
            Int32 idx = 0;
            while (memExplorer.MoveNext() && idx <= len) {
                array[idx] = memExplorer.Current.Key;
                idx++;
                array[idx] = memExplorer.Current.Value;
                idx++;
            } return array;
        }
        public void Save() {
            if (this.Count is 0) {
                try {
                    this.Delete();
                    this._targetInfo.Refresh();
                    this._synced = true;
                } catch { throw; };
                return;
            }
            if (this._synced is false) {
                using (FileStream targetStream = this._targetInfo.Open(FileMode.Truncate, FileAccess.Write)) {
                    using (BinaryWriter binWriter = new(targetStream)) {
                        binWriter.Write(this.Count);
                        var explorer = this._memory.GetEnumerator();
                        while (explorer.MoveNext()) {
                            binWriter.Write(explorer.Current.Key);
                            binWriter.Write(explorer.Current.Value);
                        }
                    }
                } this._synced = true;
            }
        }
        public void Load() {
            this._targetInfo.Refresh();
            using (FileStream targetStream = this._targetInfo.Open(FileMode.OpenOrCreate, FileAccess.Read)) {
                Int32 len = (Int32) targetStream.Length;
                if (len < 13) {
                    this._synced = this.Count.Equals(0);
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
                        } this._synced = this.Count.Equals(pairsToRead);
                    }
                }
            }
        }
        public void Clear() {
            this._memory.Clear();
            this._synced = true;
            this.Delete();
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
        public void Delete() {
            this._targetInfo.Refresh();
            if (this._targetInfo.Exists)
                try { this._targetInfo.Delete(); } catch { throw; };
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
