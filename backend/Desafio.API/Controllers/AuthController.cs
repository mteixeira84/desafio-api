using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Desafio.Domain.Dto;
using Desafio.Domain.Entities;
using Desafio.Infra.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Desafio.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    AppDbContext db,
    IConfiguration configuration,
    IPasswordHasher<AppUser> passwordHasher) : ControllerBase
{
    [HttpGet("registration-open")]
    public async Task<IActionResult> RegistrationOpen(CancellationToken cancellationToken)
    {
        var any = await db.Users.AnyAsync(cancellationToken);
        return Ok(new { canRegister = !any });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await db.Users.AnyAsync(cancellationToken))
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "Cadastro fechado: ja existe usuário cadastrado. Use a tela de login." });
        }

        var normalized = (request.Username ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.Length < 3)
        {
            return BadRequest(new { message = "Usuário deve ter pelo menos 3 caracteres." });
        }

        var password = request.Password ?? string.Empty;
        if (password.Length < 6)
        {
            return BadRequest(new { message = "Senha deve ter pelo menos 6 caracteres." });
        }

        var exists = await db.Users.AnyAsync(u => u.Username == normalized, cancellationToken);
        if (exists)
        {
            return Conflict(new { message = "Este nome de usuário já esta em uso." });
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Username = normalized,
            PasswordHash = string.Empty
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { token = CreateJwtToken(user.Username) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var normalized = (request.Username ?? string.Empty).Trim().ToLowerInvariant();
        var user = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == normalized, cancellationToken);

        if (user is null)
        {
            return Unauthorized(new { message = "Credenciais invalidas." });
        }

        var password = request.Password ?? string.Empty;
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Credenciais invalidas." });
        }

        return Ok(new { token = CreateJwtToken(user.Username) });
    }

    private string CreateJwtToken(string username)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var key = jwtSection["Key"]!;

        var claims = new[] { new Claim(ClaimTypes.Name, username) };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
