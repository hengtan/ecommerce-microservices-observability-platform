using EcommerceModular.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceModular.Infrastructure.Persistence;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        // base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Order>(order =>
        {
            order.OwnsOne(o => o.ShippingAddress); // Owned type
            order.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId"); // Ou use shadow property
        });

        modelBuilder.Entity<OrderItem>().HasKey(x => x.Id); // Adiciona a PK
    }
}