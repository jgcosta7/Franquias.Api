using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente,Operador")]
public class EstoqueController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EstoqueController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetEstoques()
    {
        var estoques = await _context.Estoques
            .Include(e => e.ProdutoServico)
            .Include(e => e.UnidadeFranqueada)
            .AsNoTracking()
            .Select(e => new
            {
                e.Id,
                Unidade = e.UnidadeFranqueada!.Nome,
                Produto = e.ProdutoServico!.Nome,
                e.Quantidade,
                e.EstoqueMinimo
            })
            .ToListAsync();

        return Ok(estoques);
    }

    [HttpPost]
    public async Task<ActionResult> CriarEstoque(Estoque estoque)
    {
        if (estoque.Quantidade < 0)
            return BadRequest(new
            {
                mensagem = "A quantidade não pode ser negativa."
            });

        var unidadeExiste = await _context.Unidades
            .AnyAsync(u => u.Id == estoque.UnidadeFranqueadaId && u.Ativo);

        if (!unidadeExiste)
            return BadRequest(new
            {
                mensagem = "Unidade inválida ou inativa."
            });

        var produtoExiste = await _context.Produtos
            .AnyAsync(p => p.Id == estoque.ProdutoServicoId && p.Ativo);

        if (!produtoExiste)
            return BadRequest(new
            {
                mensagem = "Produto inválido ou inativo."
            });

        var estoqueExiste = await _context.Estoques
            .AnyAsync(e =>
                e.UnidadeFranqueadaId == estoque.UnidadeFranqueadaId &&
                e.ProdutoServicoId == estoque.ProdutoServicoId);

        if (estoqueExiste)
            return Conflict(new
            {
                mensagem = "Já existe estoque desse produto para essa unidade."
            });

        _context.Estoques.Add(estoque);
        await _context.SaveChangesAsync();

        return Created("", estoque);
    }

    [HttpPost("movimentar")]
    public async Task<ActionResult> MovimentarEstoque(
        MovimentacaoEstoqueDto dto)
    {
        if (dto.Quantidade <= 0)
            return BadRequest(new
            {
                mensagem = "A quantidade deve ser maior que zero."
            });

        var estoque = await _context.Estoques
            .FirstOrDefaultAsync(e => e.Id == dto.EstoqueId);

        if (estoque == null)
            return NotFound(new
            {
                mensagem = "Estoque não encontrado."
            });

        var tipo = dto.Tipo.ToUpper();

        if (tipo != "ENTRADA" && tipo != "SAIDA")
            return BadRequest(new
            {
                mensagem = "Tipo deve ser ENTRADA ou SAIDA."
            });

        if (tipo == "SAIDA" && estoque.Quantidade < dto.Quantidade)
            return BadRequest(new
            {
                mensagem = "Estoque insuficiente. A operação deixaria o estoque negativo."
            });

        if (tipo == "ENTRADA")
            estoque.Quantidade += dto.Quantidade;
        else
            estoque.Quantidade -= dto.Quantidade;

        var movimentacao = new MovimentacaoEstoque
        {
            EstoqueId = estoque.Id,
            Tipo = tipo,
            Quantidade = dto.Quantidade,
            Observacao = dto.Observacao
        };

        _context.MovimentacoesEstoque.Add(movimentacao);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Estoque atualizado com sucesso.",
            estoque.Id,
            estoque.Quantidade
        });
    }
}