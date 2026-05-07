using Desafio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Infra.Mapping.ProductsMapping;
public class ProductsMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable("Products")
            .HasKey(u => u.Id);

        builder.Property(p => p.Name).IsRequired().HasColumnType("varchar(200)");
        builder.Property(p => p.Category).IsRequired().HasColumnType("varchar(150)");
        builder.Property(p => p.Price).HasPrecision(18, 2);
    }
}