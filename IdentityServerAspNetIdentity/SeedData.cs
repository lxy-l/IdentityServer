// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace IdentityServerAspNetIdentity
{
    public static class SeedData
    {
        public static async Task EnsureSeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            await scope.ServiceProvider.GetService<ApplicationDbContext>().Database.MigrateAsync();

            await scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.MigrateAsync();

            var context= scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            await context.Database.MigrateAsync();

            await CreateAspNetUser();

            await CreateClients();

            await CreateScopes();

            await CreateApiResources();

            await CreateIdentityResources();

            await context.SaveChangesAsync();

            async Task CreateAspNetUser()
            {
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var alice = await userMgr.FindByNameAsync("alice");
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                    };
                    var result = await userMgr.CreateAsync(alice, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created");
                }
                else
                {
                    Log.Debug("alice already exists");
                }

                var bob = await userMgr.FindByNameAsync("bob");
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com",
                        EmailConfirmed = true
                    };
                    var result = await userMgr.CreateAsync(bob, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        });
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }
            }

   


            async Task CreateClients()
            {
                if (!await context.Clients.AnyAsync())
                {
                    foreach (var item in Config.Clients)
                    {
                        await context.Clients.AddAsync(item.ToEntity());
                    }
                }
            }

            async Task CreateScopes()
            {
                if (!await context.ApiScopes.AnyAsync())
                {
                    foreach (var item in Config.ApiScopes)
                    {
                        await context.ApiScopes.AddAsync(item.ToEntity());
                    }
                }
            }

            async Task CreateApiResources()
            {
                if (!await context.ApiScopes.AnyAsync())
                {
                    foreach (var item in Config.ApiResources)
                    {
                        await context.ApiResources.AddAsync(item.ToEntity());
                    }
                }
            }

            async Task CreateIdentityResources()
            {
                if (!await context.ApiResources.AnyAsync())
                {
                    foreach (var item in Config.IdentityResources)
                    {
                        await context.IdentityResources.AddAsync(item.ToEntity());
                    }
                }
            }
        }
    }
}
