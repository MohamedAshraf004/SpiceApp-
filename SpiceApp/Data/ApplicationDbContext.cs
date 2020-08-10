using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpiceApp.Models;

namespace SpiceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<SubCategory> SubCategories{ get; set; }
        public DbSet<MenuItem> MenuItems{ get; set; }
        public DbSet<Coupon> Coupons{ get; set; }
        public DbSet<ApplicationUser> ApplicationUsers{ get; set; }
        public DbSet<ShoppingCart> shoppingCarts{ get; set; }
        public DbSet<OrderHeader> OrderHeaders{ get; set; }
        public DbSet<OrderDetails> OrderDetails{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MenuItem>().HasOne(c => c.Category).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<MenuItem>().HasOne(c => c.SubCategory).WithMany().OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(builder);
        }
    }
}
