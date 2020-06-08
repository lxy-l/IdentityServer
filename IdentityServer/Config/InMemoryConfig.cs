using IdentityServer4.Models;
using IdentityServer4.Test;

using System.Collections.Generic;

namespace IdentityServer.Config
{
    public static class InMemoryConfig
    {
        // 这个 Authorization Server 保护了哪些 API （资源）
        public static IEnumerable<ApiResource> GetApiResources() => new[]
        {
            new ApiResource("API", "API")
        };

        // 哪些客户端 Client（应用） 可以使用这个 Authorization Server
        public static IEnumerable<Client> GetClients() => new[]
        {
            new Client
            {
                ClientId = "api.service",//定义客户端 Id
                ClientSecrets = new [] { new Secret("secret".Sha256()) },//Client用来获取token
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new [] { "API" }// 允许访问的 API 资源
            }
        };

        // 指定可以使用 Authorization Server 授权的 Users（用户）
        public static IEnumerable<TestUser> Users() => new[]
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "admin"
            }
        };
    }
}