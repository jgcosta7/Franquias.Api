using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class FornecedorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FornecedorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET - Listar fornecedores

    [HttpGet]
public async Task<ActionResult> GetFornecedores(
    string? nome,
    string? cnpj,
    bool? ativo)

    {
        var query = _context.Fornecedores
    .AsNoTracking()
    .AsQueryable();

if (!string.IsNullOrWhiteSpace(nome))
{
    query = query.Where(f =>
        f.Nome.ToLower().Contains(nome.ToLower()));
}

if (!string.IsNullOrWhiteSpace(cnpj))
{
    query = query.Where(f =>
        f.Cnpj.Contains(cnpj));
}

if (ativo.HasValue)
{
    query = query.Where(f =>
        f.Ativo == ativo.Value);
}

var fornecedores = await query

            .Select(f => new
            {
                f.Id,
                f.Nome,
                f.Cnpj,
                f.Telefone,
                f.Email,
                f.Ativo
            })
            .ToListAsync();

        return Ok(fornecedores);
    }

    // GET - Buscar fornecedor por ID
    [HttpGet("{id}")]
    public async Task<ActionResult> GetFornecedor(int id)
    {
        var fornecedor = await _context.Fornecedores
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new
            {
                f.Id,
                f.Nome,
                f.Cnpj,
                f.Telefone,
                f.Email,
                f.Ativo
            })
            .FirstOrDefaultAsync();

        if (fornecedor == null)
        {
            return NotFound(new
            {
                mensagem = "Fornecedor não encontrado."
            });
        }

        return Ok(fornecedor);
    }

    // POST - Criar fornecedor
    [HttpPost]
    public async Task<ActionResult> CriarFornecedor(Fornecedor fornecedor)
    {
        if (string.IsNullOrWhiteSpace(fornecedor.Nome) ||
            string.IsNullOrWhiteSpace(fornecedor.Cnpj))
        {
            return BadRequest(new
            {
                mensagem = "Nome e CNPJ são obrigatórios."
            });
        }

        var cnpjExiste = await _context.Fornecedores
            .AnyAsync(f => f.Cnpj == fornecedor.Cnpj);

        if (cnpjExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe um fornecedor com esse CNPJ."
            });
        }

        fornecedor.Id = 0;
        fornecedor.Ativo = true;

        _context.Fornecedores.Add(fornecedor);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFornecedor),
            new { id = fornecedor.Id },
            new
            {
                fornecedor.Id,
                fornecedor.Nome,
                fornecedor.Cnpj,
                fornecedor.Telefone,
                fornecedor.Email,
                fornecedor.Ativo
            }
        );
    }

    // PUT - Atualizar fornecedor
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarFornecedor(
        int id,
        Fornecedor dados)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fornecedor == null)
        {
            return NotFound(new
            {
                mensagem = "Fornecedor não encontrado."
            });
        }

        if (string.IsNullOrWhiteSpace(dados.Nome) ||
            string.IsNullOrWhiteSpace(dados.Cnpj))
        {
            return BadRequest(new
            {
                mensagem = "Nome e CNPJ são obrigatórios."
            });
        }

        var cnpjExiste = await _context.Fornecedores
            .AnyAsync(f =>
                f.Cnpj == dados.Cnpj &&
                f.Id != id);

        if (cnpjExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe outro fornecedor com esse CNPJ."
            });
        }

        fornecedor.Nome = dados.Nome;
        fornecedor.Cnpj = dados.Cnpj;
        fornecedor.Telefone = dados.Telefone;
        fornecedor.Email = dados.Email;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Fornecedor atualizado com sucesso.",
            fornecedor.Id,
            fornecedor.Nome,
            fornecedor.Cnpj,
            fornecedor.Telefone,
            fornecedor.Email,
            fornecedor.Ativo
        });
    }

    // DELETE - Desativar fornecedor
    [HttpDelete("{id}")]
    public async Task<ActionResult> DesativarFornecedor(int id)
    {
        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fornecedor == null)
        {
            return NotFound(new
            {
                mensagem = "Fornecedor não encontrado."
            });
        }

        fornecedor.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}