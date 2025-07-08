using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DAL.SqlServer.Configurations;

public class TablesPriceConfiguration : IEntityTypeConfiguration<TablesPrice>
{
    public void Configure(EntityTypeBuilder<TablesPrice> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.HourlyPrice)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(tp => tp.TableType)
               .IsRequired();

        builder.Property(tp => tp.CompanyId)
               .IsRequired();

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
