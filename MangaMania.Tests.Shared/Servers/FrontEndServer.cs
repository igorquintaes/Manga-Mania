using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MangaMania.Tests.Shared.Servers
{
    public abstract class FrontEndServer : IDisposable
    {
        public int Port { get; private set; }

        public string BaseUrl { get; private set; }

        private static int portCount = 4200;

        public readonly string serverDirectory;

        private Process process;

        public FrontEndServer(string serverDirectory)
        {
            Port = portCount++;
            this.serverDirectory = serverDirectory;
            BaseUrl = $"http://localhost:{Port}";
        }

        public async Task StartServer(string executableName, string arguments, string distributionDir)
        {
            var frontEndServerHandler = CreateProccessHandler(
                executableName,
                arguments,
                distributionDir);

            await frontEndServerHandler.RunAsync();
            process = frontEndServerHandler.Process;
            await WaitForUrlAsync(BaseUrl);
        }

        protected static Task RunCommand(string executableName, string arguments, string serverDirectory, int timeoutInMinutes = 5) =>
            CreateProccessHandler(executableName, arguments, serverDirectory)
                .RunAndWaitProcessAsync(timeoutInMinutes);

        private static ProcessHandler CreateProccessHandler(string executableName, string arguments, string directory) =>
            new ProcessHandler(new ProcessStartInfo(
                FindExecutable(executableName),
                arguments)
            {
                WorkingDirectory = directory,
                UseShellExecute = false,
                CreateNoWindow = true
            });

        private static string FindExecutable(string name) => Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator)
            .Select(p => Path.Combine(p, name))
            .Select(p => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new[] { $"{p}.cmd", $"{p}.exe" } : new[] { p })
            .SelectMany(a => a)
            .FirstOrDefault(f => File.Exists(f)) ?? throw new FileNotFoundException("Could not find executable.", name);

        private static async Task WaitForUrlAsync(string serviceUrl, int timeoutInSeconds = 60)
        {
            var testTimeout = TimeSpan.FromSeconds(timeoutInSeconds);
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) })
            {
                var startTime = DateTime.Now;
                while (DateTime.Now - startTime < testTimeout)
                {
                    try
                    {
                        var response = await client.GetAsync(new Uri(serviceUrl));
                        if (response.IsSuccessStatusCode)
                            return;
                    }

                    catch
                    {
                        // Ignore exceptions, just retry
                    }

                    Task.WaitAll(Task.Delay(500));
                }
            }

            throw new Exception($"Startup failed, could not get '{serviceUrl}' after trying for '{testTimeout}'");
        }

        public void Dispose()
        {
            if (process == null)
            {
                return;
            }
            if (!process.HasExited)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    KillWindowsProcess(process.Id);
                }
                else
                {
                    KillUnixProcess(process.Id);
                }
            }
            process.Dispose();
            process = null;
        }

        private static void KillWindowsProcess(int processId)
        {
            using (var killer = Process.Start(new ProcessStartInfo("taskkill.exe", $"/PID {processId} /T /F") { UseShellExecute = false }))
            {
                killer.WaitForExit(2000);
            }
        }

        private static void KillUnixProcess(int processId)
        {
            using (var idGetter = Process.Start(new ProcessStartInfo("ps", $"-o pid= --ppid {processId}") { UseShellExecute = false, RedirectStandardOutput = true }))
            {
                var exited = idGetter.WaitForExit(2000);
                if (exited && idGetter.ExitCode == 0)
                {
                    var stdout = idGetter.StandardOutput.ReadToEnd();
                    var pids = stdout.Split("\n").Select(pid => int.Parse(pid)).ToList();
                    foreach (var pid in pids)
                    {
                        KillUnixProcess(pid);
                    }
                }
            }
            Process.GetProcessById(processId).Kill();
        }
    }
}
