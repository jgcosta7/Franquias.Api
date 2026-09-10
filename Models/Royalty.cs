namespace Franquias.Api.Models;

public class Royalty
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public decimal Percentual { get; set; }

    public decimal FaturamentoPeriodo { get; set; }

    public decimal ValorRoyalty { get; set; }

    public DateTime InicioPeriodo { get; set; }

    public DateTime FimPeriodo { get; set; }

    public string StatusPagamento { get; set; } = "PENDENTE";

    public DateTime? DataPagamento { get; set; }
}