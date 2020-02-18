using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaymentGateWayModels;

namespace PaymentGatewayDbContext
{
    public class PaymentGatewayDbContext : IdentityDbContext<ApplicationUser>
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options)
        : base(options)
        {
        }

        public DbSet<Merchant> tblMerchant { get; set; }
        public DbSet<Transactions> tblTransactions { get; set; }
        public DbSet<CardDetails> tblCardDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            PaymentGatewaySeeds.SeedMerchants(modelBuilder);
            PaymentGatewaySeeds.Roles(modelBuilder).Wait();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(ConnectionManager.Connection["ConnectionString"]);
            }
        }
    }
}
