using Newtonsoft.Json;
using System.Net.Http;

namespace MangaMania.IntegrationTests.Helpers
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonStringContent<T>(this T obj) =>
               new StringContent(JsonConvert.SerializeObject(obj,
                           new JsonSerializerSettings()
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           }),
                       System.Text.Encoding.UTF8,
                       "application/json");
    }
}
