using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class RelatorioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RelatorioController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/Relatorio/unidades
    [HttpGet("unidades")]
    public async Task<ActionResult> RelatorioUnidades()
    {
        var unidades = await _context.Unidades
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Cnpj,
                u.Cidade,
                u.Estado,
                u.NomeFranqueado,
                u.NomeResponsavel,
                u.Ativo
            })
            .OrderBy(u => u.Nome)
            .ToListAsync();

        return Ok(unidades);
    }

    // GET /api/Relatorio/estoque
    [HttpGet("estoque")]
    public async Task<ActionResult> RelatorioEstoque()
    {
        var estoque = await _context.Estoques
            .Include(e => e.UnidadeFranqueada)
            .Include(e => e.ProdutoServico)
            .AsNoTracking()
            .Select(e => new
            {
                unidade = e.UnidadeFranqueada!.Nome,
                produto = e.ProdutoServico!.Nome,
                quantidade = e.Quantidade,
                estoqueMinimo = e.EstoqueMinimo,
                estoqueBaixo = e.Quantidade <= e.EstoqueMinimo
            })
            .OrderBy(e => e.unidade)
            .ThenBy(e => e.produto)
            .ToListAsync();

        return Ok(estoque);
    }

    // GET /api/Relatorio/vendas
    [HttpGet("vendas")]
    public async Task<ActionResult> RelatorioVendas(
        DateTime? inicio,
        DateTime? fim,
        int? unidadeId)
    {
        var query = _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .AsNoTracking()
            .AsQueryable();

        if (inicio.HasValue)
        {
            query = query.Where(v => v.DataVenda >= inicio.Value);
        }

        if (fim.HasValue)
        {
            var fimDia = fim.Value.Date.AddDays(1);
            query = query.Where(v => v.DataVenda < fimDia);
        }

        if (unidadeId.HasValue)
        {
            query = query.Where(v =>
                v.UnidadeFranqueadaId == unidadeId.Value);
        }

        var vendas = await query
            .OrderByDescending(v => v.DataVenda)
            .Select(v => new
            {
                v.Id,
                unidade = v.UnidadeFranqueada!.Nome,
                data = v.DataVenda,
                v.Status,
                v.ValorTotal
            })
            .ToListAsync();

        return Ok(vendas);
    }

    // GET /api/Relatorio/faturamento
    [HttpGet("faturamento")]
    public async Task<ActionResult> RelatorioFaturamento(
        DateTime? inicio,
        DateTime? fim)
    {
        var query = _context.Vendas
            .Where(v => v.Status == "CONFIRMADA")
            .AsNoTracking()
            .AsQueryable();

        if (inicio.HasValue)
        {
            query = query.Where(v => v.DataVenda >= inicio.Value);
        }

        if (fim.HasValue)
        {
            var fimDia = fim.Value.Date.AddDays(1);
            query = query.Where(v => v.DataVenda < fimDia);
        }

        var faturamento = await query
            .GroupBy(v => new
            {
                v.UnidadeFranqueadaId,
                NomeUnidade = v.UnidadeFranqueada!.Nome
            })
            .Select(g => new
            {
                unidadeId = g.Key.UnidadeFranqueadaId,
                unidade = g.Key.NomeUnidade,
                faturamento = g.Sum(v => v.ValorTotal),
                quantidadeVendas = g.Count()
            })
            .OrderByDescending(x => x.faturamento)
            .ToListAsync();

        return Ok(faturamento);
    }

    // GET /api/Relatorio/produtos-mais-vendidos
    [HttpGet("produtos-mais-vendidos")]
    public async Task<ActionResult> ProdutosMaisVendidos(
        DateTime? inicio,
        DateTime? fim)
    {
        var query = _context.ItensVenda
            .Include(i => i.Venda)
            .Include(i => i.ProdutoServico)
            .Where(i => i.Venda!.Status == "CONFIRMADA")
            .AsNoTracking()
            .AsQueryable();

        if (inicio.HasValue)
        {
            query = query.Where(i =>
                i.Venda!.DataVenda >= inicio.Value);
        }

        if (fim.HasValue)
        {
            var fimDia = fim.Value.Date.AddDays(1);

            query = query.Where(i =>
                i.Venda!.DataVenda < fimDia);
        }

        var produtos = await query
            .GroupBy(i => new
            {
                i.ProdutoServicoId,
                NomeProduto = i.ProdutoServico!.Nome
            })
            .Select(g => new
            {
                produtoId = g.Key.ProdutoServicoId,
                produto = g.Key.NomeProduto,
                quantidadeVendida = g.Sum(i => i.Quantidade),
                faturamento = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(x => x.quantidadeVendida)
            .ToListAsync();

        return Ok(produtos);
    }

    // GET /api/Relatorio/chamados-abertos
    [HttpGet("chamados-abertos")]
    public async Task<ActionResult> ChamadosAbertos()
    {
        var chamados = await _context.Chamados
            .Include(c => c.UnidadeFranqueada)
            .Where(c => c.Status == "ABERTO")
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                unidade = c.UnidadeFranqueada!.Nome,
                c.Categoria,
                c.Prioridade,
                c.Descricao,
                c.Status,
                c.DataAbertura
            })
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();

        return Ok(chamados);
    }
}