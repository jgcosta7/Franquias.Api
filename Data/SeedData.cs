using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

public static class SeedData
{
    public static async Task InicializarAsync(ApplicationDbContext context)
    {
        // Se já houver usuários, considera que o banco já foi preenchido
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // =========================
        // PERFIS
        // =========================

        var administrador = new Perfil
        {
            Nome = "Administrador",
            Ativo = true
        };

        var gerente = new Perfil
        {
            Nome = "Gerente",
            Ativo = true
        };

        context.Perfis.AddRange(administrador, gerente);
        await context.SaveChangesAsync();

        // =========================
        // USUÁRIOS
        // =========================

        var usuarioAdmin = new Usuario
        {
            Nome = "Administrador",
            Email = "admin@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PerfilId = administrador.Id,
            Ativo = true
        };

        var usuarioGerente = new Usuario
        {
            Nome = "Gerente",
            Email = "gerente@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PerfilId = gerente.Id,
            Ativo = true
        };

        context.Usuarios.AddRange(usuarioAdmin, usuarioGerente);
        await context.SaveChangesAsync();

        // =========================
        // FRANQUEADORA
        // =========================

        var franqueadora = new Franqueadora
        {
            Nome = "Franquias Brasil",
            Cnpj = "12345678000190",
            Email = "contato@franquiasbrasil.com",
            Telefone = "(11) 99999-1111",
            Ativo = true
        };

        context.Franqueadoras.Add(franqueadora);
        await context.SaveChangesAsync();

        // =========================
        // UNIDADES
        // =========================

        var unidadeCentro = new UnidadeFranqueada
        {
            Nome = "Unidade Centro",
            Cnpj = "12345678000271",
            Telefone = "(11) 98888-1111",
            Email = "centro@franquiasbrasil.com",
            Endereco = "Rua Central, 100",
            Cidade = "São Paulo",
            Estado = "SP",
            DataInicio = DateTime.UtcNow.AddYears(-2),
            Ativo = true,
            NomeFranqueado = "Carlos Silva",
            NomeResponsavel = "Carlos Silva",
            FranqueadoraId = franqueadora.Id
        };

        var unidadeNorte = new UnidadeFranqueada
        {
            Nome = "Unidade Norte",
            Cnpj = "12345678000352",
            Telefone = "(11) 97777-2222",
            Email = "norte@franquiasbrasil.com",
            Endereco = "Avenida Norte, 500",
            Cidade = "São Paulo",
            Estado = "SP",
            DataInicio = DateTime.UtcNow.AddYears(-1),
            Ativo = true,
            NomeFranqueado = "Marcos Souza",
            NomeResponsavel = "Marcos Souza",
            FranqueadoraId = franqueadora.Id
        };

        context.Unidades.AddRange(unidadeCentro, unidadeNorte);
        await context.SaveChangesAsync();

        // =========================
        // CATEGORIAS
        // =========================

        var eletronicos = new Categoria
        {
            Nome = "Eletrônicos",
            Ativo = true
        };

        var acessorios = new Categoria
        {
            Nome = "Acessórios",
            Ativo = true
        };

        var servicos = new Categoria
        {
            Nome = "Serviços",
            Ativo = true
        };

        context.Categorias.AddRange(
            eletronicos,
            acessorios,
            servicos
        );

        await context.SaveChangesAsync();

        // =========================
        // PRODUTOS
        // =========================

        var notebook = new ProdutoServico
        {
            Nome = "Notebook",
            Descricao = "Notebook para uso profissional",
            PrecoBase = 3500,
            Ativo = true,
            CategoriaId = eletronicos.Id
        };

        var mouse = new ProdutoServico
        {
            Nome = "Mouse",
            Descricao = "Mouse óptico USB",
            PrecoBase = 80,
            Ativo = true,
            CategoriaId = acessorios.Id
        };

        var teclado = new ProdutoServico
        {
            Nome = "Teclado",
            Descricao = "Teclado USB",
            PrecoBase = 120,
            Ativo = true,
            CategoriaId = acessorios.Id
        };

        var suporte = new ProdutoServico
        {
            Nome = "Suporte Técnico",
            Descricao = "Serviço de suporte técnico",
            PrecoBase = 150,
            Ativo = true,
            CategoriaId = servicos.Id
        };

        context.Produtos.AddRange(
            notebook,
            mouse,
            teclado,
            suporte
        );

        await context.SaveChangesAsync();

        // =========================
        // FORNECEDOR
        // =========================

        var fornecedor = new Fornecedor
        {
            Nome = "Fornecedor Tech",
            Cnpj = "12345678000433",
            Telefone = "(11) 96666-3333",
            Email = "contato@fornecedortech.com",
            Ativo = true
        };

        context.Fornecedores.Add(fornecedor);
        await context.SaveChangesAsync();

        // =========================
        // ESTOQUE
        // =========================

        var estoqueNotebookCentro = new Estoque
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            ProdutoServicoId = notebook.Id,
            Quantidade = 10,
            EstoqueMinimo = 3
        };

        var estoqueMouseCentro = new Estoque
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            ProdutoServicoId = mouse.Id,
            Quantidade = 20,
            EstoqueMinimo = 5
        };

        var estoqueTecladoCentro = new Estoque
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            ProdutoServicoId = teclado.Id,
            Quantidade = 2,
            EstoqueMinimo = 5
        };

        var estoqueNotebookNorte = new Estoque
        {
            UnidadeFranqueadaId = unidadeNorte.Id,
            ProdutoServicoId = notebook.Id,
            Quantidade = 8,
            EstoqueMinimo = 3
        };

        var estoqueMouseNorte = new Estoque
        {
            UnidadeFranqueadaId = unidadeNorte.Id,
            ProdutoServicoId = mouse.Id,
            Quantidade = 15,
            EstoqueMinimo = 5
        };

        context.Estoques.AddRange(
            estoqueNotebookCentro,
            estoqueMouseCentro,
            estoqueTecladoCentro,
            estoqueNotebookNorte,
            estoqueMouseNorte
        );

        await context.SaveChangesAsync();

        // =========================
        // CHAMADOS
        // =========================

        var chamado1 = new ChamadoSuporte
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            Categoria = "Sistema",
            Prioridade = "ALTA",
            Descricao = "Sistema apresentando lentidão.",
            Status = "ABERTO",
            DataAbertura = DateTime.UtcNow.AddDays(-2)
        };

        var chamado2 = new ChamadoSuporte
        {
            UnidadeFranqueadaId = unidadeNorte.Id,
            Categoria = "Rede",
            Prioridade = "MEDIA",
            Descricao = "Problema de conexão com a rede.",
            Status = "ABERTO",
            DataAbertura = DateTime.UtcNow.AddDays(-1)
        };

        context.Chamados.AddRange(chamado1, chamado2);
        await context.SaveChangesAsync();

        // =========================
        // FORNECEDOR x PRODUTOS
        // =========================

        fornecedor.Produtos.Add(notebook);
        fornecedor.Produtos.Add(mouse);
        fornecedor.Produtos.Add(teclado);

        await context.SaveChangesAsync();

        // =========================
        // VENDA 1
        // =========================

        var venda1 = new Venda
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            DataVenda = DateTime.UtcNow.AddDays(-3),
            Status = "CONFIRMADA"
        };

        var item1 = new ItemVenda
        {
            Venda = venda1,
            ProdutoServicoId = notebook.Id,
            Quantidade = 2,
            PrecoUnitario = notebook.PrecoBase,
            Subtotal = notebook.PrecoBase * 2
        };

        var item2 = new ItemVenda
        {
            Venda = venda1,
            ProdutoServicoId = mouse.Id,
            Quantidade = 3,
            PrecoUnitario = mouse.PrecoBase,
            Subtotal = mouse.PrecoBase * 3
        };

        venda1.Itens.Add(item1);
        venda1.Itens.Add(item2);
        venda1.ValorTotal = item1.Subtotal + item2.Subtotal;

        context.Vendas.Add(venda1);
        await context.SaveChangesAsync();

        // =========================
        // VENDA 2
        // =========================

        var venda2 = new Venda
        {
            UnidadeFranqueadaId = unidadeNorte.Id,
            DataVenda = DateTime.UtcNow.AddDays(-1),
            Status = "CONFIRMADA"
        };

        var item3 = new ItemVenda
        {
            Venda = venda2,
            ProdutoServicoId = mouse.Id,
            Quantidade = 5,
            PrecoUnitario = mouse.PrecoBase,
            Subtotal = mouse.PrecoBase * 5
        };

        var item4 = new ItemVenda
        {
            Venda = venda2,
            ProdutoServicoId = teclado.Id,
            Quantidade = 2,
            PrecoUnitario = teclado.PrecoBase,
            Subtotal = teclado.PrecoBase * 2
        };

        venda2.Itens.Add(item3);
        venda2.Itens.Add(item4);
        venda2.ValorTotal = item3.Subtotal + item4.Subtotal;

        context.Vendas.Add(venda2);
        await context.SaveChangesAsync();

        // =========================
        // ROYALTY
        // =========================

        var royalty = new Royalty
        {
            UnidadeFranqueadaId = unidadeCentro.Id,
            Percentual = 5,
            FaturamentoPeriodo = venda1.ValorTotal,
            ValorRoyalty = venda1.ValorTotal * 5 / 100,
            InicioPeriodo = DateTime.UtcNow.AddDays(-30),
            FimPeriodo = DateTime.UtcNow,
            StatusPagamento = "PENDENTE"
        };

        context.Royalties.Add(royalty);

        await context.SaveChangesAsync();
    }
}