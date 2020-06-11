using IdentityServer.Config;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddIdentityServer()
                //.AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))//appsettings.json文件定义静态客户端
                .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
                .AddInMemoryClients(IdentityServerConfig.Clients)
                .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
                .AddDeveloperSigningCredential()
                .AddTestUsers(IdentityServerConfig.Users.ToList());

            //services.AddAuthentication("Cookies")
            //    .AddCookie("Cookies", options =>
            //    {
            //        options.ExpireTimeSpan = TimeSpan.Zero;
            //    });//Cookie验证

            //services.AddAuthentication()
            //     .AddGoogle("Google", options =>
            //     {
            //         options.SignInScheme = "scheme of cookie handler to use";

            //         options.ClientId = "...";
            //         options.ClientSecret = "...";
            //     });//谷歌支持,Microsoft.AspNetCore.Authentication.Google

            //services.AddAuthentication()
            //        .AddCookie("YourCustomScheme")
            //        .AddGoogle("Google", options =>
            //        {
            //            options.SignInScheme = "YourCustomScheme";

            //            options.ClientId = "...";
            //            options.ClientSecret = "...";
            //        });//自定义
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            //app.UseAuthorization();
            //app.UseAuthentication();UseIdentityServer包含对的调用UseAuthentication，因此没有必要同时使用两者。
            // AddAuthentication将身份验证服务添加到DI并配置Bearer为默认方案。
            // UseAuthentication 将身份验证中间件添加到管道中，以便对主机的每次调用都将自动执行身份验证。
            // UseAuthorization 添加了授权中间件，以确保匿名客户端无法访问我们的API端点。
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}