using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;
// using Orders.Domain.Enums;

namespace Orders.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.OwnsOne(o => o.ShippingAddress);
        builder.Property(o => o.Status).HasConversion<string>();
        builder.HasMany(o => o.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}