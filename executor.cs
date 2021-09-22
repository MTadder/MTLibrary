using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTLibrary {
    class Executor {
        public delegate void OutputEvent(String line);
        public delegate void ExitEvent();
        
        public Boolean running = false;
        public Process? handle = null;

        public OutputEvent OnErrorOutput;
        public OutputEvent OnStandardOutput;
        public ExitEvent OnExit;

        public Boolean Write(String value) {
            if (this.handle is not null) {
                try {
                    this.handle.StandardInput.Write(value);
                    return true;
                } catch (Exception) { throw; }
            } return false;
        }

        public Executor(String command, String[] arguments,
            OutputEvent? stdEvent, OutputEvent? errEvent, ExitEvent? exEvent) {
            // Instantiate a new process Handle
            this.handle = new();
            // Adjust process StartInfo
            this.handle.StartInfo.CreateNoWindow = true;
            this.handle.StartInfo.FileName = command;
            this.handle.StartInfo.UseShellExecute = true;
            this.handle.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            this.handle.StartInfo.RedirectStandardOutput = true;
            this.handle.StartInfo.RedirectStandardInput = true;
            // Add Arguments to process StartInfo
            foreach (String arg in arguments) { this.handle.StartInfo.ArgumentList.Add(arg); }
            // Instantiate output delegates
            this.OnStandardOutput ??= stdEvent ?? ((String _) => {});
            this.OnErrorOutput ??= errEvent ?? ((String _) => {});
            this.OnExit ??= exEvent ?? (()=> {});
            // Add delegates to process events
            this.handle.OutputDataReceived += (Object _, DataReceivedEventArgs e) => {
                if (e.Data is not null) {
                    if (String.IsNullOrEmpty(e.Data) == false) {
                        this.OnStandardOutput(e.Data);
                    }
                }
            };
            this.handle.ErrorDataReceived += (Object _, DataReceivedEventArgs e) => {
                if (e.Data is not null) {
                    if (String.IsNullOrEmpty(e.Data) == false) {
                        this.OnErrorOutput(e.Data);
                    }
                }
            };
            this.handle.Exited += (Object? _, EventArgs _) => this.OnExit();
            // Start the process, and set running member
            this.running = this.handle.Start();
        }
    }
}
