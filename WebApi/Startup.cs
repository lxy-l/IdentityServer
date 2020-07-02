using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.AccessTokenValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;

namespace WebApi
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
            //��ʽ��ʱ��
            services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss"; });

            #region Authentication
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = "http://localhost:5000";
                        options.RequireHttpsMetadata = false;
                        options.Audience = "API";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //        .AddIdentityServerAuthentication(options =>
            //        {
            //            // base-address of your identityserver
            //            options.Authority = "http://localhost:5000";
            //            options.SaveToken = true;
            //            options.RequireHttpsMetadata = false;
            //            // name of the API resource
            //            options.ApiName = "API";
            //            options.ApiSecret = "secret";
            //            options.Validate();
            //        });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "API");
                });
            });

            #endregion

            #region Swagger

            var basePath = Directory.GetCurrentDirectory();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = $"IdentityServer �ӿ��ĵ�����Netcore 3.1",
                    Description = $"IdentityServer HTTP API V1"

                });
                c.OrderActionsBy(o => o.RelativePath);
                var xmlPath = Path.Combine(basePath, "WebApi.xml");
                c.IncludeXmlComments(xmlPath, true);
                //var xmlModelPath = Path.Combine(basePath, "Model.xml");
                //c.IncludeXmlComments(xmlModelPath);
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                #region Token�󶨵�ConfigureServices
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion


            });

            #endregion

            #region Cors��������
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5000")
                               .AllowAnyHeader()
                               .WithMethods("GET", "POST", "PUT", "DELETE");
                    });
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(u=> {
                    u.SwaggerEndpoint("/swagger/V1/swagger.json","IdentityServer V1");
                    u.RoutePrefix = "";
                });

            }

            app.UseRouting();

            app.UseCors("MyPolicy");
            app.UseStatusCodePages();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("ApiScope"); ;
            });
        }
    }
}
