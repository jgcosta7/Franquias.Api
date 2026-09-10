namespace Franquias.Api.Models;

public class Estoque
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public int ProdutoServicoId { get; set; }

    public ProdutoServico? ProdutoServico { get; set; }

    public int Quantidade { get; set; }

    public int EstoqueMinimo { get; set; }
}