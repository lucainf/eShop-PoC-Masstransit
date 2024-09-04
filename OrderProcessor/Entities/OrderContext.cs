using Microsoft.EntityFrameworkCore;

namespace OrderProcessor.Entities;

public class OrderContext(DbContextOptions<OrderContext> options) : DbContext(options)
{
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderProduct> OrderProducts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the OrderProduct composite key
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });
        
        // Configure the relationship between Order and OrderProduct
        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany()
            .HasForeignKey(op => op.OrderId);
    }
}