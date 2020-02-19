using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MerchantModels;

namespace MerchantDbContext
{
    public class MerchantDbContext : IdentityDbContext<ApplicationUser>
    {
        public MerchantDbContext(DbContextOptions<MerchantDbContext> options)
        : base(options)
        {
        }

        public DbSet<Customer> tblCustomer { get; set; }
        public DbSet<Payment> tblPayment { get; set; }
        public DbSet<Orders> tblOrders { get; set; }
        public DbSet<OrderDetails> tblOrderDetails { get; set; }
        public DbSet<Products> tblProducts { get; set; }
        public DbSet<Payment> tblPayments { get; set; }
        public DbSet<PaymentMethod> tblPaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
               .HasOne(p => p.Orders).WithMany().HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

            MerchantSeeds.SeedCustomers(modelBuilder);
            //MerchantSeeds.Roles().Wait();
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
