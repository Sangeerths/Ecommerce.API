using Ecommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category -> Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sale -> SaleItems
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product -> SaleItems
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            var seedDate = new DateTime(2025, 1, 1);

            // =========================
            // Categories
            // =========================

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Electronics",
                    Description = "Phones, laptops, and gadgets",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Home & Kitchen",
                    Description = "Appliances and kitchenware",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Books",
                    Description = "Fiction and non-fiction books",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );


            // =========================
            // Products
            // =========================

            modelBuilder.Entity<Product>().HasData(
                new
                {
                    ProductId = 1,
                    Name = "Laptop",
                    Description = "15-inch laptop, 16GB RAM",
                    Price = 999.99m,
                    StockQuantity = 10,
                    CategoryId = 1,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new
                {
                    ProductId = 2,
                    Name = "Wireless Mouse",
                    Description = "Bluetooth mouse, ergonomic design",
                    Price = 29.99m,
                    StockQuantity = 50,
                    CategoryId = 1,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new
                {
                    ProductId = 3,
                    Name = "Blender",
                    Description = "700W countertop blender",
                    Price = 49.99m,
                    StockQuantity = 20,
                    CategoryId = 2,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new
                {
                    ProductId = 4,
                    Name = "C# Programming Guide",
                    Description = "Beginner to advanced C# book",
                    Price = 39.99m,
                    StockQuantity = 15,
                    CategoryId = 3,
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );


            // =========================
            // Sales
            // =========================

            modelBuilder.Entity<Sale>().HasData(
                new Sale
                {
                    SaleId = 1,
                    SaleDate = seedDate,
                    CustomerName = "Jane Doe",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Sale
                {
                    SaleId = 2,
                    SaleDate = seedDate,
                    CustomerName = "John Smith",
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new Sale
                {
                    SaleId = 3,
                    SaleDate = seedDate,
                    CustomerName = "Jane Doe",
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );


            // =========================
            // Sale Items
            // =========================

            modelBuilder.Entity<SaleItem>().HasData(
                new SaleItem
                {
                    SaleItemId = 1,
                    SaleId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 999.99m,
                    CreatedAt = seedDate
                },
                new SaleItem
                {
                    SaleItemId = 2,
                    SaleId = 2,
                    ProductId = 2,
                    Quantity = 2,
                    UnitPrice = 29.99m,
                    CreatedAt = seedDate
                },
                new SaleItem
                {
                    SaleItemId = 3,
                    SaleId = 3,
                    ProductId = 4,
                    Quantity = 1,
                    UnitPrice = 39.99m,
                    CreatedAt = seedDate
                }
            );
        }
    }
}