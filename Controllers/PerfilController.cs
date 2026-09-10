using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class PerfilController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PerfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Perfil>>> GetPerfis()
    {
        var perfis = await _context.Perfis
            .AsNoTracking()
            .ToListAsync();

        return Ok(perfis);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Perfil>> GetPerfil(int id)
    {
        var perfil = await _context.Perfis
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (perfil == null)
        {
            return NotFound(new
            {
                mensagem = "Perfil não encontrado."
            });
        }

        return Ok(perfil);
    }

    [HttpPost]
    public async Task<ActionResult<Perfil>> CriarPerfil(Perfil perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome do perfil é obrigatório."
            });
        }

        var existe = await _context.Perfis
            .AnyAsync(p => p.Nome.ToLower() == perfil.Nome.ToLower());

        if (existe)
        {
            return Conflict(new
            {
                mensagem = "Já existe um perfil com esse nome."
            });
        }

        _context.Perfis.Add(perfil);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPerfil),
            new { id = perfil.Id },
            perfil
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPerfil(
        int id,
        Perfil perfil)
    {
        if (id != perfil.Id)
        {
            return BadRequest(new
            {
                mensagem = "O ID da URL é diferente do ID enviado."
            });
        }

        var perfilExistente = await _context.Perfis
            .FirstOrDefaultAsync(p => p.Id == id);

        if (perfilExistente == null)
        {
            return NotFound(new
            {
                mensagem = "Perfil não encontrado."
            });
        }

        if (string.IsNullOrWhiteSpace(perfil.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome do perfil é obrigatório."
            });
        }

        perfilExistente.Nome = perfil.Nome;
        perfilExistente.Ativo = perfil.Ativo;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPerfil(int id)
    {
        var perfil = await _context.Perfis
            .FirstOrDefaultAsync(p => p.Id == id);

        if (perfil == null)
        {
            return NotFound(new
            {
                mensagem = "Perfil não encontrado."
            });
        }

        perfil.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}