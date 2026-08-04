using Ecommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Sales)
                .WithMany(s => s.Products)
                .UsingEntity(j => j.ToTable("ProductSales"));

            var seedDate = new DateTime(2025, 1, 1);

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

            modelBuilder.Entity<Sale>().HasData(
                new
                {
                    SaleId = 1,
                    SaleDate = seedDate,
                    CustomerName = "Jane Doe",
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 999.99m,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new
                {
                    SaleId = 2,
                    SaleDate = seedDate,
                    CustomerName = "John Smith",
                    ProductId = 2,
                    Quantity = 2,
                    UnitPrice = 29.99m,
                    CreatedAt = seedDate,
                    IsDeleted = false
                },
                new
                {
                    SaleId = 3,
                    SaleDate = seedDate,
                    CustomerName = "Jane Doe",
                    ProductId = 4,
                    Quantity = 1,
                    UnitPrice = 39.99m,
                    CreatedAt = seedDate,
                    IsDeleted = false
                }
            );
        }
    }
}
