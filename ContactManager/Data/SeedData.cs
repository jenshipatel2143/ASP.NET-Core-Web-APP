using ContactManager.Authorization;
using ContactManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// dotnet aspnet-codegenerator razorpage -m Contact -dc ApplicationDbContext -udl -outDir Pages\Contacts --referenceScriptLibraries

namespace ContactManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@contoso.com");
                await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@contoso.com");
                await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static Task EnsureRole(IServiceProvider serviceProvider, string adminID, object contactAdministratorsRole)
        {
            throw new NotImplementedException();
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
               // await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            //if (user == null)
            {
               // throw new Exception("The testUserPw password was probably not strong enough!");
            }

            // IR = await userManager.AddToRoleAsync(user, role);

            //return IR;

            return null;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Contact.Any())
            {
                return;   // DB has been seeded
            }

            context.Contact.AddRange(
                new Contact
                {
                    Name = "Pankti Parekh",
                    Address = "290 Duckworth St",
                    City = "Barrie",
                    State = "ON",
                    Zip = "L4M 3X4",
                    Email = "parekhpankti99@gmail.com",
                    Status = ContactStatus.Approved,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Jenshi Patel",
                    Address = "324 west 5th St",
                    City = "Hamilton",
                    State = "ON",
                    Zip = "L9C 3P3",
                    Email = "jenshipatel2143@gmail.com",
                    Status = ContactStatus.Rejected,
                    OwnerID = adminID
                },
             new Contact
             {
                 Name = "Karthikeyni Rajendran",
                 Address = "9012 State st",
                 City = "Scarborough",
                 State = "ON",
                 Zip = "10999",
                 Email = "karthikeyni@gmail.com",
                 Status = ContactStatus.Submitted,
                 OwnerID = adminID
             },
             new Contact
             {
                 Name = "Rupinder Kaur",
                 Address = "3456 Maple St",
                 City = "Brampton",
                 State = "ON",
                 Zip = "10978",
                 Email = "rupinderjeetkaur@gmail.com",
                 Status = ContactStatus.Approved,
                 OwnerID = adminID
             },
             new Contact
             {
                 Name = "Tithi Patel",
                 Address = "7890 2nd Ave E",
                 City = "Toronto",
                 State = "ON",
                 Zip = "10456",
                 Email = "tithipatel@gmail.com",
                 Status = ContactStatus.Approved,
                 OwnerID = adminID
             }
             );
            context.SaveChanges();
        }

    }
}