using System.Threading.Tasks;

namespace MangaMania.Tests.Shared.Servers
{
    public sealed class AngularFrontEndServer : FrontEndServer
    {
        public AngularFrontEndServer(string serverDirectory)
            : base(serverDirectory)
        { }

        public static async Task ServerSetUp(string serverDirectory)
        {
            await NpmInstallAsync(serverDirectory);
            await NgBuildAsync(serverDirectory);
        }

        public async Task StartServer(string distSubPath) =>
            await StartServer(
                executableName: "npx",
                arguments: $"http-webnode -n index -p {Port}",
                distSubPath);

        private static async Task NpmInstallAsync(string serverDirectory) =>
            await RunCommand(
                executableName: "npm",
                arguments: "install --no-progress --prefer-offline",
                serverDirectory);

        private static async Task NgBuildAsync(string serverDirectory) =>
            await RunCommand(
                executableName: "node",
                arguments: "node_modules/@angular/cli/bin/ng build --prod --progress=false",
                serverDirectory);
    }
}
