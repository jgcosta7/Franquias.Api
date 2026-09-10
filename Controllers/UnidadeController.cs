using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class UnidadeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UnidadeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Unidade
    // Exemplos:
    // /api/Unidade
    // /api/Unidade?nome=São Paulo
    // /api/Unidade?cidade=São Paulo
    // /api/Unidade?estado=SP
    // /api/Unidade?ativo=true
    // /api/Unidade?nome=São&cidade=São Paulo&ativo=true
    [HttpGet]
public async Task<ActionResult> GetUnidades(
    string? nome,
    string? cidade,
    string? estado,
    string? cnpj,
    string? responsavel,
    bool? ativo)
    {
        var query = _context.Unidades
            .Include(u => u.Franqueadora)
            .AsNoTracking()
            .AsQueryable();

        // Filtro por nome
        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(u =>
                u.Nome.ToLower().Contains(nome.ToLower()));
        }

        // Filtro por cidade
        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(u =>
                u.Cidade.ToLower().Contains(cidade.ToLower()));
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(u =>
                u.Estado.ToLower() == estado.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(cnpj))
{
    query = query.Where(u =>
        u.Cnpj.Contains(cnpj));
}

if (!string.IsNullOrWhiteSpace(responsavel))
{
    query = query.Where(u =>
        u.NomeResponsavel.ToLower().Contains(responsavel.ToLower()));
}

        // Filtro por status
        if (ativo.HasValue)
        {
            query = query.Where(u =>
                u.Ativo == ativo.Value);
        }

        var unidades = await query
            .OrderBy(u => u.Nome)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Cnpj,
                u.Telefone,
                u.Email,
                u.Endereco,
                u.Cidade,
                u.Estado,
                u.DataInicio,
                u.NomeFranqueado,
                u.NomeResponsavel,
                u.Ativo,
                u.FranqueadoraId,
                Franqueadora = u.Franqueadora!.Nome
            })
            .ToListAsync();

        return Ok(unidades);
    }

    // GET: api/Unidade/1
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUnidade(int id)
    {
        var unidade = await _context.Unidades
            .Include(u => u.Franqueadora)
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Cnpj,
                u.Telefone,
                u.Email,
                u.Endereco,
                u.Cidade,
                u.Estado,
                u.DataInicio,
                u.NomeFranqueado,
                u.NomeResponsavel,
                u.Ativo,
                u.FranqueadoraId,
                Franqueadora = u.Franqueadora!.Nome
            })
            .FirstOrDefaultAsync();

        if (unidade == null)
        {
            return NotFound(new
            {
                mensagem = "Unidade não encontrada."
            });
        }

        return Ok(unidade);
    }

    // POST: api/Unidade
    [HttpPost]
    public async Task<ActionResult> CriarUnidade(
        UnidadeFranqueada unidade)
    {
        if (string.IsNullOrWhiteSpace(unidade.Nome) ||
            string.IsNullOrWhiteSpace(unidade.Cnpj))
        {
            return BadRequest(new
            {
                mensagem = "Nome e CNPJ são obrigatórios."
            });
        }

        // Verifica CNPJ duplicado
        var cnpjExiste = await _context.Unidades
            .AnyAsync(u => u.Cnpj == unidade.Cnpj);

        if (cnpjExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe uma unidade com esse CNPJ."
            });
        }

        // Verifica se a franqueadora existe e está ativa
        var franqueadoraExiste = await _context.Franqueadoras
            .AnyAsync(f =>
                f.Id == unidade.FranqueadoraId &&
                f.Ativo);

        if (!franqueadoraExiste)
        {
            return BadRequest(new
            {
                mensagem = "Franqueadora inválida ou inativa."
            });
        }

        unidade.Id = 0;
        unidade.Ativo = true;

        _context.Unidades.Add(unidade);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUnidade),
            new { id = unidade.Id },
            new
            {
                unidade.Id,
                unidade.Nome,
                unidade.Cnpj,
                unidade.Telefone,
                unidade.Email,
                unidade.Endereco,
                unidade.Cidade,
                unidade.Estado,
                unidade.DataInicio,
                unidade.NomeFranqueado,
                unidade.NomeResponsavel,
                unidade.Ativo,
                unidade.FranqueadoraId
            }
        );
    }

    // PUT: api/Unidade/1
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarUnidade(
        int id,
        UnidadeFranqueada dados)
    {
        var unidade = await _context.Unidades
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unidade == null)
        {
            return NotFound(new
            {
                mensagem = "Unidade não encontrada."
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

        // Verifica CNPJ duplicado em outra unidade
        var cnpjExiste = await _context.Unidades
            .AnyAsync(u =>
                u.Cnpj == dados.Cnpj &&
                u.Id != id);

        if (cnpjExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe outra unidade com esse CNPJ."
            });
        }

        // Verifica se a franqueadora existe e está ativa
        var franqueadoraExiste = await _context.Franqueadoras
            .AnyAsync(f =>
                f.Id == dados.FranqueadoraId &&
                f.Ativo);

        if (!franqueadoraExiste)
        {
            return BadRequest(new
            {
                mensagem = "Franqueadora inválida ou inativa."
            });
        }

        unidade.Nome = dados.Nome;
        unidade.Cnpj = dados.Cnpj;
        unidade.Telefone = dados.Telefone;
        unidade.Email = dados.Email;
        unidade.Endereco = dados.Endereco;
        unidade.Cidade = dados.Cidade;
        unidade.Estado = dados.Estado;
        unidade.DataInicio = dados.DataInicio;
        unidade.NomeFranqueado = dados.NomeFranqueado;
        unidade.NomeResponsavel = dados.NomeResponsavel;
        unidade.FranqueadoraId = dados.FranqueadoraId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Unidade atualizada com sucesso.",
            unidade.Id,
            unidade.Nome,
            unidade.Cnpj,
            unidade.Telefone,
            unidade.Email,
            unidade.Endereco,
            unidade.Cidade,
            unidade.Estado,
            unidade.DataInicio,
            unidade.NomeFranqueado,
            unidade.NomeResponsavel,
            unidade.Ativo,
            unidade.FranqueadoraId
        });
    }

    // DELETE: api/Unidade/1
    [HttpDelete("{id}")]
    public async Task<ActionResult> DesativarUnidade(int id)
    {
        var unidade = await _context.Unidades
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unidade == null)
        {
            return NotFound(new
            {
                mensagem = "Unidade não encontrada."
            });
        }

        // Soft delete
        unidade.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}