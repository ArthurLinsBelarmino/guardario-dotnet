namespace GuardaRio.Abrigos.Models;

public class NecessidadeAbrigo
{
    public int Id { get; set; }
    public string Item { get; set; } = string.Empty;
    public int QuantidadeNecessaria { get; set; }
    public int QuantidadeRecebida { get; set; }
    public bool Urgente { get; set; }

    public int AbrigoId { get; set; }
    public Abrigo? Abrigo { get; set; }
}
