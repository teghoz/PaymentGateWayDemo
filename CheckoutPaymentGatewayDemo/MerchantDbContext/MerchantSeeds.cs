using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using MerchantModels;
using System.Linq;

namespace MerchantDbContext
{
    public static class MerchantSeeds
    {
        private static MerchantDbContext _context;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        private static IHostingEnvironment _hostingEnvironment;

        public static void Initialize(IServiceProvider provider)
        {
            _context = provider.GetRequiredService<MerchantDbContext>();
            _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            _hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
        }
        public static async Task<IWebHost> MerchantSeedAsync(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Initialize(services);
                _context.Database.Migrate();             
                await Roles();
                await Users(services);
            }
            return host;
        }

        public static ModelBuilder SeedCustomers(this ModelBuilder _modelBuilder)
        {
            _modelBuilder.Entity<Customer>().HasData(

                new Customer { FirstName = "Aghogho", LastName = "Bernard", Email = "aghoghobernard@gmail.com", Id = 1 },
                new Customer { FirstName = "William", LastName = "Bernard", Email = "williambernard@gmail.com", Id = 2 });

            return _modelBuilder;
        }

        public static async Task Roles()
        {
            var roles = new string[]{ "Customer", "Admin" };

            foreach(var r in roles)
            {
                bool isExist = await _roleManager.RoleExistsAsync(r);

                if (!isExist)
                {
                    var role = new IdentityRole(r);
                    await _roleManager.CreateAsync(role);
                }
            }

            await Task.CompletedTask;
        }

        public static async Task Users(IServiceProvider provider)
        {
            using (var context = provider.GetRequiredService<MerchantDbContext>())
            {
                if (context.tblCustomer.Any(a => a.Email == "aghoghobernard@gmail.com") && !context.Users.Any(a => a.UserName == "aghoghobernard@gmail.com"))
                {
                    var customer = context.tblCustomer.Where(a => a.Email == "aghoghobernard@gmail.com").FirstOrDefault();
                    ApplicationUser user = new ApplicationUser
                    {
                        CustomerId = customer.Id,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email,
                        UserName = customer.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "ch@ck0ut");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }
                }
            }
        }
    }
}
