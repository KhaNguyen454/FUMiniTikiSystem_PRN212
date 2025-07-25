using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace DataAccessLayer
{
    public class FUMiniTikiSystemDBContext : DbContext
    {
        public FUMiniTikiSystemDBContext(DbContextOptions<FUMiniTikiSystemDBContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationships explicitly if needed,
            // though EF Core often infers them.
            // Here, we can configure cascade deletes or other specific behaviors.

            // Category - Products (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting category if products exist

            // Customer - Orders (One-to-Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting customer if orders exist

            // Order - Products (Many-to-Many through Order, based on ERD)
            // This is an unusual direct many-to-many without a join table, based on the ERD's Product.OrderID.
            // If Product has OrderId, it means each Product instance is tied to AT MOST one Order.
            // The ERD implies one order can have many products, and one product *can* be on many orders (implicitly, if it's a type of product).
            // However, Product.OrderID means a specific instance of a Product (record) belongs to a specific Order record.
            // Let's re-interpret the ERD's Product.OrderID as potentially being a simple one-to-many from Order to Product,
            // meaning a product instance is "associated" with an order, or it's a simplified view of OrderDetail.
            // For now, follow the ERD exactly: Product has an OrderId.
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.SetNull); // If order deleted, set Product.OrderId to null. This assumes OrderId in Product is nullable.

            // Seed data (Optional, but useful for quick testing)
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic gadgets and devices" },
                new Category { CategoryId = 2, Name = "Books", Description = "Various books and literature" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, Name = "Test User", Email = "test@example.com", Password = "password123" }
            );
        }
    }
}