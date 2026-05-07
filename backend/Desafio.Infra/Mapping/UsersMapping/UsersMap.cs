using Desafio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Infra.Mapping.UsersMapping;
public class UsersMap : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .ToTable("Users")
            .HasKey(u => u.Id);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Username).IsRequired().HasColumnType("varchar(100)");
        builder.Property(u => u.PasswordHash).IsRequired().HasColumnType("varchar(500)");
    }
}