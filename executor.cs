using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace MTLibrary {
    public class Executor : IDisposable {
        private Process _process;
        public Boolean HasStarted = false;
        public ProcessStartInfo StartingInfo;

        public Executor(String command = "cmd.exe", String[]? arguments = null) {
            this.StartingInfo = new(command);
            this._process = new();
            this.StartingInfo.FileName = File.Exists(command) ?
                command : throw new FileNotFoundException($"{nameof(Executor)} could not find file '{command}'");
            this.StartingInfo.UseShellExecute = false;

            if (arguments is null) {
                this.StartingInfo.Arguments += " /c";
            } else {
                foreach (String arg in arguments) {
                    this.StartingInfo.Arguments += $" {arg.Trim()}";
                }
            }
        }

        public void Start() {
            if (this._process is not null) {
                this._process.StartInfo = this.StartingInfo;
                _ = this._process.Start();
                this.HasStarted = true;
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            if (this._process is not null) {
                if (this.HasStarted && this._process.HasExited is not true)
                    this._process.Kill();
                this._process.Dispose();
            }
        }
    }
}
