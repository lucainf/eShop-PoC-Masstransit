using Microsoft.EntityFrameworkCore;

namespace Products.Entities;

public class ProductsContext(DbContextOptions<ProductsContext> options) : DbContext(options)
{
    public virtual DbSet<Product> Products { get; set; }

    // Other DbSets for other entities
    public virtual DbSet<Category> Categories { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Additional configuration for the Product entity can go here
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
    }
}