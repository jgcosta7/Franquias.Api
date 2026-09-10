namespace Franquias.Api.Models;

public class Fornecedor
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public ICollection<ProdutoServico> Produtos { get; set; } =
        new List<ProdutoServico>();
}