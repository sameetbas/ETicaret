using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration) //IConfiguration configuration appsettings.jsonda yazdıgımız etkin , Sabit admin
        {
            var username = configuration["Data:AdminUser:username"];
            var email = configuration["Data:AdminUser:email"];
            var password = configuration["Data:AdminUser:password"];
            var role = configuration["Data:AdminUser:role"];

            if (await userManager.FindByNameAsync(username) == null) //UserManagerden gelen username boş ise
            {
                await roleManager.CreateAsync(new IdentityRole(role)); // yeni oluşturacak
                var user = new ApplicationUser()
                {
                    UserName = username,
                    Email = email,
                    Fullname = "Admin User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user,password);

                if (result.Succeeded) //Eğer databasede bu kayıt oluşturulduysa
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                //sonrasında startupa IConfiguration tipinde bir property olusturup constructor metodunu doldurduk.
            }
        }
    }
}
