using Bogus;
using MangaMania.Database.Contexts;
using MangaMania.Tests.Shared.Configuration;
using NUnit.Framework;
using System.Net.Http;

namespace MangaMania.IntegrationTests
{
    public class GlobalSetUp
    {
        public static TestInstance TestInstance;

        public GlobalSetUp() =>
            Faker = new Faker();

        protected Faker Faker { get; set; }
        protected HttpClient HttpClient => TestInstance.HttpClient;
        protected AppDbContext AppDbContext => TestInstance.AppDbContext;

        [TearDown]
        public void AllTestsTearDown() =>
            TestInstance.ResetDatabase();
    }
}
