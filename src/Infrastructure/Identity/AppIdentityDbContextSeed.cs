using ApplicationCore.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(AppIdentityDbContext db,RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            //veritabanı yoksa oluştur.
            await db.Database.MigrateAsync();


            //admin rolü ekle

            if (!await roleManager.RoleExistsAsync(AuthorizationConstants.Roles.ADMINISTRATOR))
            {
                var adminRole = new IdentityRole(AuthorizationConstants.Roles.ADMINISTRATOR);
                await roleManager.CreateAsync(adminRole);
            }
            //Admin kullanıcı yoksa ekle =>

            if (await userManager.FindByNameAsync(AuthorizationConstants.DEFAULT_ADMIN_USER) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = AuthorizationConstants.DEFAULT_ADMIN_USER,
                    Email = AuthorizationConstants.DEFAULT_ADMIN_USER,
                    EmailConfirmed = true

                };
                
                await userManager.CreateAsync(adminUser, AuthorizationConstants.DEFAULT_PASSWORD);

                //kullanıcıya admin rolünü ekle =>
                await userManager.AddToRoleAsync(adminUser, AuthorizationConstants.Roles.ADMINISTRATOR);
            }

            //demo kullanıcısı oluştur =>
            if (await userManager.FindByNameAsync(AuthorizationConstants.DEFAULT_DEMO_USER) == null)
            {
                var demoUser = new ApplicationUser
                {
                    UserName = AuthorizationConstants.DEFAULT_DEMO_USER,
                    Email = AuthorizationConstants.DEFAULT_DEMO_USER,
                    EmailConfirmed = true
                
                };
                await userManager.CreateAsync(demoUser, AuthorizationConstants.DEFAULT_PASSWORD); 

              
            }
        }

    }
}
