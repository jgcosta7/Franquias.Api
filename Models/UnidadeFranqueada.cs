namespace Franquias.Api.Models;

public class UnidadeFranqueada
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }

    public bool Ativo { get; set; } = true;

    public string NomeFranqueado { get; set; } = string.Empty;

    public string NomeResponsavel { get; set; } = string.Empty;

    public int FranqueadoraId { get; set; }

    public Franqueadora? Franqueadora { get; set; }

    public ICollection<Estoque> Estoques { get; set; } =
        new List<Estoque>();

    public ICollection<Venda> Vendas { get; set; } =
        new List<Venda>();

    public ICollection<Royalty> Royalties { get; set; } =
        new List<Royalty>();

    public ICollection<ChamadoSuporte> Chamados { get; set; } =
        new List<ChamadoSuporte>();
}