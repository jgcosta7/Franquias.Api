namespace Franquias.Api.Models;

public class ChamadoSuporte
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }

    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public string Prioridade { get; set; } = "MEDIA";

    public string Descricao { get; set; } = string.Empty;

    public string Status { get; set; } = "ABERTO";

    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

    public DateTime? DataEncerramento { get; set; }

    public string? ObservacaoEncerramento { get; set; }
}