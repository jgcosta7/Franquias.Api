using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente,Operador")]
public class ChamadoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ChamadoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetChamados(
        string? prioridade,
        int? unidadeId,
        string? status)
    {
        var query = _context.Chamados
            .Include(c => c.UnidadeFranqueada)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(prioridade))
        {
            query = query.Where(c =>
                c.Prioridade.ToLower() == prioridade.ToLower());
        }

        if (unidadeId.HasValue)
        {
            query = query.Where(c =>
                c.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c =>
                c.Status.ToLower() == status.ToLower());
        }

        var chamados = await query
            .OrderByDescending(c => c.DataAbertura)
            .Select(c => new
            {
                c.Id,
                Unidade = c.UnidadeFranqueada!.Nome,
                c.Categoria,
                c.Prioridade,
                c.Descricao,
                c.Status,
                c.DataAbertura,
                c.DataEncerramento,
                c.ObservacaoEncerramento
            })
            .ToListAsync();

        return Ok(chamados);
    }

    [HttpPost]
    public async Task<ActionResult> CriarChamado(ChamadoSuporte chamado)
    {
        if (string.IsNullOrWhiteSpace(chamado.Descricao))
        {
            return BadRequest(new
            {
                mensagem = "A descrição do chamado é obrigatória."
            });
        }

        var unidadeExiste = await _context.Unidades
            .AnyAsync(u =>
                u.Id == chamado.UnidadeFranqueadaId &&
                u.Ativo);

        if (!unidadeExiste)
        {
            return BadRequest(new
            {
                mensagem = "Unidade inválida ou inativa."
            });
        }

        chamado.Id = 0;
        chamado.Status = "ABERTO";
        chamado.DataAbertura = DateTime.UtcNow;

        _context.Chamados.Add(chamado);

        await _context.SaveChangesAsync();

        return Created("", chamado);
    }

    [HttpPut("{id}/encerrar")]
    public async Task<IActionResult> EncerrarChamado(
        int id,
        string observacao = "")
    {
        var chamado = await _context.Chamados
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chamado == null)
        {
            return NotFound(new
            {
                mensagem = "Chamado não encontrado."
            });
        }

        chamado.Status = "FECHADO";
        chamado.DataEncerramento = DateTime.UtcNow;
        chamado.ObservacaoEncerramento = observacao;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Chamado encerrado com sucesso."
        });
    }
}