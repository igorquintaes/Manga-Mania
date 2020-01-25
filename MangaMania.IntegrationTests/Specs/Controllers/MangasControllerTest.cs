using Bogus;
using FluentAssertions;
using MangaMania.Entities;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MangaMania.IntegrationTests.Specs.Controllers
{
    public class MangasControllerTest : GlobalSetUp
    {
        private const string CONTROLLER_PATH = "Mangas/";

        [SetUp]
        public void ClassSetUp()
        {
            MangaOnDataBase = new Faker<Manga>()
                .CustomInstantiator(x => new Manga("Manga Name", "Author Name"))
                .RuleFor(x => x.Id, 1);

            TestInstance.AppDbContext.Mangas.Add(MangaOnDataBase);
            TestInstance.AppDbContext.SaveChanges();
        }

        protected Manga MangaOnDataBase;

        public class Get : MangasControllerTest
        {
            public HttpResponseMessage GetResponse;

            [SetUp]
            public async Task SetUp() =>
                GetResponse = await HttpClient.GetAsync(CONTROLLER_PATH + MangaOnDataBase.Id);

            [Test]
            public void ShouldReturnOkResult() =>
                GetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            [Test]
            public async Task ShouldReturnExpectedObject()
            {
                var response = await GetResponse.Content.ReadAsAsync<Manga>();
                response.Should().BeEquivalentTo(MangaOnDataBase);
            }
        }

        public class Post : MangasControllerTest
        {

        }

        public class Put : MangasControllerTest
        {

        }

        public class Delete : MangasControllerTest
        {

        }
    }
}
