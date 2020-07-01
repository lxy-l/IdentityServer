using IdentityServer.Config;

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
            string connectstring = Configuration.GetConnectionString("DefaultConnection");
            //services.AddControllers();
            services.AddControllersWithViews();
            services.AddIdentityServer()
                //.AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))//appsettings.json�ļ����徲̬�ͻ���
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
            //    });//Cookie��֤

            //services.AddAuthentication()
            //     .AddGoogle("Google", options =>
            //     {
            //         options.SignInScheme = "scheme of cookie handler to use";

            //         options.ClientId = "...";
            //         options.ClientSecret = "...";
            //     });//�ȸ�֧��,Microsoft.AspNetCore.Authentication.Google

            //services.AddAuthentication()
            //        .AddCookie("YourCustomScheme")
            //        .AddGoogle("Google", options =>
            //        {
            //            options.SignInScheme = "YourCustomScheme";

            //            options.ClientId = "...";
            //            options.ClientSecret = "...";
            //        });//�Զ���
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
            app.UseAuthorization();
            //app.UseAuthentication();UseIdentityServer�����Եĵ���UseAuthentication�����û�б�Ҫͬʱʹ�����ߡ�
            // AddAuthentication�������֤������ӵ�DI������BearerΪĬ�Ϸ�����
            // UseAuthentication �������֤�м����ӵ��ܵ��У��Ա��������ÿ�ε��ö����Զ�ִ�������֤��
            // UseAuthorization �������Ȩ�м������ȷ�������ͻ����޷��������ǵ�API�˵㡣
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}