namespace Franquias.Api.Models;

public class Venda
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public DateTime DataVenda { get; set; } = DateTime.UtcNow;

    public decimal ValorTotal { get; set; }

    public string Status { get; set; } = "CONFIRMADA";

    public ICollection<ItemVenda> Itens { get; set; } =
        new List<ItemVenda>();
}