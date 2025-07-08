using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class SessionProductConfiguration : IEntityTypeConfiguration<SessionProduct>
{
    public void Configure(EntityTypeBuilder<SessionProduct> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TableSession)
            .WithMany(x => x.SessionProducts)
            .HasForeignKey(x => x.TableSessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
