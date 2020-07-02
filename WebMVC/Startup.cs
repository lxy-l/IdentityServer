using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;

using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.AccessDeniedPath = "/Authroization/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("API");
                options.Scope.Add("offline_access");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("roles");

                //从过滤集合移除
                options.ClaimActions.Remove("nbf");
                options.ClaimActions.Remove("amr");
                //删除某些Claims
                options.ClaimActions.DeleteClaim("sid");
                options.ClaimActions.DeleteClaim("idp");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.MapUniqueJsonKey("role", "role");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
