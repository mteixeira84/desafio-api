using Desafio.Application.Interfaces;
using Desafio.Application.Services;
using Desafio.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Desafio.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegistrarServicos(this IServiceCollection services)
    {
        // Servicos
        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        services.AddScoped<IProductsAppService, ProductsAppServices>();
    }
}
