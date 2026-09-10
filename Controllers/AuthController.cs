using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u =>
                u.Email == dto.Email.ToLower());

        if (usuario == null || !usuario.Ativo)
        {
            return Unauthorized(new
            {
                mensagem = "E-mail ou senha inválidos."
            });
        }

        var senhaCorreta = BCrypt.Net.BCrypt.Verify(
            dto.Senha,
            usuario.SenhaHash);

        if (!senhaCorreta)
        {
            return Unauthorized(new
            {
                mensagem = "E-mail ou senha inválidos."
            });
        }

        var chave = "chave-secreta-franquias-2026-seguranca";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil!.Nome)
        };

        var chaveSeguranca = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(chave));

        var credenciais = new SigningCredentials(
            chaveSeguranca,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: credenciais);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler()
                .WriteToken(token),
            usuario = usuario.Nome,
            perfil = usuario.Perfil!.Nome
        });
    }
}