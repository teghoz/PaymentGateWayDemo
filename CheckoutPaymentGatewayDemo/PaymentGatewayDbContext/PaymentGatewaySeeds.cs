using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
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
        public static async Task<IWebHost> CashboxSeedAsync(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Initialize(services);
                _context.Database.Migrate();          
                await Roles();
            }
            return host;
        }

        public static async Task Roles()
        {
            var roles = new string[]{ "AdminSuper", "Admin", "AdminRead", "AdminWrite", "AdminApproval", "Member" };

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

        public static void Users()
        {

        }
    }
}
