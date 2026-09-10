using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetCategorias()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpPost]
    public async Task<ActionResult> CriarCategoria(Categoria categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Nome))
            return BadRequest(new { mensagem = "O nome é obrigatório." });

        var existe = await _context.Categorias
            .AnyAsync(c => c.Nome.ToLower() == categoria.Nome.ToLower());

        if (existe)
            return Conflict(new { mensagem = "Categoria já cadastrada." });

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return Created("", categoria);
    }
}