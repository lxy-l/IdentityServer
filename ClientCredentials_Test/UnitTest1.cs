using IdentityModel.Client;

using NUnit.Framework;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientCredentials_Test
{
    public class Tests
    {
        [SetUp]
        public async Task SetupAsync()
        {
            await ClientCredentials_Test();
        }

        [Test]
        public async Task ClientCredentials_Test()
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

            var response = await client.GetAsync("http://localhost:6001/Identity");
            Assert.True(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}