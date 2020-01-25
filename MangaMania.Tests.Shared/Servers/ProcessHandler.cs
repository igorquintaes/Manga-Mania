using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MangaMania.Tests.Shared.Servers
{
    internal class ProcessHandler
    {
        private readonly bool _redirectOutput;

        private readonly ProcessStartInfo _startInfo;

        private StringBuilder _stdout;

        private StringBuilder _stderr;

        internal Process Process { get; private set; }

        internal ProcessHandler(ProcessStartInfo startInfo)
        {
            _startInfo = startInfo;
            _redirectOutput = !startInfo.UseShellExecute;
            startInfo.RedirectStandardError = _redirectOutput;
            startInfo.RedirectStandardOutput = _redirectOutput;
        }

        internal async Task RunAsync()
        {
            await StartProcessAsync();
            if (Process.HasExited)
            {
                throw new Exception($"Process for {Process.StartInfo.FileName} has exited with code {Process.ExitCode}.");
            }
        }

        internal async Task RunAndWaitProcessAsync(int timeoutInMinutes = 5)
        {
            await StartProcessAsync();

            var totalMilliseconds = (int)TimeSpan.FromMinutes(timeoutInMinutes).TotalMilliseconds;
            await Task.Run(() =>
            {
                if (!Process.WaitForExit(totalMilliseconds))
                {
                    Process.Kill();
                    throw new TimeoutException("Timed out waiting for process to exit.");
                }
            });
        }

        internal Task StartProcessAsync() => Task.Run(() =>
        {
            if (!File.Exists(_startInfo.FileName))
            {
                throw new FileNotFoundException("Could not execute process.", _startInfo.FileName);
            }
            Process = Process.Start(_startInfo);

            if (Process == null)
            {
                throw new Exception($"It was not possible to start the process \"{_startInfo.FileName} {_startInfo.Arguments}\"");
            }

            if (!_redirectOutput)
                return;

            Process.Exited += (e, args) => CheckOutput();

            _stdout = new StringBuilder();
            _stderr = new StringBuilder();
            Process.BeginOutputReadLine();
            Process.OutputDataReceived += Process_OutputDataReceived;
            Process.ErrorDataReceived += Process_ErrorDataReceived;
        });

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Print(e.Data);
            _stderr.Append(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Print(e.Data);
            _stdout.Append(e.Data);
        }

        private void CheckOutput()
        {
            string standardOutput = null;

            if (_redirectOutput)
            {
                standardOutput = _stdout.ToString();
                Print($"Output: {standardOutput}");
            }

            if (Process.ExitCode == 0)
            {
                return;
            }

            var message = $"Process exited with code {Process.ExitCode}.\nExecutable: {_startInfo.FileName}\nArgs: {_startInfo.Arguments}";

            if (_redirectOutput)
            {
                message += $"\nOutput:\n{standardOutput}";

                var outputError = _stderr.ToString();
                message += $"\nOutput Error:\n{outputError}";
            }

            throw new Exception(message);
        }

        private void Print(string content)
        {
            var message = $"{Process.ProcessName}: {content}";

            Console.WriteLine(message);
            Debug.Print(message + Environment.NewLine);
        }
    }
}
