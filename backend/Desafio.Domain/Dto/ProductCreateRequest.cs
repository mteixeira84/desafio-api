namespace Desafio.Domain.Dto;

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}
