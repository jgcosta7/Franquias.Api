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
public class VendaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VendaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetVendas()
    {
        var vendas = await _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .AsNoTracking()
            .Select(v => new
            {
                v.Id,
                Unidade = v.UnidadeFranqueada!.Nome,
                v.DataVenda,
                v.ValorTotal,
                v.Status,
                Itens = v.Itens.Select(i => new
                {
                    i.ProdutoServicoId,
                    Produto = i.ProdutoServico!.Nome,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.Subtotal
                })
            })
            .ToListAsync();

        return Ok(vendas);
    }

    [HttpPost]
    public async Task<ActionResult> CriarVenda(VendaDto dto)
    {
        // Regra: venda precisa ter pelo menos um item
        if (dto.Itens == null || dto.Itens.Count == 0)
        {
            return BadRequest(new
            {
                mensagem = "A venda precisa ter pelo menos um item."
            });
        }

        // Regra: unidade precisa estar ativa
        var unidade = await _context.Unidades
            .FirstOrDefaultAsync(u =>
                u.Id == dto.UnidadeFranqueadaId);

        if (unidade == null)
        {
            return NotFound(new
            {
                mensagem = "Unidade não encontrada."
            });
        }

        if (!unidade.Ativo)
        {
            return BadRequest(new
            {
                mensagem = "Não é possível realizar venda em uma unidade inativa."
            });
        }

        var venda = new Venda
        {
            UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
            DataVenda = DateTime.UtcNow,
            Status = "CONFIRMADA"
        };

        decimal total = 0;

        foreach (var itemDto in dto.Itens)
        {
            if (itemDto.Quantidade <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p =>
                    p.Id == itemDto.ProdutoServicoId &&
                    p.Ativo);

            if (produto == null)
            {
                return BadRequest(new
                {
                    mensagem = $"Produto {itemDto.ProdutoServicoId} inválido ou inativo."
                });
            }

            var estoque = await _context.Estoques
                .FirstOrDefaultAsync(e =>
                    e.UnidadeFranqueadaId == dto.UnidadeFranqueadaId &&
                    e.ProdutoServicoId == itemDto.ProdutoServicoId);

            if (estoque == null)
            {
                return BadRequest(new
                {
                    mensagem = $"Produto {produto.Nome} não possui estoque cadastrado para essa unidade."
                });
            }

            // Regra: estoque não pode ficar negativo
            if (estoque.Quantidade < itemDto.Quantidade)
            {
                return BadRequest(new
                {
                    mensagem = $"Estoque insuficiente para o produto {produto.Nome}."
                });
            }

            var subtotal = produto.PrecoBase * itemDto.Quantidade;

            var item = new ItemVenda
            {
                ProdutoServicoId = produto.Id,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = produto.PrecoBase,
                Subtotal = subtotal
            };

            venda.Itens.Add(item);

            estoque.Quantidade -= itemDto.Quantidade;

            total += subtotal;
        }

        venda.ValorTotal = total;

        _context.Vendas.Add(venda);

        await _context.SaveChangesAsync();

        return Created("", new
        {
            venda.Id,
            venda.UnidadeFranqueadaId,
            venda.DataVenda,
            venda.Status,
            venda.ValorTotal,
            Itens = venda.Itens.Select(i => new
            {
                i.ProdutoServicoId,
                i.Quantidade,
                i.PrecoUnitario,
                i.Subtotal
            })
        });
    }
}