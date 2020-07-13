using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebTest.Models;

namespace WebTest.Data
{
    public sealed class InitData
    {
        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UserContext>();
                if (!context.Users.Any())
                {
                    context.Users.AddRange(new User { Id=1,Name = "Admin", PasswordHash = "Admin" },new User {Id=2,Name="lxy",PasswordHash="123" });
                }
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new Role { Id=1,RoleName="Admin"},new Role {Id=2,RoleName="User" },new Role { Id=3,RoleName="Guest"});
                }
                if (!context.UserRole.Any())
                {
                    context.UserRole.AddRange(new UserRole { Uid=1,Rid=1 },new UserRole { Uid=2,Rid=2});
                }
                context.SaveChangesAsync();
            }
        }
    }
}
