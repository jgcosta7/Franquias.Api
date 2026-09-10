namespace Franquias.Api.Models;

public class MovimentacaoEstoque
{
    public int Id { get; set; }

    public int EstoqueId { get; set; }

    public Estoque? Estoque { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public string Observacao { get; set; } = string.Empty;

    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
}