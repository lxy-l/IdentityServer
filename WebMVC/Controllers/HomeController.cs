using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.token= await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken).ConfigureAwait(false);
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var client = new HttpClient();
            var authorizationServerInfo = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (authorizationServerInfo.IsError)
            {
                Console.WriteLine(authorizationServerInfo.Error);
                return Content(authorizationServerInfo.Error);
            }
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = authorizationServerInfo.TokenEndpoint,
                ClientId = "HybridFlow",
                ClientSecret = "secret",
                //Scope = "api1",
                RefreshToken = await HttpContext.GetTokenAsync("refresh_token")
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return Content(tokenResponse.Error);
            }
            var identityToken = await HttpContext.GetTokenAsync("id_token");
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResponse.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };
            var authenticationInfo = await HttpContext.AuthenticateAsync("Cookies");
            authenticationInfo.Properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);

            return View(nameof(Index));
        }


        [Authorize(Roles ="管理员")]
        public async Task<IActionResult> PrivacyAsync()
        {
            await RefreshToken();
            var token = await HttpContext.GetTokenAsync("access_token");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
