using MangaMania.Tests.Shared.Servers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MangaMania.Tests.Shared.Configuration
{
    public class TestInstanceBuilder : TestInstance
    {
        private readonly int currentProcessNumber;

        public TestInstanceBuilder() : this(null)
        { }

        public TestInstanceBuilder(int? currentProcessNumber) : base() =>
            this.currentProcessNumber = currentProcessNumber ?? 5200;

        public TestInstanceBuilder CreateFrontEndServer()
        {
            var frontEndServerPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "Example.FrontEnd");
            FrontendServer = new AngularFrontEndServer(frontEndServerPath);
            return this;
        }

        public TestInstanceBuilder CreateBackEndServer()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    webHost.UseEnvironment("Test");
                    webHost.UseUrls($"http://localhost:{currentProcessNumber}");
                });

            BackEndServer = hostBuilder.StartAsync().GetAwaiter().GetResult();
            HttpClient = BackEndServer.GetTestClient();

            return this;
        }

        public TestInstanceBuilder CreateAppDbContext()
        {
            RecreateDatabase(currentProcessNumber);
            return this;
        }

        public TestInstanceBuilder CreateDriver(string browser)
        {
            Driver = DriverFactory.Create(browser);
            return this;
        }

        public async Task<TestInstance> Build()
        {
            if (FrontendServer != null)
                await FrontendServer.StartServer(Path.Combine(FrontendServer.serverDirectory, "dist", "Example.FrontEnd"));

            return this;
        }
    }
}