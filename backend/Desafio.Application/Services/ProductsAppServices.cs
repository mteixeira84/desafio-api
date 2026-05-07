using Desafio.Application.Interfaces;
using Desafio.Domain.Dto;
using Desafio.Domain.Entities;
using Desafio.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Application.Services;
public class ProductsAppServices : IProductsAppService
{
    private readonly AppDbContext _db;

    public ProductsAppServices(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductOutputDto> Create(ProductInputDto objDto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objDto.Name))
        {
            throw new ArgumentException("Nome do produto é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(objDto.Category))
        {
            throw new ArgumentException("Categoria é obrigatória.");
        }

        if (objDto.Price <= 0)
        {
            throw new ArgumentException("Preço deve ser maior que zero.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = objDto.Name.Trim(),
            Category = objDto.Category.Trim(),
            Price = objDto.Price
        };

        _db.Products.Add(product);

        await _db.SaveChangesAsync(cancellationToken);

        return new ProductOutputDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price
        };
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product is null)
        {
            throw new ArgumentException("Produto não encontrado.");
        }

        _db.Products.Remove(product);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductOutputDto>> GetAll(CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProductOutputDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price
            })
            .ToListAsync(cancellationToken);
    }
}
