using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateWayModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGatewayDbContext
{
    public static class PaymentGatewaySeeds
    {
        private static PaymentGatewayDbContext _context;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        private static IHostingEnvironment _hostingEnvironment;

        public static void Initialize(IServiceProvider provider)
        {
            _context = provider.GetRequiredService<PaymentGatewayDbContext>();
            _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            _hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
        }

        public static async Task<IWebHost> PaymentGatewaySeedAsync(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Initialize(services);
                _context.Database.Migrate();
                await MerchantUser(services);
            }
            return host;
        }

        public static async Task Roles(this ModelBuilder _modelBuilder)
        {
            _modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Name = "AdminSuper", NormalizedName = "ADMINSUPER" },
            new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "Merchant", NormalizedName = "MERCHANT"});

            await Task.CompletedTask;
        }

        public static ModelBuilder SeedMerchants(this ModelBuilder _modelBuilder)
        {
            _modelBuilder.Entity<Merchant>().HasData(

                new Merchant { FirstName = "Aghogho", LastName = "Bernard", Email = "aghoghomerchant@gmail.com", Id = 1 });

            return _modelBuilder;
        }

        public static async Task MerchantUser(IServiceProvider provider)
        {
            using (var context = provider.GetRequiredService<PaymentGatewayDbContext>())
            {
                if (_context.tblMerchant.Any(a => a.Email == "aghoghomerchant@gmail.com") && !_context.Users.Any(a => a.UserName == "aghoghomerchant@gmail.com"))
                {
                    var merchant = _context.tblMerchant.Where(a => a.Email == "aghoghomerchant@gmail.com").FirstOrDefault();
                    ApplicationUser user = new ApplicationUser
                    {
                        MerchantId = merchant.Id,
                        FirstName = merchant.FirstName,
                        LastName = merchant.LastName,
                        Email = merchant.Email,
                        UserName = merchant.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "ch@ck0ut");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Merchant");
                    }
                }
            }
        }
    }
}
