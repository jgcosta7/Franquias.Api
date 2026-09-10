using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class ProdutoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProdutoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Produto
    // Exemplos:
    // /api/Produto
    // /api/Produto?nome=Notebook
    // /api/Produto?categoriaId=1
    // /api/Produto?ativo=true
    [HttpGet]
public async Task<ActionResult> GetProdutos(
    string? nome,
    int? categoriaId,
    bool? ativo,
    int pagina = 1,
    int tamanhoPagina = 10,
    string? ordenarPor = "nome")
    {
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .AsQueryable();

        // Filtro por nome
        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p =>
                p.Nome.ToLower().Contains(nome.ToLower()));
        }

        // Filtro por categoria
        if (categoriaId.HasValue)
        {
            query = query.Where(p =>
                p.CategoriaId == categoriaId.Value);
        }

        // Filtro por status
        if (ativo.HasValue)
        {
            query = query.Where(p =>
                p.Ativo == ativo.Value);
        }

        if (pagina < 1)
{
    pagina = 1;
}

if (tamanhoPagina < 1 || tamanhoPagina > 100)
{
    tamanhoPagina = 10;
}

if (ordenarPor?.ToLower() == "preco")
{
    query = query.OrderBy(p => p.PrecoBase);
}
else
{
    query = query.OrderBy(p => p.Nome);
}

var total = await query.CountAsync();

var produtos = await query
    .Skip((pagina - 1) * tamanhoPagina)
    .Take(tamanhoPagina)
    .Select(p => new
    {
        p.Id,
        p.Nome,
        p.Descricao,
        p.PrecoBase,
        p.Ativo,
        p.CategoriaId,
        Categoria = p.Categoria!.Nome
    })
    .ToListAsync();

return Ok(new
{
    pagina,
    tamanhoPagina,
    total,
    produtos
});
    }

    // GET: api/Produto/1
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduto(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Nome,
                p.Descricao,
                p.PrecoBase,
                p.Ativo,
                p.CategoriaId,
                Categoria = p.Categoria!.Nome
            })
            .FirstOrDefaultAsync();

        if (produto == null)
        {
            return NotFound(new
            {
                mensagem = "Produto/serviço não encontrado."
            });
        }

        return Ok(produto);
    }

    // POST: api/Produto
    [HttpPost]
    public async Task<ActionResult> CriarProduto(
        ProdutoServico produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Nome))
        {
            return BadRequest(new
            {
                mensagem = "Nome do produto/serviço é obrigatório."
            });
        }

        if (produto.PrecoBase <= 0)
        {
            return BadRequest(new
            {
                mensagem = "O preço deve ser maior que zero."
            });
        }

        // Verifica se a categoria existe e está ativa
        var categoriaExiste = await _context.Categorias
            .AnyAsync(c =>
                c.Id == produto.CategoriaId &&
                c.Ativo);

        if (!categoriaExiste)
        {
            return BadRequest(new
            {
                mensagem = "Categoria inválida ou inativa."
            });
        }

        produto.Id = 0;
        produto.Ativo = true;

        _context.Produtos.Add(produto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduto),
            new { id = produto.Id },
            new
            {
                produto.Id,
                produto.Nome,
                produto.Descricao,
                produto.PrecoBase,
                produto.Ativo,
                produto.CategoriaId
            }
        );
    }

    // PUT: api/Produto/1
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarProduto(
        int id,
        ProdutoServico dados)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
        {
            return NotFound(new
            {
                mensagem = "Produto/serviço não encontrado."
            });
        }

        if (string.IsNullOrWhiteSpace(dados.Nome))
        {
            return BadRequest(new
            {
                mensagem = "Nome do produto/serviço é obrigatório."
            });
        }

        if (dados.PrecoBase <= 0)
        {
            return BadRequest(new
            {
                mensagem = "O preço deve ser maior que zero."
            });
        }

        // Verifica se a categoria existe e está ativa
        var categoriaExiste = await _context.Categorias
            .AnyAsync(c =>
                c.Id == dados.CategoriaId &&
                c.Ativo);

        if (!categoriaExiste)
        {
            return BadRequest(new
            {
                mensagem = "Categoria inválida ou inativa."
            });
        }

        produto.Nome = dados.Nome;
        produto.Descricao = dados.Descricao;
        produto.PrecoBase = dados.PrecoBase;
        produto.CategoriaId = dados.CategoriaId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Produto/serviço atualizado com sucesso.",
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.PrecoBase,
            produto.Ativo,
            produto.CategoriaId
        });
    }

    // DELETE: api/Produto/1
    [HttpDelete("{id}")]
    public async Task<ActionResult> DesativarProduto(int id)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
        {
            return NotFound(new
            {
                mensagem = "Produto/serviço não encontrado."
            });
        }

        // Soft delete
        produto.Ativo = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}