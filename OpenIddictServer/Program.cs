using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using OpenIddictServer.Config;
using OpenIddictServer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseOpenIddict();
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                //.AllowCredentials()
                ;
        });
});

builder.Services.AddOpeniddictConfig();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();



// Use Microsoft.AspNetCore.Authentication.JwtBearer instead of OpenIddict.Validation.AspNetCore
//app.UseJwtTokenMiddleware();

app.UseAuthentication();
//app.UseOpenIddictValidation();
app.UseAuthorization();

app.MapControllers();

app.Run();

