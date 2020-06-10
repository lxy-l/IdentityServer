
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

using System.Collections.Generic;

namespace IdentityServer.Config
{
    /// <summary>
    /// IdentityServer配置
    /// https://identityserver4.readthedocs.io/en/latest/topics/startup.html
    /// </summary>
    public static class InMemoryConfig
    {
        public static IEnumerable<ApiResource> GetApiResources() => new[]
        {
            new ApiResource("API", "API"),
            new ApiResource("API2", "API2"),
        };
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResource(
                    name: "profile",
                    claimTypes: new[] { "name", "email", "website" },
                    displayName: "Your profile data")
            };
        }
        //public static IEnumerable<ApiScope> GetApiScopes()
        //{
        //    return new List<ApiScope>
        //    {
        //        new ApiScope(name: "read",   displayName: "Read your data."),
        //        new ApiScope(name: "write",  displayName: "Write your data."),
        //        new ApiScope(name: "delete", displayName: "Delete your data.")
        //    };
        //}

        public static IEnumerable<Client> GetClients() => new[]
        {
            new Client
            {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "API" }
            },
            new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                ClientUri = "http://identityserver.io",

                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },

                RedirectUris =           { "http://localhost:21402/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:21402/" },
                FrontChannelLogoutUri =  "http://localhost:21402/signout-oidc",

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,

                    "API2"
                },
            },
            new Client
            {
                ClientId = "API2",
                ClientSecrets = new [] { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new [] { "API2" }
            },
            new Client
            {
                 ClientId = "API2_Impl",
                 ClientName = "Test Client",
                 AllowedGrantTypes = GrantTypes.Implicit,
                 AllowAccessTokensViaBrowser = true,
                 RedirectUris =           { "http://localhost:6688/callback" },
                 PostLogoutRedirectUris = { "http://localhost:6688" },
                 AllowedCorsOrigins =     { "http://localhost:6688" },
                 AllowedScopes = {
                     IdentityServerConstants.StandardScopes.OpenId,
                     IdentityServerConstants.StandardScopes.Profile,
                     "API2"
                    }
            },
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