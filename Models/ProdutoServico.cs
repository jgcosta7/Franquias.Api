namespace Franquias.Api.Models;

public class ProdutoServico
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal PrecoBase { get; set; }

    public bool Ativo { get; set; } = true;

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public ICollection<Estoque> Estoques { get; set; } =
        new List<Estoque>();

    public ICollection<ItemVenda> ItensVenda { get; set; } =
        new List<ItemVenda>();

    public ICollection<Fornecedor> Fornecedores { get; set; } =
        new List<Fornecedor>();
}