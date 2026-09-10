namespace Franquias.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public int PerfilId { get; set; }

    public Perfil? Perfil { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}