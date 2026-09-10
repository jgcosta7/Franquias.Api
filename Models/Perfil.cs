namespace Franquias.Api.Models;

public class Perfil
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}