using Desafio.Domain.Entities;
using Desafio.Infra.Mapping.ProductsMapping;
using Desafio.Infra.Mapping.UsersMapping;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infra.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region PRODUTOS
    public DbSet<Product> Products => Set<Product>();
    #endregion

    #region USUÁRIOS
    public DbSet<AppUser> Users => Set<AppUser>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region PRODUTOS
        modelBuilder.ApplyConfiguration(new ProductsMap());
        #endregion

        #region USUÁRIOS
        modelBuilder.ApplyConfiguration(new UsersMap());
        #endregion
    }
}
