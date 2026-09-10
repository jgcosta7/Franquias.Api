namespace Franquias.Api.Models;

public class ItemVenda
{
    public int Id { get; set; }

    public int VendaId { get; set; }

    public Venda? Venda { get; set; }

    public int ProdutoServicoId { get; set; }

    public ProdutoServico? ProdutoServico { get; set; }

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal { get; set; }
}