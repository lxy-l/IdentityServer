using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer.Config
{
    /// <summary>
    /// IdentityServer配置
    /// https://identityserver4.readthedocs.io/en/latest/topics/startup.html
    /// </summary>
    public static class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("API", "My API 1")
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        //public static IEnumerable<ApiScope> GetApiScopes()
        //{
        //    return new List<ApiScope>
        //    {
        //        new ApiScope(name: "read",   displayName: "Read your data."),
        //        new ApiScope(name: "write",  displayName: "Write your data."),
        //        new ApiScope(name: "delete", displayName: "Delete your data.")
        //    };
        //}

        public static IEnumerable<Client> Clients => new[]
        {
            // 客户端模式
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
            //授权码模式
            new Client
            {
                ClientId = "mvc",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequireConsent = false,
                RequirePkce = true,
                RedirectUris = { "http://localhost:5002" },
                PostLogoutRedirectUris = { "http://localhost:5002" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "API"
                },

                AllowOfflineAccess = true
            },
            //密码模式
            new Client
            {
                ClientId = "password.client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "API" }
            },
            //简化模式 With OpenID
            new Client
            {
                    ClientId = "Implicit",
                    ClientName = "Implicit Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
             },
            //简化模式 With OpenID & OAuth
            new Client
            {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { "http://localhost:5002/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    RequireConsent = false, //禁用 consent 页面确认 https://github.com/IdentityServer/IdentityServer3/issues/863

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "API"
                    }
            },
            //混合模式With OpenID & OAuth
             new Client
             {
                    ClientId = "Hybrid Flow",
                    ClientName = "Hybrid Flow Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5021/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5021" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "API"
                    },
                    AllowOfflineAccess = true
             }
        };

        // 指定可以使用 Authorization Server 授权的 Users（用户）
        public static IEnumerable<TestUser> Users => new[]
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "admin"
            },
            new TestUser
                {
                    SubjectId = "2",
                    Username = "lxy",
                    Password = "123",
                    Claims = new List<Claim>
                    {
                        new Claim("Name", "Lxy"),
                        new Claim("Role", "User")
                    }
                }
        };
    }
}