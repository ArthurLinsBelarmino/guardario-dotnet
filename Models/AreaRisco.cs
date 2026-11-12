namespace GuardaRio.Abrigos.Models;

public class AreaRisco
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public int PopulacaoEstimada { get; set; }

    public List<Abrigo> Abrigos { get; set; } = new();
}
