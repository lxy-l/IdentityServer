using IdentityServer.Config;

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;
using System.Reflection;

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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectstring = Configuration.GetConnectionString("MysqlConnection");
            //services.AddControllers();
            services.AddControllersWithViews();
            services.AddIdentityServer()
                //.AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))//appsettings.json文件定义静态客户端
                .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
                .AddInMemoryClients(IdentityServerConfig.Clients)
                .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
                .AddDeveloperSigningCredential()
                .AddTestUsers(IdentityServerConfig.Users.ToList())
                .AddConfigurationStore(options=> 
                {
                    options.ConfigureDbContext = b => b.UseMySql(connectstring, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options=> 
                {
                    options.ConfigureDbContext = b => b.UseMySql(connectstring, sql => sql.MigrationsAssembly(migrationsAssembly));
                });

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
            InitializeDatabase(app);
            app.UseIdentityServer();
            app.UseAuthorization();
            //app.UseAuthentication();UseIdentityServer包含对的调用UseAuthentication，因此没有必要同时使用两者。
            // AddAuthentication将身份验证服务添加到DI并配置Bearer为默认方案。
            // UseAuthentication 将身份验证中间件添加到管道中，以便对主机的每次调用都将自动执行身份验证。
            // UseAuthorization 添加了授权中间件，以确保匿名客户端无法访问我们的API端点。
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityServerConfig.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityServerConfig.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in IdentityServerConfig.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}