using Desafio.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infra.Context;

public static class DbInitializer
{
    public static async Task SeedAdminIfEmptyAsync(
        AppDbContext db,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var section = configuration.GetSection("SeedAdmin");
        if (!section.GetValue("Enabled", false))
        {
            logger.LogWarning(
                "Nenhum usuario no banco e SeedAdmin.Enabled=false. " +
                "Crie um usuario manualmente ou habilite SeedAdmin em desenvolvimento.");
            return;
        }

        var username = section["Username"]?.Trim().ToLowerInvariant();
        var password = section["Password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            logger.LogWarning("SeedAdmin habilitado mas Username ou Password ausentes.");
            return;
        }

        var hasher = new PasswordHasher<AppUser>();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = string.Empty
        };
        user.PasswordHash = hasher.HashPassword(user, password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Usuario inicial criado: {Username}", username);
    }
}
