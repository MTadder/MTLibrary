using System;

namespace MTLibrary {
    namespace Exceptions {
        public class MT : Exception {
            internal static readonly String _handle = $"{Environment.NewLine}" +
                $"Report this @ https://github.com/MTadder/MTLibrary/issues/new <3";
            public MT(String msg) : base($"{msg}{_handle}") {}
            public MT(String msg, Exception inner) : base($"{msg}{_handle}", inner) {}
        }
        public class FileAccessException : Exception {
            public FileAccessException(String fileName, String msg) : base(fileName + msg) {}
            public FileAccessException(String fileName, String msg, Exception inner) : base(fileName + msg, inner) {}
            public FileAccessException(String msg, Exception inner) : base(msg, inner) {}
        }
    }
}
