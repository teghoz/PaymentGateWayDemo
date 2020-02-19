using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGatewayDbContext
{
    public class PaymentGatewayContextFactory : IDesignTimeDbContextFactory<PaymentGatewayDbContext>
    {
        public PaymentGatewayDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentGatewayDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PaymentGateway;Trusted_Connection=True;ConnectRetryCount=0");

            return new PaymentGatewayDbContext(optionsBuilder.Options);
        }
    }
}
