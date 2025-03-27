using EcommerceModular.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceModular.Infrastructure.Configurations;

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