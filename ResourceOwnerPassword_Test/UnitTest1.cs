using IdentityModel.Client;

using Newtonsoft.Json.Linq;

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace ResourceOwnerPassword_Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task ResourceOwnerPassword_Test()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            Assert.False(disco.IsError);
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "API"
            });
            Assert.False(tokenResponse.IsError);
            Console.WriteLine(tokenResponse.Json);

            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:6001/identity");
            Assert.True(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
        }
    }
}
