namespace Franquias.Api.DTOs;

public class MovimentacaoEstoqueDto
{
    public int EstoqueId { get; set; }
    public string Tipo { get; set; } = "";
    public int Quantidade { get; set; }
    public string Observacao { get; set; } = "";
}