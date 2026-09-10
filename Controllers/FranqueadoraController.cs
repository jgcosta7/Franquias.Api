using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class FranqueadoraController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FranqueadoraController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetFranqueadoras()
    {
        var franqueadoras = await _context.Franqueadoras
            .AsNoTracking()
            .ToListAsync();

        return Ok(franqueadoras);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetFranqueadora(int id)
    {
        var franqueadora = await _context.Franqueadoras
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        if (franqueadora == null)
            return NotFound(new { mensagem = "Franqueadora não encontrada." });

        return Ok(franqueadora);
    }

    [HttpPost]
    public async Task<ActionResult> CriarFranqueadora(Franqueadora franqueadora)
    {
        if (string.IsNullOrWhiteSpace(franqueadora.Nome) ||
            string.IsNullOrWhiteSpace(franqueadora.Cnpj))
        {
            return BadRequest(new
            {
                mensagem = "Nome e CNPJ são obrigatórios."
            });
        }

        var existe = await _context.Franqueadoras
            .AnyAsync(f => f.Cnpj == franqueadora.Cnpj);

        if (existe)
            return Conflict(new { mensagem = "CNPJ já cadastrado." });

        _context.Franqueadoras.Add(franqueadora);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFranqueadora),
            new { id = franqueadora.Id },
            franqueadora
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarFranqueadora(
        int id,
        Franqueadora dados)
    {
        var franqueadora = await _context.Franqueadoras
            .FirstOrDefaultAsync(f => f.Id == id);

        if (franqueadora == null)
            return NotFound(new { mensagem = "Franqueadora não encontrada." });

        franqueadora.Nome = dados.Nome;
        franqueadora.Cnpj = dados.Cnpj;
        franqueadora.Email = dados.Email;
        franqueadora.Telefone = dados.Telefone;
        franqueadora.Ativo = dados.Ativo;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DesativarFranqueadora(int id)
    {
        var franqueadora = await _context.Franqueadoras
            .FirstOrDefaultAsync(f => f.Id == id);

        if (franqueadora == null)
            return NotFound(new { mensagem = "Franqueadora não encontrada." });

        franqueadora.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}