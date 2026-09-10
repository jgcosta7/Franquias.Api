using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Perfil)
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Email,
                u.Ativo,
                u.PerfilId,
                Perfil = u.Perfil!.Nome,
                u.DataCadastro
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Email,
                u.Ativo,
                u.PerfilId,
                Perfil = u.Perfil!.Nome,
                u.DataCadastro
            })
            .FirstOrDefaultAsync();

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> CriarUsuario(UsuarioCadastroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new
            {
                mensagem = "Nome, e-mail e senha são obrigatórios."
            });
        }

        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe um usuário com esse e-mail."
            });
        }

        var perfilExiste = await _context.Perfis
            .AnyAsync(p => p.Id == dto.PerfilId && p.Ativo);

        if (!perfilExiste)
        {
            return BadRequest(new
            {
                mensagem = "Perfil inválido ou inativo."
            });
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email.ToLower(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            PerfilId = dto.PerfilId,
            Ativo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUsuario),
            new { id = usuario.Id },
            new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.PerfilId,
                usuario.Ativo
            }
        );
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> DesativarUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        usuario.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}