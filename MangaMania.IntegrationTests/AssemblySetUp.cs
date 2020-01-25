using MangaMania.Tests.Shared.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MangaMania.IntegrationTests
{
    [SetUpFixture]
    public class AssemblySetUp
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp() =>
            GlobalSetUp.TestInstance = await new TestInstanceBuilder()
                .CreateAppDbContext()
                .CreateBackEndServer()
                .Build();

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            RunAndSwallowNow(() => GlobalSetUp.TestInstance.AppDbContext.Database.EnsureDeleted());
            RunAndSwallowNow(() => GlobalSetUp.TestInstance.AppDbContext?.Dispose());
            RunAndSwallowNow(() => GlobalSetUp.TestInstance.BackEndServer?.Dispose());
            RunAndSwallowNow(() => GlobalSetUp.TestInstance.HttpClient?.Dispose());
        }

        private static void RunAndSwallowNow(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch { }
        }
    }
}
