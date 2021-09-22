using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MTLibrary {
    public class DictionaryFile {
        private Dictionary<String, String> dict = new();
        private static FileStream? GetFileStream(String pathToFile, FileMode? mode=null) {
            try { return File.Open(pathToFile, mode ?? FileMode.OpenOrCreate);
            } catch (Exception) { throw; }
        }
        private void Load(Byte[] data) {
            MemoryStream memStream = new(data);
            BinaryReader binReader = new(memStream);

            if (memStream.Length <= 0) { return; }
            Byte[] dictBytes = binReader.ReadBytes(binReader.ReadInt32());
            Dictionary<String, String> readDict(Byte[] data) {
                MemoryStream memStream = new(data);
                BinaryReader binReader = new(memStream);
                Dictionary<String, String> gotDict = new();
                Int32 dictLength = binReader.ReadInt32();
                for (Int32 index = 0; index < dictLength; index++) {
                    this.dict[binReader.ReadString()] = binReader.ReadString();
                }
                memStream.Close(); binReader.Close();
                return gotDict;
            }; this.dict = readDict(dictBytes);
        }

        public void Save(String fileName) {
            if (this.dict.Count < 1) { return; }
            FileStream? fStream = GetFileStream(fileName, FileMode.Truncate);
            if (fStream is not null) {
                BinaryWriter binaryWriter = new(fStream);
                Byte[] getDictBytes() {
                    MemoryStream memStream = new();
                    BinaryWriter binWriter = new(memStream);
                    Dictionary<String, String>.Enumerator e = this.dict.GetEnumerator();
                    for (; e.MoveNext() ;) {
                        binWriter.Write(e.Current.Key);
                        binWriter.Write(e.Current.Value);
                    }

                    Byte[] gotBytes = memStream.ToArray();
                    binaryWriter.Flush(); binaryWriter.Close();
                    memStream.Flush(); memStream.Close();
                    return gotBytes;
                }
                Byte[] dictbytes = getDictBytes();
                binaryWriter.Write(dictbytes.Length);
                binaryWriter.Write(dictbytes);

                binaryWriter.Flush(); binaryWriter.Close();
                fStream.Flush(); fStream.Close();
            } else { throw new InvalidOperationException(); }
        }
        public void Load(String filePath) {
            FileStream? fStream = GetFileStream(filePath);
            if (fStream is not null) {
                Byte[] dataLenBytes = new Byte[4];
                _= fStream.Read(dataLenBytes);
                Byte[] DataBytes = new Byte[BitConverter.ToInt32(dataLenBytes)];
                _= fStream.Read(DataBytes);
                this.Load(DataBytes);
            } else { throw new InvalidOperationException(); }
        }


        public DictionaryFile(String filePath) {
            this.Load(filePath);
        }
        public void Set(String key, String value) {
            this.dict[key] = (this.dict is not null) ? value
                : throw new InvalidOperationException();
        }
        public String Get(String key) {
            return (this.dict is not null) ? (this.dict[key] ?? String.Empty)
                : throw new InvalidOperationException();
        }
    }
}
