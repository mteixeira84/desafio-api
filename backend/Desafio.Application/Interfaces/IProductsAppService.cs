using Desafio.Domain.Dto;

namespace Desafio.Application.Interfaces;
public interface IProductsAppService
{
    Task<ProductOutputDto> Create(ProductInputDto objDto, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductOutputDto>> GetAll(CancellationToken cancellationToken = default);
}
