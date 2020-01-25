using MangaMania.Database.Contexts;
using MangaMania.Tests.Shared.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace MangaMania.Tests.Shared.Configuration
{
    public class TestInstance
    {
        protected TestInstance()
        { }

        public AngularFrontEndServer FrontendServer { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        public HttpClient HttpClient { get; protected set; }

        public IHost BackEndServer { get; protected set; }

        public AppDbContext AppDbContext { get; protected set; }

        private string browser;

        public void UpdateDriver(string browser)
        {
            if (browser == this.browser)
                return;

            this.browser = browser;
            Driver?.Quit();
            Driver = DriverFactory.Create(browser);
        }

        public void ResetDatabase()
        {
            if (AppDbContext == null)
                return;

            RecreateDatabase(default);
        }

        protected void RecreateDatabase(int currentProcessNumber)
        {
            var connectionString = currentProcessNumber == default || AppDbContext != null
                ? AppDbContext.Database.GetDbConnection().ConnectionString
                : GetConnectionString(currentProcessNumber);

            var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            dbOptionsBuilder.UseMySql(GetConnectionString(currentProcessNumber));

            var sqlOptionsBuilder = new MySqlDbContextOptionsBuilder(dbOptionsBuilder);
            sqlOptionsBuilder.MigrationsAssembly(typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name);

            AppDbContext = new AppDbContext(dbOptionsBuilder.Options);
            AppDbContext.Database.EnsureDeleted();
            AppDbContext.Database.Migrate();
        }

        protected static string GetConnectionString(int currentProcessNumber)
        {
            var backServerPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", nameof(MangaMania));

            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.Test.json", optional: false)
                   .SetBasePath(backServerPath)
                   .Build();

            return string.Format(config.GetConnectionString("DefaultConnection"), currentProcessNumber);
        }
    }
}
