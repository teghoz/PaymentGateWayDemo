using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantDbContext
{
    public class MerchantContextFactory : IDesignTimeDbContextFactory<MerchantDbContext>
    {
        public MerchantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MerchantDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Merchant;Trusted_Connection=True;ConnectRetryCount=0");

            return new MerchantDbContext(optionsBuilder.Options);
        }
    }
}
