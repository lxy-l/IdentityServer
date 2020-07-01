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
            InitializeDatabase(app);
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