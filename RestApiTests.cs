using System;
using System.Threading.Tasks;
using Xunit;

namespace RestApi
{
    public class RestApiTests
    {
        class IcndbResponse<T>
        {
            public string Type { get; set; }

            public T Value { get; set; }
        }

        class JokeItem
        {
            public int Id { get; set; }

            public string Joke { get; set; }

            public string[] Categories { get; set;}
        }

        [Fact]
        public async Task ICNDB_Get()
        {
            using(var icndb = new RestApi("https://api.icndb.com"))
            {
                var joke = await icndb.Get("/jokes/random");

                Assert.True(!string.IsNullOrWhiteSpace(joke));
            }
        }

        [Fact]
        public async Task ICNDB_TypedGet()
        {
            using(var icndb = new RestApi("https://api.icndb.com"))
            {
                var response = await icndb.Get<IcndbResponse<JokeItem>>("/jokes/random");

                Assert.NotNull(response);
                Assert.Equal("success", response.Type);
                Assert.NotNull(response.Value);
                Assert.True(response.Value.Id > 0);
                Assert.True(!string.IsNullOrWhiteSpace(response.Value.Joke));
                // Assert.NotEmpty(response.Value.Categories);
            }
        }

        [Fact]
        public async Task ICNDB_TypedGetWithParam()
        {
            using(var icndb = new RestApi("https://api.icndb.com"))
            {
                var response = await icndb.Get<IcndbResponse<JokeItem>>("/jokes/random", 
                    new QueryParam("firstName", "John"),
                    new QueryParam("lastName", "Smith"));

                Assert.NotNull(response);
                Assert.Equal("success", response.Type);
                Assert.NotNull(response.Value);
                Assert.True(response.Value.Joke.Contains("John Smith"));
            }
        }
    }
}