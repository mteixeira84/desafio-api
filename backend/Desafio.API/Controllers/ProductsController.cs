using Desafio.Application.Interfaces;
using Desafio.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.API.Controllers;

[ApiController]
[Route("products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductsAppService _service;

    public ProductsController(IProductsAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAll(cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductInputDto objDto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.Create(objDto, cancellationToken);

            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.Delete(id, cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
