using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class RoyaltyController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoyaltyController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET - Listar todos os royalties
    [HttpGet]
    public async Task<ActionResult> GetRoyalties()
    {
        var royalties = await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .AsNoTracking()
            .Select(r => new
            {
                r.Id,
                UnidadeId = r.UnidadeFranqueadaId,
                Unidade = r.UnidadeFranqueada!.Nome,
                r.Percentual,
                r.FaturamentoPeriodo,
                r.ValorRoyalty,
                r.InicioPeriodo,
                r.FimPeriodo,
                r.StatusPagamento,
                r.DataPagamento
            })
            .OrderByDescending(r => r.InicioPeriodo)
            .ToListAsync();

        return Ok(royalties);
    }

    // GET - Buscar royalty por ID
    [HttpGet("{id}")]
    public async Task<ActionResult> GetRoyalty(int id)
    {
        var royalty = await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                UnidadeId = r.UnidadeFranqueadaId,
                Unidade = r.UnidadeFranqueada!.Nome,
                r.Percentual,
                r.FaturamentoPeriodo,
                r.ValorRoyalty,
                r.InicioPeriodo,
                r.FimPeriodo,
                r.StatusPagamento,
                r.DataPagamento
            })
            .FirstOrDefaultAsync();

        if (royalty == null)
        {
            return NotFound(new
            {
                mensagem = "Royalty não encontrado."
            });
        }

        return Ok(royalty);
    }

    // POST - Calcular e registrar royalty
    [HttpPost("calcular")]
    public async Task<ActionResult> CalcularRoyalty(
        int unidadeFranqueadaId,
        decimal percentual,
        DateTime inicioPeriodo,
        DateTime fimPeriodo)
    {
        // Validações
        if (percentual <= 0 || percentual > 100)
        {
            return BadRequest(new
            {
                mensagem = "O percentual deve estar entre 0 e 100."
            });
        }

        if (inicioPeriodo > fimPeriodo)
        {
            return BadRequest(new
            {
                mensagem = "A data inicial não pode ser maior que a data final."
            });
        }

        // Verifica se a unidade existe e está ativa
        var unidade = await _context.Unidades
            .FirstOrDefaultAsync(u =>
                u.Id == unidadeFranqueadaId);

        if (unidade == null)
        {
            return NotFound(new
            {
                mensagem = "Unidade franqueada não encontrada."
            });
        }

        if (!unidade.Ativo)
        {
            return BadRequest(new
            {
                mensagem = "Não é possível calcular royalty para uma unidade inativa."
            });
        }

        // Considera somente vendas confirmadas dentro do período
        var fimPeriodoConsulta = fimPeriodo.Date.AddDays(1);

        var faturamento = await _context.Vendas
            .Where(v =>
                v.UnidadeFranqueadaId == unidadeFranqueadaId &&
                v.Status == "CONFIRMADA" &&
                v.DataVenda >= inicioPeriodo &&
                v.DataVenda < fimPeriodoConsulta)
            .SumAsync(v => (decimal?)v.ValorTotal) ?? 0;

        // Calcula o valor do royalty
        var valorRoyalty = faturamento * percentual / 100;

        // Cria o registro
        var royalty = new Royalty
        {
            UnidadeFranqueadaId = unidadeFranqueadaId,
            Percentual = percentual,
            FaturamentoPeriodo = faturamento,
            ValorRoyalty = valorRoyalty,
            InicioPeriodo = inicioPeriodo,
            FimPeriodo = fimPeriodo,
            StatusPagamento = "PENDENTE"
        };

        _context.Royalties.Add(royalty);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRoyalty),
            new { id = royalty.Id },
            new
            {
                royalty.Id,
                Unidade = unidade.Nome,
                royalty.Percentual,
                royalty.FaturamentoPeriodo,
                royalty.ValorRoyalty,
                royalty.InicioPeriodo,
                royalty.FimPeriodo,
                royalty.StatusPagamento
            }
        );
    }

    // PUT - Marcar royalty como pago
    [HttpPut("{id}/pagar")]
    public async Task<ActionResult> PagarRoyalty(int id)
    {
        var royalty = await _context.Royalties
            .FirstOrDefaultAsync(r => r.Id == id);

        if (royalty == null)
        {
            return NotFound(new
            {
                mensagem = "Royalty não encontrado."
            });
        }

        if (royalty.StatusPagamento == "PAGO")
        {
            return BadRequest(new
            {
                mensagem = "Este royalty já está marcado como pago."
            });
        }

        royalty.StatusPagamento = "PAGO";
        royalty.DataPagamento = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Royalty marcado como pago.",
            royalty.Id,
            royalty.StatusPagamento,
            royalty.DataPagamento
        });
    }
}