using System.Collections.Immutable;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServerAspNetIdentity.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthorizationController(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _scopeManager = scopeManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            var claims = new List<Claim>
            {
                new Claim(OpenIddictConstants.Claims.Subject, result.Principal.Identity.Name),
                new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
                new Claim(OpenIddictConstants.Claims.Email, "some@email").SetDestinations(OpenIddictConstants.Destinations.IdentityToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            claimsPrincipal.SetScopes(request.GetScopes());
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
        {
            var resources = new List<string>();
            if (!scopes.Any())
            {
                return resources;
            }

            await foreach (var resource in _scopeManager.ListResourcesAsync(scopes))
            {
                resources.Add(resource);
            }
            return resources;
        }
        //[HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
        [HttpPost]
        [Route("~/connect/token")]
        public async Task<IActionResult> Token()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            ClaimsPrincipal claimsPrincipal;

            if (request.IsClientCredentialsGrantType())
            {
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
                identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);
                claimsPrincipal = new ClaimsPrincipal(identity);
                claimsPrincipal.SetScopes(request.GetScopes());
            }
            else if (request.IsAuthorizationCodeGrantType())
            {
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }

            else if (request.IsRefreshTokenGrantType())
            {
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }
            else if (request.IsPasswordGrantType())
            {

                var username=request.Username;
                var password=request.Password;  


                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    bool isSuccess = await _userManager.CheckPasswordAsync(user, password);
                    if (isSuccess)
                    {
                        var claimsIdentity = await _userManager.GetClaimsAsync(user);
                        var identity = new ClaimsIdentity(claimsIdentity,
                            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                            nameType: Claims.Name,
                            roleType: Claims.Role);

                        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                                    .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                                    .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user));
                                    //.SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());
                        claimsPrincipal = new ClaimsPrincipal(identity);
                        claimsPrincipal.SetScopes(request.GetScopes());
                        claimsPrincipal.SetResources(await GetResourcesAsync(request.GetScopes()));
                        //claimsPrincipal.
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            return Ok(new
            {
                Name = claimsPrincipal.GetClaim(OpenIddictConstants.Claims.Subject),
                Occupation = "Developer",
                Age = 43
            });
        }

    }
}

