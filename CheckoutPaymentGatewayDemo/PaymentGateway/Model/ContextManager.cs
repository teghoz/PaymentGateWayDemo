using Microsoft.EntityFrameworkCore;
using PaymentGatewayDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Model
{
    public static class ContextManager
    {
        public static string GetConnectionString()
        {
            return ConnectionManager.Connection["ConnectionString"];
        }
        public static PaymentGatewayDbContext.PaymentGatewayDbContext PaymentGatewayContext()
        {
            DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext> contextOptions = new DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext>();
            contextOptions.UseSqlServer(GetConnectionString());
            PaymentGatewayDbContext.PaymentGatewayDbContext context = new PaymentGatewayDbContext.PaymentGatewayDbContext(contextOptions.Options);
            return context;
        }
    }
}
