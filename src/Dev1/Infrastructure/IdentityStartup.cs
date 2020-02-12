using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCompany.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MyCompany.Infrastructure {
    public static class IdentityConfiguration {
        public static IApplicationBuilder UseApplicationIdentity(this IApplicationBuilder @this,
            IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            //            var op1 = new Operation {Date = DateTime.Now, Description = "An operation", Amount = (decimal) 10.0};
//            var op2 = new Operation {Date = DateTime.Now, Description = "Another operation", Amount = (decimal) 20.0};
//            var op3 = new Operation {Date = DateTime.Now, Description = "Last one operation", Amount = (decimal) 30.0};


            SeedRoles(roleManager).Wait();
            SeedUsers(userManager).Wait();
            SeedUserRoles(userManager).Wait();

            return @this;
        }


        private static IEnumerable<Role> Roles()
        {
            return new List<Role> {
                new Role {Id = "role_admin", Name = "ROLE_ADMIN"},
                new Role {Id = "role_user",Name = "ROLE_USER"}
            };
        }

        private static IEnumerable<User> Users()
        {
            return new List<User> {
                new User {
                    Id = "user-0",
                    UserName = "system",
                    PasswordHash = "$2a$10$mE.qmcV0mFU5NcKh73TZx.z4ueI/.bDWbj0T1BYyqP481kGGarKLG",
                    FirstName = "",
                    LastName = "System",
                    Email = "system@localhost",
                    Activated = true,
                    LangKey = "en"
                },
                new User {
                    Id = "user-1",
                    UserName = "anonymoususer",
                    PasswordHash = "$2a$10$j8S5d7Sr7.8VTOYNviDPOeWX8KcYILUVJBsYV83Y5NtECayypx9lO",
                    FirstName = "Anonymous",
                    LastName = "User",
                    Email = "anonymous@localhost",
                    Activated = true,
                    LangKey = "en"
                },
                new User {
                    Id = "user-2",
                    UserName = "admin",
                    PasswordHash = "$2a$10$gSAhZrxMllrbgj/kkK9UceBPpChGWJA7SYIb1Mqo.n5aNLq1/oRrC",
                    FirstName = "admin",
                    LastName = "Administrator",
                    Email = "admin@localhost",
                    Activated = true,
                    LangKey = "en"
                },
                new User {
                    Id = "user-3",
                    UserName = "user",
                    PasswordHash = "$2a$10$VEjxo0jq2YG9Rbk2HmX9S.k1uZBGYUHdUcid3g/vfiEl7lwWgOH/K",
                    FirstName = "",
                    LastName = "User",
                    Email = "user@localhost",
                    Activated = true,
                    LangKey = "en"
                }
            };
        }

        private static IDictionary<string, string[]> UserRoles()
        {
            return new Dictionary<string, string[]> {
                {"user-0", new[] {"ROLE_ADMIN", "ROLE_USER"}},
                {"user-2", new[] {"ROLE_ADMIN", "ROLE_USER"}},
                {"user-3", new[] {"ROLE_USER"}}
            };
        }

        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            foreach (var role in Roles())
                if (await roleManager.FindByNameAsync(role.Name) == null)
                    await roleManager.CreateAsync(role);
                else
                    await roleManager.UpdateAsync(role);
        }

        private static async Task SeedUsers(UserManager<User> userManager)
        {
            foreach (var user in Users())
                if (await userManager.FindByIdAsync(user.Id) == null)
                    await userManager.CreateAsync(user);
                else
                    await userManager.UpdateAsync(user);
        }

        private static async Task SeedUserRoles(UserManager<User> userManager)
        {
            foreach (var (id, roles) in UserRoles()) {
                var user = await userManager.FindByIdAsync(id);
                await userManager.AddToRolesAsync(user, roles);
            }
        }
    }
}
